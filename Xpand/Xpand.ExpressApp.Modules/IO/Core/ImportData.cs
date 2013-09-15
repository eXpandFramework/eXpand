using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using System.Linq;
using DevExpress.Xpo.Metadata.Helpers;
using Xpand.Utils.Helpers;
using Xpand.Xpo.ConnectionProviders;
using Xpand.Xpo.MetaData;
using Fasterflect;

namespace Xpand.ExpressApp.IO.Core {
    public class InitDataImporter {
        class ObjectValue {
            public readonly int ImportGeneration;
            public object Object;
            public ObjectValue(int importGeneration, object obj) {
                ImportGeneration = importGeneration;
                Object = obj;
            }
        }
        class FillRefList {
            public readonly List<object> OwnerList = new List<object>();
            public readonly List<object> RefKeyList = new List<object>();
        }


        public void Import(Func<UnitOfWork> createOutputUow, Func<UnitOfWork> createInputOuw) {
            Tracing.Tracer.LogVerboseSubSeparator("Import started");
            using (UnitOfWork mainOutputUow = createOutputUow()) {
                using (UnitOfWork mainInputUow = createInputOuw()) {
                    OnCreatingDynamicDictionary();
                    var dataStoreSchemaExplorer = (IDataStoreSchemaExplorer)((BaseDataLayer)mainInputUow.DataLayer).ConnectionProvider;
                    var dictionaryMapper = new DictionaryMapper(dataStoreSchemaExplorer.GetStorageTables(dataStoreSchemaExplorer.GetStorageTablesList(true)));
                    dictionaryMapper.Map(mainOutputUow.Dictionary, mainInputUow.Dictionary);
                    Tracing.Tracer.LogVerboseText("Dictionary created");
                    foreach (var objectClassInfo in dictionaryMapper.InputClassInfos) {
                        ImportCore(createOutputUow, createInputOuw, objectClassInfo);
                    }
                    Tracing.Tracer.LogVerboseText("Comming data");
                    OnCommitingData();
                    Tracing.Tracer.LogVerboseText("Data commited");
                    ClearObjectCache();
                }
            }
            Tracing.Tracer.LogVerboseSubSeparator("Import finished");
        }
        int _currentImportGeneration;
        readonly Dictionary<Type, Dictionary<object, ObjectValue>> _importedObjecs = new Dictionary<Type, Dictionary<object, ObjectValue>>();
        readonly Dictionary<Type, Func<UnitOfWork, object>> objectsCreatorDictionary = new Dictionary<Type, Func<UnitOfWork, object>>();
        readonly ParameterExpression _uowParameter = Expression.Parameter(typeof(Session), "session");
        readonly Dictionary<InputMemberInfo, FillRefList> _fillRefsDictionary = new Dictionary<InputMemberInfo, FillRefList>();
        readonly Dictionary<InputMemberInfo, FillRefList> _fillRefsAndImportDictionary = new Dictionary<InputMemberInfo, FillRefList>();
        object CreateInstance(Type type, UnitOfWork uow) {
            Func<UnitOfWork, object> creator;
            if (!objectsCreatorDictionary.TryGetValue(type, out creator)) {
                ConstructorInfo ci = type.GetConstructor(new[] { typeof(Session) });
                if (ci == null) {
                    ci = type.GetConstructor(new[] { typeof(UnitOfWork) });
                    if (ci == null) throw new InvalidOperationException("ci");
                    creator = Expression.Lambda<Func<UnitOfWork, object>>(Expression.Convert(Expression.New(ci, _uowParameter), typeof(object)), _uowParameter).Compile();
                } else {
                    creator = Expression.Lambda<Func<UnitOfWork, object>>(Expression.Convert(Expression.New(ci, Expression.Convert(_uowParameter, typeof(Session))), typeof(object)), _uowParameter).Compile();
                }
                objectsCreatorDictionary.Add(type, creator);
            }
            return creator(uow);
        }

        Dictionary<object, ObjectValue> GetObjectDictionary(Type objectType) {
            Dictionary<object, ObjectValue> objectDict;
            if (!_importedObjecs.TryGetValue(objectType, out objectDict)) {
                objectDict = new Dictionary<object, ObjectValue>();
                _importedObjecs.Add(objectType, objectDict);
            }
            return objectDict;
        }

        void AddObjectToCache(Dictionary<object, ObjectValue> objectDictionary, object objectKey, object obj) {
            objectDictionary[objectKey] = new ObjectValue(_currentImportGeneration, obj);
        }

        object GetCachedObject(Dictionary<object, ObjectValue> objectDictionary, UnitOfWork outputOuw, object objectKey, Type objectType, bool reloadIfNotExists, out bool returnKey) {
            returnKey = false;
            ObjectValue value = objectDictionary[objectKey];
            if (value.ImportGeneration < _currentImportGeneration) {
                if (reloadIfNotExists) {
                    return outputOuw.GetObjectByKey(objectType, value.Object);
                }
                returnKey = true;
            }
            return value.Object;
        }

