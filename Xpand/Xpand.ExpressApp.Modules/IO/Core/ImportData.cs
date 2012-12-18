using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;

namespace Xpand.ExpressApp.IO.Core {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false)]
    public class InitialDataAttribute : Attribute {
        public string Name { get; set; }
        public bool AllOwnMembers { get; set; }
        public string BaseMembers { get; set; }

        internal bool DataProvider {
            get { return !string.IsNullOrEmpty(DataProviderQueryColumnName) && !string.IsNullOrEmpty(DataProviderResultColumnName) && !string.IsNullOrEmpty(DataProviderTableName); }
        }

        public InitialDataAttribute() {
            ThrowIfColumnNotFound = true;
        }

        public string DataProviderTableName { get; set; }

        public string DataProviderQueryColumnName { get; set; }

        public string DataProviderResultColumnName { get; set; }

        public bool ThrowIfColumnNotFound { get; set; }
    }


    public static class InitDataExtensions {
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
        class InitDataImpl {
            int _currentImportGeneration;
            readonly Dictionary<Type, Dictionary<object, ObjectValue>> _importedObjecs = new Dictionary<Type, Dictionary<object, ObjectValue>>();
            readonly Dictionary<Type, Func<UnitOfWork, object>> objectsCreatorDictionary = new Dictionary<Type, Func<UnitOfWork, object>>();
            readonly ParameterExpression _uowParameter = Expression.Parameter(typeof(Session), "session");
            readonly Dictionary<IMemberInfo, FillRefList> _fillRefsDictionary = new Dictionary<IMemberInfo, FillRefList>();
            readonly Dictionary<IMemberInfo, FillRefList> _fillRefsAndImportDictionary = new Dictionary<IMemberInfo, FillRefList>();
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

            void AddFillRefInfo(IMemberInfo memberInfo, object owner, object refKey) {
                FillRefList fillRefsList;
                if (!_fillRefsDictionary.TryGetValue(memberInfo, out fillRefsList)) {
                    fillRefsList = new FillRefList();
                    _fillRefsDictionary.Add(memberInfo, fillRefsList);
                }
                fillRefsList.OwnerList.Add(owner);
                fillRefsList.RefKeyList.Add(refKey);
            }

            void AddFillRefAndImportInfo(IMemberInfo memberInfo, object owner, object refImportKey) {
                FillRefList fillRefsList;
                if (!_fillRefsAndImportDictionary.TryGetValue(memberInfo, out fillRefsList)) {
                    fillRefsList = new FillRefList();
                    _fillRefsAndImportDictionary.Add(memberInfo, fillRefsList);
                }
                fillRefsList.OwnerList.Add(owner);
                fillRefsList.RefKeyList.Add(refImportKey);
            }

            void GenerationNext(UnitOfWork outputOuw) {
                foreach (var objectDict in _importedObjecs.Values) {
                    foreach (ObjectValue objectValue in objectDict.Values) {
                        if (objectValue.ImportGeneration != _currentImportGeneration) continue;
                        objectValue.Object = outputOuw.GetKeyValue(objectValue.Object);
                    }
                }
                _currentImportGeneration++;
            }
            public void ImportCore(Func<UnitOfWork> createOutputUow, Func<UnitOfWork> createInputUow) {
                using (UnitOfWork mainOutputUow = createOutputUow()) {
                    using (UnitOfWork mainInputUow = createInputUow()) {
                        var builder = new InitDataDictionaryBuilder(mainOutputUow);
                        var dataStoreSchemaExplorer = ((IDataStoreSchemaExplorer)((BaseDataLayer)mainInputUow.DataLayer).ConnectionProvider);
                        ApplicationStatusUpdater.Notify("Import", "Creating a dynamic dictionary...");
                        builder.InitDictionary(mainInputUow.Dictionary, dataStoreSchemaExplorer);
                        foreach (var persistentTypeInfo in builder.GetPersistentTypes()) {
                            Import(createOutputUow, builder, createInputUow, persistentTypeInfo);
                        }
                        ApplicationStatusUpdater.Notify("Import", "Commiting data...");
                        ClearObjectCache();
                    }
                }
            }

            void Import(Func<UnitOfWork> createOutputUow, InitDataDictionaryBuilder builder, Func<UnitOfWork> createInputUow, ITypeInfo persistentTypeInfo) {
                var classInfo = builder.FindClassInfo(persistentTypeInfo);
                UnitOfWork inputUow = createInputUow();
                UnitOfWork outputUow = createOutputUow();
                try {
                    var memberInfos = MemberInfos(builder, persistentTypeInfo);
                    var keys = InputObjectKeys(classInfo, inputUow);
                    int position = 0;
                    int commitAccumulator = 0;
                    int length = keys.Count;
                    while (position < length) {
                        int chunkSize = XpoDefault.GetTerminalInSize(length - position);
                        var keysToImport = keys.GetRange(position, chunkSize);
                        var objectsToImport = inputUow.GetObjectsByKey(classInfo, keysToImport, false);
                        position += chunkSize;
                        commitAccumulator += chunkSize;
                        PrefetchDelayedMembers(memberInfos, inputUow, classInfo, objectsToImport);
                        ImportMembers(builder, persistentTypeInfo, objectsToImport, keysToImport, outputUow, memberInfos, inputUow);
                        do {
                            var refsAndImports = RefsAndImports(outputUow);
                            foreach (KeyValuePair<IMemberInfo, FillRefList> pair in refsAndImports) {
                                IMemberInfo memberInfo = pair.Key;
                                FillRefList fillRefsList = pair.Value;
                                ITypeInfo typeInfo = XafTypesInfo.CastTypeToTypeInfo(memberInfo.IsList ? memberInfo.ListElementType : memberInfo.MemberType);
                                List<IMemberInfo> memberList = MemberInfos(builder, typeInfo);
                                var importClassInfo = builder.FindClassInfo(typeInfo);
                                var objects = inputUow.GetObjectsByKey(importClassInfo, fillRefsList.RefKeyList, false);
                                PrefetchDelayedMembers(memberInfos, inputUow, importClassInfo, objects);
                                ImportRefs(builder, typeInfo, objects, fillRefsList, outputUow, inputUow, memberList, memberInfo);
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
                            ApplicationStatusUpdater.Notify("Import", string.Format("Transforming records from {0}: {1}", persistentTypeInfo.Name, position));
                        }
                    }
                } finally {
                    ApplicationStatusUpdater.Notify("Import", string.Format("Transforming records from {0} ...", persistentTypeInfo.Name));
                    outputUow.CommitChanges();
                    GenerationNext(outputUow);
                    outputUow.Dispose();
                    inputUow.Dispose();
                }
            }

            void ImportRefs(InitDataDictionaryBuilder builder, ITypeInfo typeInfo, ICollection objects, FillRefList fillRefsList,
                            UnitOfWork outputUow, UnitOfWork inputUow, List<IMemberInfo> memberList, IMemberInfo memberInfo) {
                int counter = 0;
                var memberObjectDictionary = GetObjectDictionary(typeInfo.Type);
                foreach (object importObj in objects) {
                    object owner = fillRefsList.OwnerList[counter];
                    object refKey = fillRefsList.RefKeyList[counter];
                    counter++;
                    object obj;
                    if (!memberObjectDictionary.ContainsKey(refKey)) {
                        obj = ImportSingle(refKey, importObj, outputUow, builder, inputUow, typeInfo, memberList);
                    } else {
                        bool returnKey;
                        obj = GetCachedObject(memberObjectDictionary, outputUow, refKey, typeInfo.Type, true, out returnKey);
                    }
                    if (memberInfo.IsList) {
                        var list = memberInfo.GetValue(owner) as IList;
                        if (list == null) continue;
                        list.Add(obj);
                    } else {
                        memberInfo.SetValue(owner, obj);
                    }
                }
            }

            IEnumerable<KeyValuePair<IMemberInfo, FillRefList>> RefsAndImports(UnitOfWork outputUow) {
                foreach (KeyValuePair<IMemberInfo, FillRefList> pair in _fillRefsDictionary) {
                    IMemberInfo memberInfo = pair.Key;
                    FillRefList fillRefsList = pair.Value;
                    Type memberType = memberInfo.IsList ? memberInfo.ListElementType : memberInfo.MemberType;
                    var objects = outputUow.GetObjectsByKey(outputUow.GetClassInfo(memberType), fillRefsList.RefKeyList, false);
                    int counter = 0;
                    var memberObjectDictionary = GetObjectDictionary(memberType);
                    foreach (object obj in objects) {
                        object owner = fillRefsList.OwnerList[counter];
                        object refKey = fillRefsList.RefKeyList[counter];
                        counter++;
                        AddObjectToCache(memberObjectDictionary, refKey, obj);
                        if (memberInfo.IsList) {
                            var list = memberInfo.GetValue(owner) as IList;
                            if (list == null) continue;
                            list.Add(obj);
                        } else {
                            memberInfo.SetValue(owner, obj);
                        }
                    }
                }
                _fillRefsDictionary.Clear();
                List<KeyValuePair<IMemberInfo, FillRefList>> refsAndImports = _fillRefsAndImportDictionary.ToList();
                _fillRefsAndImportDictionary.Clear();
                return refsAndImports;
            }

            List<object> InputObjectKeys(XPClassInfo classInfo, UnitOfWork inputUow) {
                var operatorCollection = new CriteriaOperatorCollection{
                    new OperandProperty(classInfo.KeyProperty.Name)
                };
                return inputUow.SelectData(classInfo, operatorCollection, null, false, 0, new SortingCollection()).Select(r => r[0]).ToList();
            }

            void ImportMembers(InitDataDictionaryBuilder builder, ITypeInfo persistentTypeInfo, ICollection objectsToImport,
                               List<object> keysToImport, UnitOfWork outputUow, List<IMemberInfo> memberInfos, UnitOfWork inputUow) {
                int keysCounter = 0;
                var objectDictionary = GetObjectDictionary(persistentTypeInfo.Type);
                foreach (var objectToImport in objectsToImport) {
                    object objectKey = keysToImport[keysCounter++];
                    if (objectDictionary.ContainsKey(objectKey)) continue;
                    var xafObject = CreateInstance(persistentTypeInfo.Type, outputUow);
                    AddObjectToCache(objectDictionary, objectKey, xafObject);
                    foreach (var memberInfo in memberInfos) {
                        var importedMemberInfo = new ImportedMemberInfo(persistentTypeInfo, memberInfo);
                        if (importedMemberInfo.MemberInfo.IsList) {
                            ImportManyToManyCollection(outputUow, inputUow, xafObject, importedMemberInfo, objectToImport, memberInfo);
                        } else {
                            ImportSimpleProperty(outputUow, builder, inputUow, memberInfo, importedMemberInfo, objectToImport, xafObject);
                        }
                    }
                }
            }

            void PrefetchDelayedMembers(IEnumerable<IMemberInfo> memberInfos, UnitOfWork inputUow, XPClassInfo classInfo, ICollection objectsToImport) {
                foreach (var memberInfo in memberInfos) {
                    if (memberInfo.IsDelayed) {
                        inputUow.PreFetch(classInfo, objectsToImport, memberInfo.Name);
                    }
                }
            }

            object ImportSingle(object objectKeyToImport, object objectToImport, UnitOfWork outputUow, InitDataDictionaryBuilder builder, UnitOfWork inputUow, ITypeInfo persistentTypeInfo, IEnumerable<IMemberInfo> memberInfos) {
                var objectDictionary = GetObjectDictionary(persistentTypeInfo.Type);
                var xafObject = CreateInstance(persistentTypeInfo.Type, outputUow);
                AddObjectToCache(objectDictionary, objectKeyToImport, xafObject);
                foreach (var memberInfo in memberInfos) {
                    var importedMemberInfo = new ImportedMemberInfo(persistentTypeInfo, memberInfo);
                    if (importedMemberInfo.MemberInfo.IsList) {
                        ImportManyToManyCollection(outputUow, inputUow, xafObject, importedMemberInfo, objectToImport, memberInfo);
                    } else {
                        ImportSimpleProperty(outputUow, builder, inputUow, memberInfo, importedMemberInfo, objectToImport, xafObject);
                    }
                }
                return xafObject;
            }

            void ImportSimpleProperty(UnitOfWork outputUow, InitDataDictionaryBuilder builder, UnitOfWork inputUow,
                                             IMemberInfo memberInfo, ImportedMemberInfo importedMemberInfo, object objectToImport, object xafObject) {
                XPMemberInfo xpMemberInfo = builder.GetXpMemberInfo(inputUow.GetClassInfo(objectToImport), importedMemberInfo);
                if (xpMemberInfo != null) {
                    var value = xpMemberInfo.GetValue(objectToImport);
                    if (memberInfo.MemberTypeInfo.IsPersistent) {
                        var memberType = memberInfo.MemberType;
                        bool returnKey;
                        bool returnImportKey;
                        value = GetReferenceMemberValue(outputUow, value, memberType, out returnKey, out returnImportKey);
                        if (returnKey) {
                            AddFillRefInfo(memberInfo, xafObject, value);
                            return;
                        }
                        if (returnImportKey) {
                            AddFillRefAndImportInfo(memberInfo, xafObject, value);
                            return;
                        }
                    }
                    memberInfo.SetValue(xafObject, value);
                }
            }

            void ImportManyToManyCollection(UnitOfWork outputUow, UnitOfWork intputUow,
                                         object xafObject, ImportedMemberInfo importedMemberInfo, object objectToImport,
                                         IMemberInfo memberInfo) {
                if (!importedMemberInfo.InitialData.DataProvider)
                    return;
                var xpClassInfo = intputUow.Dictionary.GetClassInfo(null, importedMemberInfo.InitialData.DataProviderTableName);
                var criteriaOperator = CriteriaOperator.Parse(importedMemberInfo.InitialData.DataProviderQueryColumnName + "=?", objectToImport);
                using (var xpCollection = new XPCollection(intputUow, xpClassInfo, criteriaOperator)) {
                    var collection = (XPBaseCollection)memberInfo.GetValue(xafObject);
                    var dataProviderResultInfo = xpClassInfo.GetMember(importedMemberInfo.InitialData.DataProviderResultColumnName);
                    foreach (var o in xpCollection) {
                        var listElementTypeInfo = memberInfo.ListElementTypeInfo;
                        var memberValue = dataProviderResultInfo.GetValue(o);
                        bool returnKey;
                        bool returnImportKey;
                        var referenceMemberValue = GetReferenceMemberValue(outputUow, memberValue, listElementTypeInfo.Type, out returnKey, out returnImportKey);
                        if (returnKey) {
                            AddFillRefInfo(memberInfo, xafObject, referenceMemberValue);
                        } if (returnImportKey) {
                            AddFillRefAndImportInfo(memberInfo, xafObject, referenceMemberValue);
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

            readonly Dictionary<ITypeInfo, List<IMemberInfo>> membersToImportDict = new Dictionary<ITypeInfo, List<IMemberInfo>>();
            List<IMemberInfo> MemberInfos(InitDataDictionaryBuilder builder, ITypeInfo persistentTypeInfo) {
                List<IMemberInfo> result;
                if (!membersToImportDict.TryGetValue(persistentTypeInfo, out result)) {
                    result = persistentTypeInfo.Members.Where(info => builder.NeedsImport(new ImportedMemberInfo(persistentTypeInfo, info))).ToList();
                    membersToImportDict.Add(persistentTypeInfo, result);
                }
                return result;
            }
        }

        public static void Import(Func<UnitOfWork> createOutputUow, Func<UnitOfWork> createInputOuw) {
            var initDataImpl = new InitDataImpl();
            initDataImpl.ImportCore(createOutputUow, createInputOuw);
        }
    }

    internal class InitDataDictionaryBuilder {
        readonly UnitOfWork _unitOfWork;
        Dictionary<string, DBTable> _dbTables;
        IDataStoreSchemaExplorer _dataStoreSchemaExplorer;
        readonly Dictionary<string, XPClassInfo> _classInfos = new Dictionary<string, XPClassInfo>();

        public InitDataDictionaryBuilder(UnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public Dictionary<XPMemberInfo, IMemberInfo> InitDictionary(XPDictionary dictionary, IDataStoreSchemaExplorer dataStoreSchemaExplorer) {
            _dataStoreSchemaExplorer = dataStoreSchemaExplorer;
            _dbTables = new Dictionary<string, DBTable>();
            var persistentTypes = GetPersistentTypes();
            return CreateMemberInfos(dictionary, persistentTypes);
        }

        Dictionary<XPMemberInfo, IMemberInfo> CreateMemberInfos(XPDictionary dictionary, IEnumerable<ITypeInfo> persistentTypes) {
            var infos = new Dictionary<XPMemberInfo, IMemberInfo>();
            var keys = new Dictionary<string, XPClassInfo>();
            foreach (var importedMemberInfo in GetMembers(persistentTypes)) {
                CreateXPInfo(dictionary, keys, infos, importedMemberInfo);
            }
            CreateMissingKeys(keys, infos);
            return infos;
        }

        void CreateXPInfo(XPDictionary dictionary, Dictionary<string, XPClassInfo> keys, Dictionary<XPMemberInfo, IMemberInfo> infos, ImportedMemberInfo importedMemberInfo) {
            var owner = importedMemberInfo.RootTypeInfo;
            XPClassInfo classInfo = GetClassInfo(dictionary, owner);
            var memberInfo = importedMemberInfo.MemberInfo;
            if (memberInfo.IsList) {
                var importDataAttribute = importedMemberInfo.InitialData;
                var className = importDataAttribute.DataProviderTableName;
                if (importDataAttribute.DataProvider && dictionary.QueryClassInfo(null, className) == null) {
                    var info = dictionary.CreateClass(className);
                    info.CreateMember("Oid_" + importDataAttribute.DataProviderQueryColumnName, typeof(int)).AddAttribute(new KeyAttribute(true));
                    info.CreateMember(importDataAttribute.DataProviderQueryColumnName, importedMemberInfo.RootTypeInfo.KeyMember.MemberType);
                    info.CreateMember(importDataAttribute.DataProviderResultColumnName, memberInfo.ListElementTypeInfo.KeyMember.MemberType);
                }
                return;
            }
            var columnName = GetColumnName(classInfo, importedMemberInfo);
            if (columnName != null) {
                var member = CreateMember(keys, memberInfo, columnName, classInfo, infos, dictionary);
                infos.Add(member, memberInfo);
            }
        }

        string GetColumnName(XPClassInfo classInfo, ImportedMemberInfo importedMemberInfo) {
            string columnName = GetColumnName(importedMemberInfo);
            if (!ColumnExists(columnName, classInfo, importedMemberInfo)) {
                if (ThrowIfColumnNotFound(importedMemberInfo))
                    throw new NullReferenceException("Column " + columnName + " not found in " + classInfo.TableName);
                return null;
            }
            return columnName;
        }

        bool ThrowIfColumnNotFound(ImportedMemberInfo importedMemberInfo) {
            var importDataAttribute = importedMemberInfo.RootTypeInfo.FindAttribute<InitialDataAttribute>();
            return importDataAttribute != null && importDataAttribute.ThrowIfColumnNotFound;
        }

        XPCustomMemberInfo CreateMember(Dictionary<string, XPClassInfo> keys, IMemberInfo memberInfo, string columnName, XPClassInfo classInfo, Dictionary<XPMemberInfo, IMemberInfo> infos, XPDictionary dictionary) {
            var propertyType = memberInfo.MemberType;
            if (memberInfo.MemberTypeInfo.IsPersistent) {
                var memberInfos = CreateMemberInfos(dictionary, new[] { memberInfo.MemberTypeInfo });
                foreach (var info in memberInfos) {
                    infos.Add(info.Key, info.Value);
                }
                propertyType = memberInfo.MemberTypeInfo.KeyMember.MemberType;
            }


            var member = classInfo.CreateMember(columnName, propertyType);
            AddAttributes(member, memberInfo);
            AddKeys(keys, classInfo, memberInfo, member);
            return member;
        }

        void AddKeys(Dictionary<string, XPClassInfo> keys, XPClassInfo classInfo, IMemberInfo memberInfo, XPCustomMemberInfo member) {
            if (memberInfo.IsKey) {
                member.AddAttribute(new KeyAttribute(memberInfo.FindAttribute<KeyAttribute>().AutoGenerate));
                keys.Add(classInfo.TableName, classInfo);
            }
        }

        void AddAttributes(XPCustomMemberInfo member, IMemberInfo memberInfo) {
            foreach (var attribute in memberInfo.FindAttributes<Attribute>()) {
                member.AddAttribute(attribute);
            }
        }

        void CreateMissingKeys(Dictionary<string, XPClassInfo> keys, Dictionary<XPMemberInfo, IMemberInfo> infos) {
            var classInfos = infos.Select(pair => pair.Key.Owner).Distinct();
            var infosWithoutKeys = classInfos.Where(info => !keys.ContainsKey(info.TableName) && info.KeyProperty == null);
            var keyMembers = infosWithoutKeys.Select(classInfo => classInfo.CreateMember(typeof(InitDataExtensions).Name + "Oid", typeof(int)));
            foreach (var member in keyMembers) {
                member.AddAttribute(new KeyAttribute(true));
            }
        }

        bool ColumnExists(string columnName, XPClassInfo classInfo, ImportedMemberInfo importedMemberInfo) {
            var autoMode = AutoMode(importedMemberInfo);
            bool found = _dbTables[classInfo.TableName].Columns.Any(column => column.Name == columnName);
            return (found || autoMode) && found;
        }

        bool IsServiceField(IMemberInfo memberInfo) {
            return memberInfo.Name == GCRecordField.StaticName || memberInfo.Name == "OptimisticLockField";
        }

        IEnumerable<ImportedMemberInfo> GetMembers(IEnumerable<ITypeInfo> persistentTypes) {
            var members = persistentTypes.Select(typeInfo => new { typeInfo, typeInfo.Members });
            var importedMemberInfos = members.SelectMany(member => member.Members, (member, info) => new ImportedMemberInfo(member.typeInfo, info));
            return importedMemberInfos.Where(NeedsImport).ToList();
        }

        public IList<ITypeInfo> GetPersistentTypes() {
            return XafTypesInfo.Instance.PersistentTypes.Where(info => IsImportedType(info) && IsNotAlreadyImported(info)).ToList();
        }

        string GetColumnName(ImportedMemberInfo importedMemberInfo) {
            var memberInfo = importedMemberInfo.MemberInfo;
            var rootTypeInfo = importedMemberInfo.RootTypeInfo;
            var importFromAttribute = memberInfo.FindAttribute<InitialDataAttribute>();
            if (importFromAttribute == null) {
                var attribute = rootTypeInfo.FindAttribute<InitialDataAttribute>();
                if (attribute != null) {
                    if (attribute.BaseMembers == null)
                        return memberInfo.Name;
                    var firstOrDefault = attribute.BaseMembers.Split(',').FirstOrDefault(s => s == memberInfo.Name || s.Split('|')[0] == memberInfo.Name);
                    if (!string.IsNullOrEmpty(firstOrDefault)) {
                        return firstOrDefault.IndexOf("|", StringComparison.Ordinal) > -1 ? firstOrDefault.Split('|')[1] : firstOrDefault;
                    }
                    if (attribute.AllOwnMembers)
                        return memberInfo.Name;
                    if (IsBaseProperty(importedMemberInfo))
                        return memberInfo.Name;
                }
                throw new NotImplementedException(memberInfo.Name);
            }
            return !string.IsNullOrEmpty(importFromAttribute.Name) ? importFromAttribute.Name : memberInfo.Name;
        }

        bool IsNotAlreadyImported(ITypeInfo info) {
            return _unitOfWork.FindObject(info.Type, null) == null;
        }

        XPClassInfo GetClassInfo(XPDictionary dictionary, ITypeInfo typeInfo) {
            var className = GetTableName(typeInfo);
            if (!_dbTables.ContainsKey(className)) {
                var dbTable = _dataStoreSchemaExplorer.GetStorageTables(className).Single();
                _dbTables.Add(className, dbTable);
                var attributes = new Attribute[] { new OptimisticLockingAttribute(false), new DeferredDeletionAttribute(false) };
                var classInfo = dictionary.CreateClass(className, attributes);
                _classInfos.Add(classInfo.TableName, classInfo);
                return classInfo;
            }
            return dictionary.QueryClassInfo(null, className);
        }

        readonly Dictionary<ITypeInfo, string> tableNameDict = new Dictionary<ITypeInfo, string>();
        string GetTableName(ITypeInfo typeInfo) {
            string tableName;
            if (!tableNameDict.TryGetValue(typeInfo, out tableName)) {
                var importFromAttribute = typeInfo.FindAttribute<InitialDataAttribute>();
                tableName = importFromAttribute != null && !string.IsNullOrEmpty(importFromAttribute.Name) ? importFromAttribute.Name : typeInfo.Name;
                tableNameDict.Add(typeInfo, tableName);
            }
            return tableName;
        }
        bool IsImportedType(ITypeInfo info) {
            if (info.IsPersistent) {
                var initDataAttribute = info.FindAttribute<InitialDataAttribute>();
                return (initDataAttribute != null && initDataAttribute.AllOwnMembers) || info.Members.Any(memberInfo => NeedsImport(new ImportedMemberInfo(info, memberInfo)));
            }
            return false;
        }

        public bool NeedsImport(ImportedMemberInfo importedMemberInfo) {
            if (!importedMemberInfo.MemberInfo.IsList && !importedMemberInfo.MemberInfo.IsPersistent) {
                return false;
            }
            return (AutoMode(importedMemberInfo) || IsBaseProperty(importedMemberInfo) || importedMemberInfo.MemberInfo.FindAttribute<InitialDataAttribute>() != null);
        }

        bool IsBaseProperty(ImportedMemberInfo importedMemberInfo) {
            if (importedMemberInfo.RootTypeInfo.OwnMembers.Any(info => info.Name == importedMemberInfo.MemberInfo.Name))
                return false;
            var initDataAttribute = importedMemberInfo.RootTypeInfo.FindAttribute<InitialDataAttribute>();
            return initDataAttribute != null && (initDataAttribute.BaseMembers + "").Split(',').Any(s => s == importedMemberInfo.MemberInfo.Name || s.Split('|')[0] == importedMemberInfo.MemberInfo.Name);
        }

        bool AutoMode(ImportedMemberInfo importedMemberInfo) {
            var memberInfo = importedMemberInfo.MemberInfo;
            var initDataAttribute = memberInfo.Owner.FindAttribute<InitialDataAttribute>();
            return (initDataAttribute != null && initDataAttribute.AllOwnMembers) && memberInfo.IsPersistent && !IsServiceField(memberInfo);
        }

        public XPClassInfo FindClassInfo(ITypeInfo typeInfo) {
            var tableName = GetTableName(typeInfo);
            return _classInfos[tableName];
        }

        public XPMemberInfo GetXpMemberInfo(XPClassInfo classInfo, ImportedMemberInfo importedMemberInfo) {
            var columnName = GetColumnName(importedMemberInfo);
            return !ThrowIfColumnNotFound(importedMemberInfo) ? classInfo.FindMember(columnName) : classInfo.GetMember(columnName);
        }
    }

    internal class ImportedMemberInfo {
        readonly ITypeInfo _rootTypeInfo;
        readonly IMemberInfo _memberInfo;

        public ImportedMemberInfo(ITypeInfo rootTypeInfo, IMemberInfo memberInfo) {
            _rootTypeInfo = rootTypeInfo;
            _memberInfo = memberInfo;
        }

        public IMemberInfo MemberInfo {
            get { return _memberInfo; }
        }

        public InitialDataAttribute InitialData {
            get { return _memberInfo.FindAttribute<InitialDataAttribute>(); }
        }

        public ITypeInfo RootTypeInfo {
            get { return _rootTypeInfo; }
        }
        public override string ToString() {
            return String.Format("{0},{1}", _rootTypeInfo.Name, _memberInfo.Name);
        }
    }

}
