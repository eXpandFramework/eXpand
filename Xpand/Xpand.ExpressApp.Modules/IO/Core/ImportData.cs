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
        class InitDataImpl {
            readonly Dictionary<KeyValuePair<Type, CriteriaOperator>, object> importedObjecs = new Dictionary<KeyValuePair<Type, CriteriaOperator>, object>();

            public void ImportCore(IObjectSpace objectSpace, UnitOfWork unitOfWork) {
                var builder = new InitDataDictionaryBuilder(objectSpace);
                var dataStoreSchemaExplorer = ((IDataStoreSchemaExplorer)((BaseDataLayer)unitOfWork.DataLayer).ConnectionProvider);
                builder.InitDictionary(unitOfWork.Dictionary, dataStoreSchemaExplorer);
                foreach (var persistentTypeInfo in builder.GetPersistentTypes()) {
                    Import(objectSpace, builder, unitOfWork, persistentTypeInfo);
                }
                importedObjecs.Clear();
                unitOfWork.CommitChanges();
            }

            public void ImportCore(IObjectSpace objectSpace, string connectionString) {
                using (var unitOfWork = new UnitOfWork { ConnectionString = connectionString }) {
                    ImportCore(objectSpace, unitOfWork);
                }
            }

            void Import(IObjectSpace objectSpace, InitDataDictionaryBuilder builder, UnitOfWork unitOfWork, ITypeInfo persistentTypeInfo) {
                var classInfo = builder.FindClassInfo(unitOfWork, persistentTypeInfo);
                var objectsToImport = new XPCollection(unitOfWork, classInfo);
                var memberInfos = MemberInfos(builder, persistentTypeInfo);
                foreach (var objectToImport in objectsToImport) {
                    var xafObject = CreateObject(objectSpace, persistentTypeInfo);
                    foreach (var memberInfo in memberInfos) {
                        var importedMemberInfo = new ImportedMemberInfo(persistentTypeInfo, memberInfo);
                        if (importedMemberInfo.MemberInfo.IsList) {
                            ImportManyToManyCollection(objectSpace, builder, unitOfWork, xafObject, importedMemberInfo, objectToImport, memberInfo);
                        } else {
                            ImportSimpleProperty(objectSpace, builder, unitOfWork, memberInfo, importedMemberInfo, objectToImport, xafObject);
                        }
                    }
                }
            }

            object CreateObject(IObjectSpace objectSpace, ITypeInfo persistentTypeInfo) {
                return objectSpace.CreateObject(persistentTypeInfo.Type);
            }

            void ImportSimpleProperty(IObjectSpace objectSpace, InitDataDictionaryBuilder builder, UnitOfWork unitOfWork,
                                             IMemberInfo memberInfo, ImportedMemberInfo importedMemberInfo, object objectToImport, object xafObject) {
                XPMemberInfo xpMemberInfo = builder.GetXpMemberInfo(unitOfWork.GetClassInfo(objectToImport), importedMemberInfo);
                if (xpMemberInfo != null) {
                    var value = xpMemberInfo.GetValue(objectToImport);
                    if (memberInfo.MemberTypeInfo.IsPersistent) {
                        var operandProperty = memberInfo.Owner.KeyMember.Name;
                        var memberType = memberInfo.MemberType;
                        value = GetReferenceMemberValue(objectSpace, builder, unitOfWork, value, operandProperty, memberType);
                    }
                    memberInfo.SetValue(xafObject, value);
                }
            }

            void ImportManyToManyCollection(IObjectSpace objectSpace, InitDataDictionaryBuilder builder, UnitOfWork unitOfWork,
                                         object xafObject, ImportedMemberInfo importedMemberInfo, object objectToImport,
                                         IMemberInfo memberInfo) {
                if (!importedMemberInfo.InitialData.DataProvider)
                    return;
                var xpClassInfo = unitOfWork.Dictionary.GetClassInfo(null, importedMemberInfo.InitialData.DataProviderTableName);
                var keyValue = unitOfWork.GetKeyValue(objectToImport);
                var criteriaOperator = CriteriaOperator.Parse(importedMemberInfo.InitialData.DataProviderQueryColumnName + "=?", keyValue);
                var xpCollection = new XPCollection(unitOfWork, xpClassInfo, criteriaOperator);
                var collection = (XPBaseCollection)memberInfo.GetValue(xafObject);
                var dataProviderResultInfo = xpClassInfo.GetMember(importedMemberInfo.InitialData.DataProviderResultColumnName);
                foreach (var o in xpCollection) {
                    var listElementTypeInfo = memberInfo.ListElementTypeInfo;
                    var memberValue = dataProviderResultInfo.GetValue(o);
                    var referenceMemberValue = GetReferenceMemberValue(objectSpace, builder, unitOfWork, memberValue, listElementTypeInfo.KeyMember.Name, listElementTypeInfo.Type);
                    collection.BaseAdd(referenceMemberValue);
                }
            }

            object GetReferenceMemberValue(IObjectSpace objectSpace, InitDataDictionaryBuilder builder, UnitOfWork unitOfWork, object memberValue, string operandProperty, Type memberType) {
                if (memberValue != null) {
                    object value = FindObject(objectSpace, memberValue, operandProperty, memberType);
                    if (value == null) {
                        Import(objectSpace, builder, unitOfWork, XafTypesInfo.CastTypeToTypeInfo(memberType));
                        value = FindObject(objectSpace, memberValue, operandProperty, memberType);
                        Guard.ArgumentNotNull(value, "value");
                    }
                    return value;
                }
                return null;
            }

            object FindObject(IObjectSpace objectSpace, object memberValue, string operandProperty, Type objectType) {
                var criteriaOperator = CriteriaOperator.Parse(operandProperty + "=?", memberValue);
                var keyValuePair = new KeyValuePair<Type, CriteriaOperator>(objectType, criteriaOperator);
                if (!importedObjecs.ContainsKey(keyValuePair)) {
                    var findObject = objectSpace.FindObject(objectType, criteriaOperator, true);
                    importedObjecs.Add(keyValuePair, findObject);
                } else if (importedObjecs[keyValuePair] == null)
                    importedObjecs[keyValuePair] = objectSpace.FindObject(objectType, criteriaOperator, true);
                return importedObjecs[keyValuePair];
            }


            IEnumerable<IMemberInfo> MemberInfos(InitDataDictionaryBuilder builder, ITypeInfo persistentTypeInfo) {
                return persistentTypeInfo.Members.Where(info => builder.NeedsImport(new ImportedMemberInfo(persistentTypeInfo, info)));
            }
        }
        public static void ImportFromSQLServer(this IObjectSpace objectSpace, string database) {

            objectSpace.Import(string.Format("Integrated Security=SSPI;Pooling=false;Data Source=(local);Initial Catalog={0}", database));
        }

        public static void ImportFromAccess(this IObjectSpace objectSpace, string path) {
            objectSpace.Import(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Password=;User ID=Admin;Data Source='{0}';Mode=Share Deny None;", path));
        }

        public static void Import(this IObjectSpace objectSpace, UnitOfWork unitOfWork) {
            var initDataImpl = new InitDataImpl();
            initDataImpl.ImportCore(objectSpace, unitOfWork);
        }

        public static void Import(this IObjectSpace objectSpace, string connectionString) {
            var initDataImpl = new InitDataImpl();
            initDataImpl.ImportCore(objectSpace, connectionString);
        }
    }

    internal class InitDataDictionaryBuilder {
        readonly IObjectSpace _objectSpace;
        Dictionary<string, DBTable> _dbTables;
        IDataStoreSchemaExplorer _dataStoreSchemaExplorer;

        public InitDataDictionaryBuilder(IObjectSpace objectSpace) {
            _objectSpace = objectSpace;
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
            return importedMemberInfos.Where(NeedsImport);
        }

        public IEnumerable<ITypeInfo> GetPersistentTypes() {
            return XafTypesInfo.Instance.PersistentTypes.Where(info => IsImportedType(info) && IsNotAlreadyImported(info));
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
            return _objectSpace.FindObject(info.Type, null) == null;
        }

        XPClassInfo GetClassInfo(XPDictionary dictionary, ITypeInfo typeInfo) {
            var className = GetTableName(typeInfo);
            if (!_dbTables.ContainsKey(className)) {
                var dbTable = _dataStoreSchemaExplorer.GetStorageTables(className).Single();
                _dbTables.Add(className, dbTable);
                var attributes = new Attribute[] { new OptimisticLockingAttribute(false), new DeferredDeletionAttribute(false) };
                return dictionary.CreateClass(className, attributes);
            }
            return dictionary.QueryClassInfo(null, className);
        }

        string GetTableName(ITypeInfo typeInfo) {
            var importFromAttribute = typeInfo.FindAttribute<InitialDataAttribute>();
            return importFromAttribute != null && !string.IsNullOrEmpty(importFromAttribute.Name) ? importFromAttribute.Name : typeInfo.Name;
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

        public XPClassInfo FindClassInfo(UnitOfWork unitOfWork, ITypeInfo typeInfo) {
            var tableName = GetTableName(typeInfo);
            var classInfo = unitOfWork.Dictionary.Classes.OfType<XPDataObjectClassInfo>().SingleOrDefault(info => info.TableName == tableName);
            if (classInfo == null)
                throw new ArgumentNullException("Table " + tableName + " not found");
            return classInfo;
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