        void ClearObjectCache() {
            _importedObjecs.Clear();
        }

        void AddFillRefInfo(InputMemberInfo memberInfo, object owner, object refKey) {
            FillRefList fillRefsList;
            if (!_fillRefsDictionary.TryGetValue(memberInfo, out fillRefsList)) {
                fillRefsList = new FillRefList();
                _fillRefsDictionary.Add(memberInfo, fillRefsList);
            }
            fillRefsList.OwnerList.Add(owner);
            fillRefsList.RefKeyList.Add(refKey);
        }

        void AddFillRefAndImportInfo(InputMemberInfo memberInfo, object owner, object refImportKey) {
            FillRefList fillRefsList;
            if (!_fillRefsAndImportDictionary.TryGetValue(memberInfo, out fillRefsList)) {
                fillRefsList = new FillRefList();
                _fillRefsAndImportDictionary.Add(memberInfo, fillRefsList);
            }
            fillRefsList.OwnerList.Add(owner);
            fillRefsList.RefKeyList.Add(refImportKey);
        }

        void GenerationNext(UnitOfWork outputOuw) {
            foreach (ObjectValue objectValue in ObjectValues()) {
                objectValue.Object = outputOuw.GetKeyValue(objectValue.Object);
            }
            _currentImportGeneration++;
        }

        IEnumerable<ObjectValue> ObjectValues() {
            return _importedObjecs.Values.SelectMany(objectDict =>
                    objectDict.Values.Where(objectValue => objectValue.ImportGeneration == _currentImportGeneration));
        }


        void ImportCore(Func<UnitOfWork> createOutputUow, Func<UnitOfWork> createInputUow, InputObjectClassInfo inputObjectClassInfo) {
            UnitOfWork inputUow = createInputUow();
            UnitOfWork outputUow = createOutputUow();
            try {
                var inputMemberInfos =MemberMapper.MemberInfos(inputObjectClassInfo).OfType<InputMemberInfo>().ToList();
                var keys = InputObjectKeys(inputObjectClassInfo, inputUow);
                int position = 0;
                int commitAccumulator = 0;
                int length = keys.Count;
                while (position < length) {
                    int chunkSize = XpoDefault.GetTerminalInSize(length - position);
                    var keysToImport = keys.GetRange(position, chunkSize);
                    var objectsToImport = inputUow.GetObjectsByKey(inputObjectClassInfo, keysToImport, false);
                    position += chunkSize;
                    commitAccumulator += chunkSize;
                    PrefetchDelayedMembers(inputMemberInfos, inputUow, objectsToImport);
                    ImportMembers(inputObjectClassInfo, objectsToImport, keysToImport, outputUow, inputMemberInfos,inputUow);
                    do {
                        var refsAndImports = RefsAndImports(outputUow);
                        foreach (KeyValuePair<InputMemberInfo, FillRefList> pair in refsAndImports) {
                            var memberInfo = pair.Key.OutputMemberInfo;
                            var fillRefsList = pair.Value;
                            var classInfo = memberInfo.IsAssociationList? memberInfo.CollectionElementType: memberInfo.ReferenceType;
                            var memberList =MemberMapper.MemberInfos(pair.Key.ReferenceType).OfType<InputMemberInfo>().ToList();
                            var importClassInfo = pair.Key.ReferenceType;
                            var objects = inputUow.GetObjectsByKey(importClassInfo, fillRefsList.RefKeyList, false);
                            PrefetchDelayedMembers(inputMemberInfos, inputUow, objects);
                            ImportRefs(classInfo, objects, fillRefsList, outputUow, inputUow, memberList, memberInfo);
                        }
                    } while (_fillRefsDictionary.Count > 0 || _fillRefsAndImportDictionary.Count > 0);

                    if (commitAccumulator > 400) {
                        outputUow.CommitChanges();
                        GenerationNext(outputUow);
                        var ou = outputUow;
                        var iu = inputUow;
                        outputUow = createOutputUow();
                        inputUow = createInputUow();
                        ou.Dispose();
                        iu.Dispose();
                        commitAccumulator = 0;
                        
                        OnTransformingRecords(new TransformingRecordsArgs(inputObjectClassInfo.FullName, inputObjectClassInfo.OutputClassInfo.FullName, position));
                    }
                }

// ReSharper disable RedundantCatchClause
#pragma warning disable 168
            } catch (Exception e) {
#pragma warning restore 168
                throw;

            } finally {
// ReSharper restore RedundantCatchClause
                outputUow.CommitChanges();
                OnTransformRecords(new TransformRecordsArgs(inputObjectClassInfo.FullName, inputObjectClassInfo.OutputClassInfo.FullName));
                GenerationNext(outputUow);
                outputUow.Dispose();
                inputUow.Dispose();
            }
        }

