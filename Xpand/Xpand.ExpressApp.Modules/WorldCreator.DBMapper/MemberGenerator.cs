using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    public struct MemberGeneratorInfo {
        readonly IPersistentMemberInfo _persistentMemberInfo;
        readonly DBColumn _dbColumn;

        public MemberGeneratorInfo(IPersistentMemberInfo persistentMemberInfo, DBColumn dbColumn)
            : this() {
            _persistentMemberInfo = persistentMemberInfo;
            _dbColumn = dbColumn;
        }

        public IPersistentMemberInfo PersistentMemberInfo {
            get { return _persistentMemberInfo; }
        }

        public DBColumn DbColumn {
            get { return _dbColumn; }
        }
    }
    public class MemberGenerator {
        readonly DBTable _dbTable;
        readonly Dictionary<string, ClassGeneratorInfo> _classInfos;
        readonly IObjectSpace _objectSpace;

        public MemberGenerator(DBTable dbTable, Dictionary<string, ClassGeneratorInfo> classInfos) {
            _dbTable = dbTable;
            _classInfos = classInfos;
            _objectSpace = ObjectSpace.FindObjectSpaceByObject(classInfos[classInfos.Keys.First()].PersistentClassInfo);
        }

        public IEnumerable<MemberGeneratorInfo> Create() {
            var generatorInfos = CreateMembersCore(GetNonCompoundColumns());
            var coumboundPkColumn = GetCoumboundPKColumn();
            if (coumboundPkColumn != null) {
                var persistentClassInfo = _classInfos[ClassGenerator.GetTableName(_dbTable.Name) + ClassGenerator.KeyStruct].PersistentClassInfo;
                if (persistentClassInfo == null) throw new ArgumentNullException("persistentClassInfo with name " + _dbTable.Name + ClassGenerator.KeyStruct);
                var pkDbColumns = _dbTable.Columns.Where(column => coumboundPkColumn.Columns.Contains(column.Name));
                var membersCore = CreateMembersCore(pkDbColumns, persistentClassInfo, TemplateType.ReadWriteMember, TemplateType.ReadWriteMember).ToList();
                var persistentReferenceMemberInfo = CreatePersistentReferenceMemberInfo(persistentClassInfo.Name, _classInfos[ClassGenerator.GetTableName(_dbTable.Name)].PersistentClassInfo, persistentClassInfo, TemplateType.FieldMember);
                membersCore.Add(new MemberGeneratorInfo(persistentReferenceMemberInfo, _dbTable.GetColumn(coumboundPkColumn.Columns[0])));
                return generatorInfos.Union(membersCore);
            }
            return generatorInfos;
        }


        IEnumerable<DBColumn> GetNonCompoundColumns() {
            var columns = _dbTable.PrimaryKey.Columns;
            return columns.Count > 1 ? _dbTable.Columns.Where(column => !columns.Contains(column.Name)) : _dbTable.Columns;
        }

        IEnumerable<MemberGeneratorInfo> CreateMembersCore(IEnumerable<DBColumn> dbColumns, IPersistentClassInfo persistentClassInfo = null, TemplateType coreTemplateType = TemplateType.XPReadWritePropertyMember, TemplateType refTemplateType = TemplateType.XPReadWritePropertyMember) {
            return dbColumns.Select(dbColumn => CreateMember(dbColumn, persistentClassInfo, coreTemplateType, refTemplateType)).Where(info => info.PersistentMemberInfo != null);
        }

        MemberGeneratorInfo CreateMember(DBColumn dbColumn, IPersistentClassInfo persistentClassInfo = null, TemplateType coreTemplateType = TemplateType.XPReadWritePropertyMember, TemplateType refTemplateType = TemplateType.XPReadWritePropertyMember) {
            return CreateMemberCore(dbColumn, persistentClassInfo, coreTemplateType, refTemplateType);
        }


        MemberGeneratorInfo CreateMemberCore(DBColumn dbColumn, IPersistentClassInfo persistentClassInfo,
                                               TemplateType coreTemplateType, TemplateType refTemplateType) {
            if (!IsFKColumn(dbColumn) && (IsCoreColumn(dbColumn) || IsPrimaryKey(dbColumn))) {
                return new MemberGeneratorInfo(CreatePersistentCoreTypeMemberInfo(dbColumn, persistentClassInfo, coreTemplateType), dbColumn);
            }
            if (IsFKColumn(dbColumn)) {
                var dbForeignKeys = _dbTable.ForeignKeys.Where(key => key.Columns.Contains(dbColumn.Name));
                var foreignKey = dbForeignKeys.FirstOrDefault(key => key.PrimaryKeyTableKeyColumns.Count == 1);
                if (foreignKey != null) {
                    var primaryKeyTable = foreignKey.PrimaryKeyTable;
                    var tableName = ClassGenerator.GetTableName(primaryKeyTable);
                    if (!_classInfos.ContainsKey(tableName))
                        Debug.Print("");

                    var classGeneratorInfo = _classInfos[tableName];
                    return new MemberGeneratorInfo(CreatePersistentReferenceMemberInfo(dbColumn.Name, persistentClassInfo, classGeneratorInfo.PersistentClassInfo, refTemplateType), dbColumn);
                }
                return new MemberGeneratorInfo(null, dbColumn);
            }
            throw new NotImplementedException(dbColumn.Name);
        }

        bool IsPrimaryKey(DBColumn dbColumn) {
            return _dbTable.PrimaryKey.Columns.Contains(dbColumn.Name);
        }

        bool IsFKColumn(DBColumn dbColumn) {
            return _dbTable.ForeignKeys.Where(key => key.Columns.Contains(dbColumn.Name)).FirstOrDefault() != null;
        }

        static bool IsCoreColumn(DBColumn dbColumn) {
            return !dbColumn.IsIdentity && !dbColumn.IsKey;
        }

        DBPrimaryKey GetCoumboundPKColumn() {
            return _dbTable.PrimaryKey.Columns.Count > 1 ? _dbTable.PrimaryKey : null;
        }

        IPersistentMemberInfo CreatePersistentReferenceMemberInfo(string name, IPersistentClassInfo persistentClassInfo, IPersistentClassInfo persistentReferenceClassInfo, TemplateType templateType) {
            var persistentReferenceMemberInfo = _objectSpace.CreateWCObject<IPersistentReferenceMemberInfo>();
            persistentReferenceMemberInfo.Name = name;
            persistentReferenceMemberInfo.ReferenceClassInfo = persistentReferenceClassInfo;
            if (persistentClassInfo == null)
                persistentClassInfo = _classInfos[ClassGenerator.GetTableName(_dbTable.Name)].PersistentClassInfo;
            persistentClassInfo.OwnMembers.Add(persistentReferenceMemberInfo);
            persistentReferenceMemberInfo.SetDefaultTemplate(templateType);
            if (persistentReferenceClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.Class)
                CreateCollection(persistentReferenceMemberInfo, persistentClassInfo);
            return persistentReferenceMemberInfo;
        }

        void CreateCollection(IPersistentReferenceMemberInfo persistentReferenceMemberInfo, IPersistentClassInfo owner) {
            var persistentCollectionMemberInfo = _objectSpace.CreateWCObject<IPersistentCollectionMemberInfo>();
            persistentReferenceMemberInfo.ReferenceClassInfo.OwnMembers.Add(persistentCollectionMemberInfo);
            persistentCollectionMemberInfo.Name = persistentReferenceMemberInfo.Owner.Name + persistentReferenceMemberInfo.Name + "s";
            persistentCollectionMemberInfo.Owner = persistentReferenceMemberInfo.ReferenceClassInfo;
            var tableName = ClassGenerator.GetTableName(Regex.Replace(owner.Name, ClassGenerator.KeyStruct, "", RegexOptions.Singleline | RegexOptions.IgnoreCase));
            persistentCollectionMemberInfo.CollectionClassInfo = owner.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.Struct ? _classInfos[tableName].PersistentClassInfo : owner;
            persistentCollectionMemberInfo.SetDefaultTemplate(TemplateType.XPCollectionMember);
            var persistentAssociationAttribute = _objectSpace.CreateWCObject<IPersistentAssociationAttribute>();
            persistentCollectionMemberInfo.TypeAttributes.Add(persistentAssociationAttribute);
            persistentAssociationAttribute.AssociationName = String.Format("{0}-{1}s", persistentReferenceMemberInfo.Name, persistentCollectionMemberInfo.Name);
        }

        IPersistentCoreTypeMemberInfo CreatePersistentCoreTypeMemberInfo(DBColumn column, IPersistentClassInfo persistentClassInfo, TemplateType templateType) {
            var persistentCoreTypeMemberInfo = _objectSpace.CreateWCObject<IPersistentCoreTypeMemberInfo>();
            persistentCoreTypeMemberInfo.Name = column.Name;
            persistentCoreTypeMemberInfo.DataType = column.ColumnType;
            if (persistentClassInfo == null) {
                var tableName = ClassGenerator.GetTableName(_dbTable.Name);
                persistentClassInfo = _classInfos[tableName].PersistentClassInfo;
            }
            persistentClassInfo.OwnMembers.Add(persistentCoreTypeMemberInfo);
            persistentCoreTypeMemberInfo.SetDefaultTemplate(templateType);
            return persistentCoreTypeMemberInfo;
        }

    }
}