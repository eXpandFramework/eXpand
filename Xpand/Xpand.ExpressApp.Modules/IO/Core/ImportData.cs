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
            readonly Dictionary<KeyValuePair<Type, object>, object> _importedObjecs = new Dictionary<KeyValuePair<Type, object>, object>();
            readonly HashSet<XPClassInfo> _importedClassInfos = new HashSet<XPClassInfo>();
            string _lastStatusMessageTable;


            public void ImportCore(UnitOfWork uow, UnitOfWork unitOfWork) {
                var builder = new InitDataDictionaryBuilder(uow);
                var dataStoreSchemaExplorer = ((IDataStoreSchemaExplorer)((BaseDataLayer)unitOfWork.DataLayer).ConnectionProvider);
                ApplicationStatusUpdater.Notify("Import", "Creating dynamic dictionary...");
                builder.InitDictionary(unitOfWork.Dictionary, dataStoreSchemaExplorer);
                foreach (var persistentTypeInfo in builder.GetPersistentTypes()) {
                    Import(uow, builder, unitOfWork, persistentTypeInfo);
                }
                _importedObjecs.Clear();
                _importedClassInfos.Clear();
                ApplicationStatusUpdater.Notify("Import", "Commiting...pleasing wait");
                uow.CommitChanges();
            }

            public void ImportCore(UnitOfWork uow, string connectionString) {
                using (var unitOfWork = new UnitOfWork { ConnectionString = connectionString }) {
                    ImportCore(uow, unitOfWork);
                }
            }

            void Import(UnitOfWork uow, InitDataDictionaryBuilder builder, UnitOfWork unitOfWork, ITypeInfo persistentTypeInfo) {
                var classInfo = builder.FindClassInfo(unitOfWork, persistentTypeInfo);
                if (!_importedClassInfos.Contains(classInfo)) {
                    _importedClassInfos.Add(classInfo);

                    using (var objectsToImport = new XPCollection(unitOfWork, classInfo)) {
                        var memberInfos = MemberInfos(builder, persistentTypeInfo);
                        foreach (var objectToImport in objectsToImport) {
                            var xafObject = CreateObject(uow, persistentTypeInfo);
                            var keyValuePair = KeyValuePair(unitOfWork.GetKeyValue(objectToImport), persistentTypeInfo.Type);
                            _importedObjecs.Add(keyValuePair, xafObject);
                            foreach (var memberInfo in memberInfos) {
                                var importedMemberInfo = new ImportedMemberInfo(persistentTypeInfo, memberInfo);
                                if (importedMemberInfo.MemberInfo.IsList) {
                                    ImportManyToManyCollection(uow, builder, unitOfWork, xafObject, importedMemberInfo, objectToImport, memberInfo);
                                } else {
                                    ImportSimpleProperty(uow, builder, unitOfWork, memberInfo, importedMemberInfo, objectToImport, xafObject);
                                }


                                if (_lastStatusMessageTable != classInfo.TableName) {
                                    _lastStatusMessageTable = classInfo.TableName;
                                    var statusMessage = string.Format("Importing {2} objects from {0} into {1}", classInfo.TableName, persistentTypeInfo.Name, objectsToImport.Count);
                                    ApplicationStatusUpdater.Notify("Import", statusMessage);
                                }
                            }
                        }
                    }
                }
            }

            object CreateObject(UnitOfWork unitOfWork, ITypeInfo persistentTypeInfo) {
                return Activator.CreateInstance(persistentTypeInfo.Type, unitOfWork);
            }

            void ImportSimpleProperty(UnitOfWork uow, InitDataDictionaryBuilder builder, UnitOfWork unitOfWork,
                                             IMemberInfo memberInfo, ImportedMemberInfo importedMemberInfo, object objectToImport, object xafObject) {
                XPMemberInfo xpMemberInfo = builder.GetXpMemberInfo(unitOfWork.GetClassInfo(objectToImport), importedMemberInfo);
                if (xpMemberInfo != null) {
                    var value = xpMemberInfo.GetValue(objectToImport);
                    if (memberInfo.MemberTypeInfo.IsPersistent) {
                        var memberType = memberInfo.MemberType;
                        value = GetReferenceMemberValue(uow, builder, unitOfWork, value, memberType);
                    }
                    memberInfo.SetValue(xafObject, value);
                }
            }

            void ImportManyToManyCollection(UnitOfWork objectSpace, InitDataDictionaryBuilder builder, UnitOfWork unitOfWork,
                                         object xafObject, ImportedMemberInfo importedMemberInfo, object objectToImport,
                                         IMemberInfo memberInfo) {
                if (!importedMemberInfo.InitialData.DataProvider)
                    return;
                var xpClassInfo = unitOfWork.Dictionary.GetClassInfo(null, importedMemberInfo.InitialData.DataProviderTableName);
                var keyValue = unitOfWork.GetKeyValue(objectToImport);
                var criteriaOperator = CriteriaOperator.Parse(importedMemberInfo.InitialData.DataProviderQueryColumnName + "=?", keyValue);
                using (var xpCollection = new XPCollection(unitOfWork, xpClassInfo, criteriaOperator)) {
                    var collection = (XPBaseCollection)memberInfo.GetValue(xafObject);
                    var dataProviderResultInfo = xpClassInfo.GetMember(importedMemberInfo.InitialData.DataProviderResultColumnName);
                    foreach (var o in xpCollection) {
                        var listElementTypeInfo = memberInfo.ListElementTypeInfo;
                        var memberValue = dataProviderResultInfo.GetValue(o);
                        var referenceMemberValue = GetReferenceMemberValue(objectSpace, builder, unitOfWork, memberValue, listElementTypeInfo.Type);
                        collection.BaseAdd(referenceMemberValue);
                    }
                }
            }

            object GetReferenceMemberValue(UnitOfWork uow, InitDataDictionaryBuilder builder, UnitOfWork unitOfWork, object memberValue, Type memberType) {
                if (memberValue != null) {
                    var keyValuePair = KeyValuePair(memberValue, memberType);
                    if (!_importedObjecs.ContainsKey(keyValuePair)) {
                        Import(uow, builder, unitOfWork, XafTypesInfo.CastTypeToTypeInfo(memberType));
                    }
                    object value = _importedObjecs[keyValuePair];
                    Guard.ArgumentNotNull(value, "value");
                    return value;
                }
                return null;
            }

            KeyValuePair<Type, object> KeyValuePair(object keyValue, Type objectType) {
                return new KeyValuePair<Type, object>(objectType, keyValue);
            }

            IList<IMemberInfo> MemberInfos(InitDataDictionaryBuilder builder, ITypeInfo persistentTypeInfo) {
                return persistentTypeInfo.Members.Where(info => builder.NeedsImport(new ImportedMemberInfo(persistentTypeInfo, info))).ToList();
            }
        }

        public static void Import(this UnitOfWork uow, UnitOfWork unitOfWork) {
            var initDataImpl = new InitDataImpl();
            initDataImpl.ImportCore(uow, unitOfWork);
        }

        public static void Import(this UnitOfWork uow, string connectionString) {
            var initDataImpl = new InitDataImpl();
            initDataImpl.ImportCore(uow, connectionString);
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