        void ImportRefs(XPClassInfo classInfo, ICollection objects, FillRefList fillRefsList,
                        UnitOfWork outputUow, UnitOfWork inputUow, List<InputMemberInfo> memberList, XPMemberInfo memberInfo) {
            int counter = 0;
            var memberObjectDictionary = GetObjectDictionary(classInfo.ClassType);
            foreach (object importObj in objects) {
                object owner = fillRefsList.OwnerList[counter];
                object refKey = fillRefsList.RefKeyList[counter];
                counter++;
                object obj;
                if (!memberObjectDictionary.ContainsKey(refKey)) {
                    obj = ImportSingle(refKey, importObj, outputUow, inputUow, classInfo, memberList);
                } else {
                    bool returnKey;
                    obj = GetCachedObject(memberObjectDictionary, outputUow, refKey, classInfo.ClassType, true, out returnKey);
                }
                if (memberInfo.IsAssociationList) {
                    var list = memberInfo.GetValue(owner) as IList;
                    if (list == null) continue;
                    list.Add(obj);
                } else {
                    memberInfo.SetValue(owner, obj);
                }
            }
        }

        IEnumerable<KeyValuePair<InputMemberInfo, FillRefList>> RefsAndImports(UnitOfWork outputUow) {
            foreach (KeyValuePair<InputMemberInfo, FillRefList> pair in _fillRefsDictionary) {
                XPMemberInfo memberInfo = pair.Key;
                FillRefList fillRefsList = pair.Value;
                Type memberType = memberInfo.IsAssociationList ? memberInfo.CollectionElementType.ClassType : memberInfo.ReferenceType.ClassType;
                var objects = outputUow.GetObjectsByKey(outputUow.GetClassInfo(memberType), fillRefsList.RefKeyList, false);
                int counter = 0;
                var memberObjectDictionary = GetObjectDictionary(memberType);
                foreach (object obj in objects) {
                    object owner = fillRefsList.OwnerList[counter];
                    object refKey = fillRefsList.RefKeyList[counter];
                    counter++;
                    AddObjectToCache(memberObjectDictionary, refKey, obj);
                    if (memberInfo.IsAssociationList) {
                        var list = memberInfo.GetValue(owner) as IList;
                        if (list == null) continue;
                        list.Add(obj);
                    } else {
                        memberInfo.SetValue(owner, obj);
                    }
                }
            }
            _fillRefsDictionary.Clear();
            List<KeyValuePair<InputMemberInfo, FillRefList>> refsAndImports = _fillRefsAndImportDictionary.ToList();
            _fillRefsAndImportDictionary.Clear();
            return refsAndImports;
        }

        List<object> InputObjectKeys(XPClassInfo classInfo, UnitOfWork inputUow) {
            var operatorCollection = new CriteriaOperatorCollection{
                        new OperandProperty(classInfo.KeyProperty.Name)
                    };
            return inputUow.SelectData(classInfo, operatorCollection, null, false, 0, new SortingCollection()).Select(r => r[0]).ToList();
        }

        void ImportMembers(InputObjectClassInfo inputObjectClassInfo, ICollection objectsToImport, List<object> keysToImport, UnitOfWork outputUow, List<InputMemberInfo> inputMemberInfos, UnitOfWork inputUow) {
            int keysCounter = 0;
            var objectDictionary = GetObjectDictionary(inputObjectClassInfo.OutputClassInfo.ClassType);
            foreach (var objectToImport in objectsToImport) {
                object objectKey = keysToImport[keysCounter++];
                if (objectDictionary.ContainsKey(objectKey)) continue;
                var xpoObject = CreateInstance(inputObjectClassInfo.OutputClassInfo.ClassType, outputUow);
                AddObjectToCache(objectDictionary, objectKey, xpoObject);
                foreach (var inputMemberInfo in inputMemberInfos) {
                    var importedMemberInfo = new ImportedMemberInfo(inputObjectClassInfo, inputMemberInfo);
                    if (inputMemberInfo.IsAssociationList) {
                        ImportManyToManyCollection(outputUow, inputUow, xpoObject, importedMemberInfo, objectToImport, inputMemberInfo);
                    } else {
                        ImportSimpleProperty(outputUow, inputUow, inputMemberInfo, importedMemberInfo, objectToImport, xpoObject);
                    }
                }
            }
        }
        internal class ImportedMemberInfo {
            readonly XPClassInfo _classInfo;
            readonly XPMemberInfo _memberInfo;

            public ImportedMemberInfo(XPClassInfo classInfo, XPMemberInfo memberInfo) {
                _classInfo = classInfo;
                _memberInfo = memberInfo;
            }

            public XPMemberInfo MemberInfo {
                get { return _memberInfo; }
            }

            public InitialDataAttribute InitialData {
                get { return _memberInfo.FindAttributeInfo(typeof(InitialDataAttribute)) as InitialDataAttribute; }
            }

            public XPClassInfo ClassInfo {
                get { return _classInfo; }
            }
            public override string ToString() {
                return String.Format("{0},{1}", _classInfo.ClassType.Name, _memberInfo.Name);
            }
        }


        void PrefetchDelayedMembers(IEnumerable<InputMemberInfo> memberInfos, UnitOfWork inputUow, ICollection objectsToImport) {
            foreach (var memberInfo in memberInfos) {
                if (memberInfo.IsDelayed) {
                    inputUow.PreFetch(memberInfo.Owner, objectsToImport, memberInfo.Name);
                }
            }
        }

        object ImportSingle(object objectKeyToImport, object objectToImport, UnitOfWork outputUow, UnitOfWork inputUow, XPClassInfo classInfo, IEnumerable<InputMemberInfo> memberInfos) {
            var objectDictionary = GetObjectDictionary(classInfo.ClassType);
            var xpoObject = CreateInstance(classInfo.ClassType, outputUow);
            AddObjectToCache(objectDictionary, objectKeyToImport, xpoObject);
            foreach (var memberInfo in memberInfos) {
                var importedMemberInfo = new ImportedMemberInfo(classInfo, memberInfo);
                if (importedMemberInfo.MemberInfo.IsAssociationList) {
                    ImportManyToManyCollection(outputUow, inputUow, xpoObject, importedMemberInfo, objectToImport, memberInfo);
                } else {
                    ImportSimpleProperty(outputUow, inputUow, memberInfo, importedMemberInfo, objectToImport, xpoObject);
                }
            }
            return xpoObject;
        }

        void ImportSimpleProperty(UnitOfWork outputUow, UnitOfWork inputUow, InputMemberInfo memberInfo, ImportedMemberInfo importedMemberInfo, object objectToImport, object xpoObject) {
            XPMemberInfo xpMemberInfo = importedMemberInfo.MemberInfo;
            if (xpMemberInfo != null) {
                var value = xpMemberInfo.GetValue(objectToImport);
                if (memberInfo.ReferenceType != null && memberInfo.ReferenceType.IsPersistent) {
                    var memberType = ((InputObjectClassInfo)memberInfo.ReferenceType).OutputClassInfo.ClassType;
                    bool returnKey;
                    bool returnImportKey;
                    value = GetReferenceMemberValue(outputUow, value, memberType, out returnKey, out returnImportKey);
                    if (returnKey) {
                        AddFillRefInfo(memberInfo, xpoObject, value);
                        return;
                    }
                    if (returnImportKey) {
                        value = inputUow.GetKeyValue(value);
                        AddFillRefAndImportInfo(memberInfo, xpoObject, value);
                        return;
                    }
                }
                memberInfo.SetOutputMemberValue(xpoObject, value);
            }
        }

        void ImportManyToManyCollection(UnitOfWork outputUow, UnitOfWork intputUow,
                                     object xpoObject, ImportedMemberInfo importedMemberInfo, object objectToImport,
                                     InputMemberInfo memberInfo) {
            if (!importedMemberInfo.InitialData.DataProvider)
                return;
            var xpClassInfo = intputUow.Dictionary.GetClassInfo(null, importedMemberInfo.InitialData.DataProviderTableName);
            var criteriaOperator = CriteriaOperator.Parse(importedMemberInfo.InitialData.DataProviderQueryColumnName + "=?", objectToImport);
            using (var xpCollection = new XPCollection(intputUow, xpClassInfo, criteriaOperator)) {
                var collection = (XPBaseCollection)memberInfo.GetValue(xpoObject);
                var dataProviderResultInfo = xpClassInfo.GetMember(importedMemberInfo.InitialData.DataProviderResultColumnName);
                foreach (var o in xpCollection) {
                    var listElementTypeInfo = memberInfo.CollectionElementType;
                    var memberValue = dataProviderResultInfo.GetValue(o);
                    bool returnKey;
                    bool returnImportKey;
                    var referenceMemberValue = GetReferenceMemberValue(outputUow, memberValue, listElementTypeInfo.ClassType, out returnKey, out returnImportKey);
                    if (returnKey) {
                        AddFillRefInfo(memberInfo, xpoObject, referenceMemberValue);
                    } if (returnImportKey) {
                        AddFillRefAndImportInfo(memberInfo, xpoObject, referenceMemberValue);
                    } else {
                        collection.BaseAdd(referenceMemberValue);
                    }
                }
            }
        }



        object GetReferenceMemberValue(UnitOfWork outputUow, object memberValue, Type memberType, out bool returnKey, out bool returnImportKey) {
            returnKey = false;
            returnImportKey = false;
            if (memberValue != null) {
                var objectDictionary = GetObjectDictionary(memberType);
                if (!objectDictionary.ContainsKey(memberValue)) {
                    returnImportKey = true;
                    return memberValue;
                }
                object value = GetCachedObject(objectDictionary, outputUow, memberValue, memberType, false, out returnKey);
                Guard.ArgumentNotNull(value, "value");
                return value;
            }
            return null;
        }

        public event EventHandler CreatingDynamicDictionary;

        void OnCreatingDynamicDictionary() {
            var handler = CreatingDynamicDictionary;
            if (handler != null) handler(this, EventArgs.Empty);
        }
        public event EventHandler<TransformingRecordsArgs> TransformingRecords;
        public event EventHandler<TransformRecordsArgs> TransformRecords;

        protected virtual void OnTransformingRecords(TransformingRecordsArgs e) {
            Tracing.Tracer.LogVerboseValue("TransformingRecords",e);
            var handler = TransformingRecords;
            if (handler != null) handler(this, e);
        }
        protected virtual void OnTransformRecords(TransformRecordsArgs e) {
            Tracing.Tracer.LogVerboseValue("TransformRecords", e);
            var handler = TransformRecords;
            if (handler != null) handler(this, e);
        }

        public event EventHandler CommitingData;

        void OnCommitingData() {
            var handler = CommitingData;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }

    public class TransformRecordsArgs:EventArgs {
        readonly string _inputClassName;
        readonly string _outputClassName;

        public TransformRecordsArgs(string inputClassName, string outputClassName) {
            _inputClassName = inputClassName;
            _outputClassName = outputClassName;
        }

        public string OutputClassName {
            get { return _outputClassName; }
        }

        public string InputClassName {
            get { return _inputClassName; }
        }

        public override string ToString() {
            return string.Format("OutputClassName: {0}", OutputClassName );
        }
    }
    public class TransformingRecordsArgs : TransformRecordsArgs {
        
        readonly int _position = -1;

        public TransformingRecordsArgs(string inputClassName, string outputClassName, int position) : base(inputClassName, outputClassName) {
            _position = position;
        }      

        public int Position {
            get { return _position; }
        }
        public override string ToString() {
            var s = base.ToString();
            return string.Format("{0} " + "Position: {1}", s, Position);
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false)]
    public class InitialDataAttribute : Attribute {
        public string Name { get; set; }
        public bool AllOwnMembers { get; set; }
        public string BaseMembers { get; set; }

        public bool DataProvider {
            get { return !string.IsNullOrEmpty(DataProviderQueryColumnName) && !string.IsNullOrEmpty(DataProviderResultColumnName) && !string.IsNullOrEmpty(DataProviderTableName); }
        }

        public InitialDataAttribute() {
            ThrowIfColumnNotFound = true;
        }

        public string DataProviderTableName { get; set; }

        public string DataProviderQueryColumnName { get; set; }

        public string DataProviderResultColumnName { get; set; }

        public bool ThrowIfColumnNotFound { get; set; }

        public Type inputKeyPropertyType { get; set; }

        public string InputKeyPropertyName { get; set; }

        public bool CaseInSensitive { get; set; }

        
    }

    public class InputMemberInfo : XpandCustomMemberInfo {

        readonly XPMemberInfo _outputMemberInfo;
        readonly Type _conversionType;
        readonly ImportValueConverterAttribute _importValueConverterAttribute;


        public InputMemberInfo(XPClassInfo owner, string propertyName, XPClassInfo referenceType, bool nonPersistent, XPMemberInfo outputMemberInfo, params Attribute[] attributes)
            : base(owner, propertyName, referenceType.ClassType, referenceType, nonPersistent, false) {
            Guard.ArgumentNotNull(outputMemberInfo, "outputMemberInfo");
            _outputMemberInfo = outputMemberInfo;
            AddAttributes(attributes);
        }

        public InputMemberInfo(XPClassInfo owner, string propertyName, DBColumnType dbColumnType, Type propertyType, bool nonPersistent, XPMemberInfo outputMemberInfo, params Attribute[] attributes)
            : base(owner, propertyName, PropertyType(propertyType, dbColumnType), null, nonPersistent, false) {
            Guard.ArgumentNotNull(outputMemberInfo, "outputMemberInfo");
            _outputMemberInfo = outputMemberInfo;
            _conversionType = propertyType;
            AddAttributes(attributes);
            _importValueConverterAttribute = FindAttributeInfo(typeof (ImportValueConverterAttribute)) as ImportValueConverterAttribute;
        }

        static Type PropertyType(Type propertyType, DBColumnType dbColumnType) {
            var type = dbColumnType.GetType(propertyType);
            Guard.ArgumentNotNull(dbColumnType, dbColumnType.ToString());
            return propertyType == type ? propertyType : type;
        }
        
        object Convert(object theObject, object theValue, Type conversionType) {
            object result=theValue;
            if (_importValueConverterAttribute != null) {
                if (_importValueConverterAttribute.ValueConverter.TryConvert(theObject, Name, ref result, conversionType))
                    return result;
            }
            var changed = result.TryToChange(conversionType, out result, Conversion.GuessValues);
            if (changed)
                return result;
            
            throw new ImportFormatException(
                string.Format("Cannot format value {0} to {1} for the member {2} of the class {3}. Consider using the {4} in the "+_outputMemberInfo.Name+" member of the "+_outputMemberInfo.Owner.FullName+" class",
                    theValue, conversionType, this, Owner.FullName, typeof (ImportValueConverterAttribute)));
        }

        void AddAttributes(IEnumerable<Attribute> attributes) {
            foreach (Attribute attribute in attributes.Where(attribute => !(attribute is AssociationAttribute)))
                AddAttribute(attribute);
        }

        public XPMemberInfo OutputMemberInfo {
            get { return _outputMemberInfo; }
        }

        public static bool CanChangeType(object value, Type conversionType) {
            if (conversionType == null) {
                return false;
            }
            if (value == null) {
                return false;
            }
            var convertible = value as IConvertible;
            return convertible != null;
        }

        public void SetOutputMemberValue(object theObject, object theValue) {
            var outputValue = OutputValue(theObject,theValue);
            _outputMemberInfo.SetValue(theObject, outputValue);
        }

        object OutputValue(object theObject, object theValue) {
            return CanChangeType(theValue, _conversionType) ? Convert(theObject,theValue, _conversionType) : theValue;
        }
    }

    public class ImportFormatException : FormatException {
        public ImportFormatException(string message) : base(message) {
        }
    }
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    public class ImportValueConverterAttribute:Attribute {
        readonly Type _valueConverterType;
        public ImportValueConverterAttribute(Type valueConverterType) {
            DevExpress.ExpressApp.Utils.Guard.TypeArgumentIs(typeof(IImportValueConverter), valueConverterType, "ValueConverterType");
            _valueConverterType = valueConverterType;
        }

        public Type ValueConverterType {
            get { return _valueConverterType; }
        }

        public IImportValueConverter ValueConverter {
            get { return (IImportValueConverter)ValueConverterType.CreateInstance(); }
        }
    }

    public interface IImportValueConverter {
        bool TryConvert(object theObject, string memberName, ref object value, Type conversionType);
    }
    public class DictionaryMapper {
        readonly DBTable[] _dbTables;

        IList<InputObjectClassInfo> _inputClassInfos;

        public DictionaryMapper(DBTable[] dbTables) {
            _dbTables = dbTables;
        }

        public IList<InputObjectClassInfo> InputClassInfos {
            get { return _inputClassInfos; }
        }

        public void Map(XPDictionary outputDictionary, XPDictionary inputDictionary) {
            var classMapper = new ClassMapper();
            classMapper.Map(outputDictionary, inputDictionary);
            _inputClassInfos = classMapper.InputClassInfos;
            foreach (var classInfo in _inputClassInfos) {
                var dbTable = GetTable(classInfo);
                if (dbTable != null) {
                    var memberMapper = new MemberMapper(dbTable);
                    memberMapper.Map(classInfo.OutputClassInfo, classInfo);
                }
            }
        }

        DBTable GetTable(InputObjectClassInfo classInfo) {
            return _dbTables.SingleOrDefault(table => table.Name == classInfo.TableName);
        }
    }

    public class MemberMapper {
        readonly DBTable _dbTable;

        public MemberMapper(DBTable dbTable) {
            _dbTable = dbTable;
        }

        public void Map(XPClassInfo outputClassInfo, XPClassInfo inputClassInfo) {
            var caseInSensitive = CaseInSensitive(outputClassInfo);
            if (!AllOwnMembers(outputClassInfo)) {
                foreach (var memberInfo in MemberInfos(outputClassInfo)) {
                    CreateMemberFromAttribute(inputClassInfo, memberInfo,caseInSensitive);
                }
            } else {
                CreateAllOwnMembers(outputClassInfo, inputClassInfo,caseInSensitive);
            }
            CreateBaseMembers(outputClassInfo, inputClassInfo, caseInSensitive);
            CreateKey(outputClassInfo,inputClassInfo);
        }

        internal static IEnumerable<XPMemberInfo> MemberInfos(XPClassInfo classInfo) {
            return classInfo.Members.Where(info => info.IsPersistent && !(info is ServiceField) && !info.IsReadOnly);
        }

        void CreateBaseMembers(XPClassInfo outputClassInfo, XPClassInfo inputClassInfo, bool caseInSensitive) {
            foreach (var memberInfo in BaseMemberInfos(outputClassInfo)) {
                var xpMemberInfo = memberInfo.Value;
                var propertyName = memberInfo.Key;
                CreateMemberCore(inputClassInfo, propertyName, xpMemberInfo, caseInSensitive);
            }
        }
        DBColumn ColumnExists(string columnName, bool caseInSensitive) {
            return _dbTable.Columns.FirstOrDefault(column => !caseInSensitive ? column.Name == columnName : column.Name.ToLower() == columnName.ToLower());
        }

        void CreateMemberCore(XPClassInfo inputClassInfo, string propertyName, XPMemberInfo xpMemberInfo, bool caseInSensitive) {
            var dbColumn = ColumnExists(propertyName, caseInSensitive);
            if (xpMemberInfo != null && dbColumn != null) {
                var referenceType = xpMemberInfo.ReferenceType;
                var dbColumnType = dbColumn.ColumnType;
                if (referenceType == null) {
                    new InputMemberInfo(inputClassInfo, propertyName, dbColumnType, xpMemberInfo.MemberType, !xpMemberInfo.IsPersistent, xpMemberInfo, xpMemberInfo.Attributes);
                } else if (xpMemberInfo.IsPersistent) {
                    var attributeInfo = (InitialDataAttribute)referenceType.FindAttributeInfo(typeof(InitialDataAttribute));
                    if (attributeInfo != null) {
                        var classInfo = inputClassInfo.Dictionary.QueryClassInfo(null, attributeInfo.Name ?? referenceType.ClassType.Name);
                        new InputMemberInfo(inputClassInfo, propertyName, classInfo, !xpMemberInfo.IsPersistent, xpMemberInfo, xpMemberInfo.Attributes);
                    } else {
                        new InputMemberInfo(inputClassInfo, propertyName, dbColumnType, xpMemberInfo.ReferenceType.KeyProperty.MemberType, !xpMemberInfo.IsPersistent, xpMemberInfo, xpMemberInfo.Attributes);
                    }
                }
            }
        }

        Dictionary<string, XPMemberInfo> BaseMemberInfos(XPClassInfo outputClassInfo) {
            var members = ((InitialDataAttribute)outputClassInfo.FindAttributeInfo(typeof(InitialDataAttribute))).BaseMembers;
            var xpMemberInfos = new Dictionary<string, XPMemberInfo>();
            if (members != null) {
                var baseMembers = members.Split(',');
                foreach (var baseMember in baseMembers) {
                    var strings = baseMember.Split('|');
                    var xpMemberInfo = outputClassInfo.FindMember(strings[0]);
                    xpMemberInfos.Add(strings[strings.Length - 1], xpMemberInfo);
                }
            }
            return xpMemberInfos;
        }

        void CreateKey(XPClassInfo outputClassInfo, XPClassInfo classInfo) {
            var initialDataAttribute = ((InitialDataAttribute) outputClassInfo.FindAttributeInfo(typeof (InitialDataAttribute)));
            var inputKeyPropertyName = initialDataAttribute.InputKeyPropertyName;
            if (!String.IsNullOrEmpty(inputKeyPropertyName) ) {
                var member = classInfo.FindMember(inputKeyPropertyName);
                if (member == null)
                    classInfo.CreateMember(inputKeyPropertyName, initialDataAttribute.inputKeyPropertyType).AddAttribute(new KeyAttribute(true));
                else {
                    member.AddAttribute(new KeyAttribute(true));
                }
            }
            else if (!HasKey(classInfo)) {
                classInfo.CreateMember("Oid_" + classInfo.TableName, typeof(int)).AddAttribute(new KeyAttribute(true));
            }
        }

        bool HasKey(XPClassInfo classInfo) {
            XPMemberInfo xpMemberInfo;
            try {
#pragma warning disable 168
                xpMemberInfo = classInfo.KeyProperty;
#pragma warning restore 168
            } catch (KeyPropertyAbsentException) {
                return false;
            }
            return xpMemberInfo != null;
        }

        void CreateMemberFromAttribute(XPClassInfo classInfo, XPMemberInfo memberInfo, bool caseInSensitive) {
            var initialDataAttribute = (InitialDataAttribute)memberInfo.FindAttributeInfo(typeof(InitialDataAttribute));
            if (initialDataAttribute != null) {
                CreateMemberCore(classInfo, initialDataAttribute.Name ?? memberInfo.Name, memberInfo, caseInSensitive);
            }
        }

        bool CaseInSensitive(XPClassInfo xpClassInfo) {
            var attribute = xpClassInfo.FindAttributeInfo(typeof(InitialDataAttribute)) as InitialDataAttribute;
            return attribute == null || attribute.CaseInSensitive;
        }

        bool AllOwnMembers(XPClassInfo classInfo) {
            return ((InitialDataAttribute)classInfo.FindAttributeInfo(typeof(InitialDataAttribute))).AllOwnMembers;
        }

        void CreateAllOwnMembers(XPClassInfo outputClassInfo, XPClassInfo inputClassInfo, bool caseInSensitive) {
            foreach (var memberInfo in MemberInfos(outputClassInfo)) {
                if (memberInfo.HasAttribute(typeof(InitialDataAttribute))) {
                    CreateMemberFromAttribute(inputClassInfo, memberInfo, caseInSensitive);
                } else {
                    CreateMemberCore(inputClassInfo, memberInfo.Name, memberInfo, caseInSensitive);
                }
            }
        }
    }
    class IntermediateClassInfo : XPDataObjectClassInfo {
        public IntermediateClassInfo(XPDictionary dictionary, string className)
            : base(dictionary, null, className, GetAttributes()) {
        }
        static Attribute[] GetAttributes() {
            return new Attribute[] { new OptimisticLockingAttribute(false), new DeferredDeletionAttribute(false) };
        }

    }

    public class InputObjectClassInfo : XPDataObjectClassInfo {
        readonly XPClassInfo _outputClassInfo;
        

        public InputObjectClassInfo(XPDictionary dictionary, string className, XPClassInfo outputClassInfo)
            : base(dictionary, null, className, GetAttributes()) {
            Guard.ArgumentNotNull(outputClassInfo, "outputClassInfo");
            _outputClassInfo = outputClassInfo;
        }

        static Attribute[] GetAttributes() {
            return new Attribute[] { new OptimisticLockingAttribute(false), new DeferredDeletionAttribute(false) };
        }


        public XPClassInfo OutputClassInfo {
            get { return _outputClassInfo; }
        }
    }

    public class ClassMapper {
        IList<XPClassInfo> _outputClassInfos = new List<XPClassInfo>();
        readonly IList<InputObjectClassInfo> _inputClassInfos = new List<InputObjectClassInfo>();
        public void Map(XPDictionary outputDictionary, XPDictionary inputDictionary) {
            _outputClassInfos = outputDictionary.Classes.OfType<XPClassInfo>().Where(info => info.HasAttribute(typeof(InitialDataAttribute))).ToList();
            foreach (var classInfo in _outputClassInfos) {
                var initialDataAttribute = (InitialDataAttribute)classInfo.FindAttributeInfo(typeof(InitialDataAttribute));
                var objectClassInfo = new InputObjectClassInfo(inputDictionary, initialDataAttribute.Name ?? classInfo.ClassType.Name, classInfo);
                _inputClassInfos.Add(objectClassInfo);
                MapManyToMany(outputDictionary, inputDictionary, classInfo);
            }
        }

        public IList<XPClassInfo> OutputClassInfos {
            get { return _outputClassInfos; }
        }

        public IList<InputObjectClassInfo> InputClassInfos {
            get { return _inputClassInfos; }
        }

        void MapManyToMany(XPDictionary outputDictionary, XPDictionary inputDictionary, XPClassInfo classInfo) {
            foreach (var memberInfo in ManyToManyMemberInfos(classInfo)) {
                var initialDataAttribute = (InitialDataAttribute)memberInfo.FindAttributeInfo(typeof(InitialDataAttribute));
                var className = initialDataAttribute.DataProviderTableName;
                if (initialDataAttribute.DataProvider && inputDictionary.QueryClassInfo(null, className) == null) {
                    CreateIntermediateClassInfo(outputDictionary, inputDictionary, className, initialDataAttribute, memberInfo);
                }
            }
        }

        void CreateIntermediateClassInfo(XPDictionary outputDictionary, XPDictionary inputDictionary, string className, InitialDataAttribute importDataAttribute, XPMemberInfo memberInfo) {
            var info = new IntermediateClassInfo(inputDictionary, className);
            info.CreateMember("Oid_" + importDataAttribute.DataProviderQueryColumnName, typeof(int)).AddAttribute(new KeyAttribute(true));
            info.CreateMember(importDataAttribute.DataProviderQueryColumnName, memberInfo.Owner.KeyProperty.MemberType);
            var collectionElementType = memberInfo.CollectionElementType.ClassType;
            var memberType = outputDictionary.QueryClassInfo(collectionElementType).KeyProperty.MemberType;
            info.CreateMember(importDataAttribute.DataProviderResultColumnName, memberType);
        }

        IEnumerable<XPMemberInfo> ManyToManyMemberInfos(XPClassInfo classInfo) {
            return classInfo.Members.Where(info => info.IsAssociationList && info.HasAttribute(typeof(InitialDataAttribute)));
        }
    }

}
