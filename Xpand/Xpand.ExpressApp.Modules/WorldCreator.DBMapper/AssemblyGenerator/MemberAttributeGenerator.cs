using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.DB;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.DBMapper.AssemblyGenerator {
    public class MemberAttributeGenerator {

        readonly IPersistentMemberInfo _persistentMemberInfo;
        readonly DBColumn _column;
        readonly bool _isPrimaryKey;
        readonly IObjectSpace _objectSpace;
        readonly DBTable _dbTable;


        public MemberAttributeGenerator(MemberGeneratorInfo memberGeneratorInfo, ClassGeneratorInfo classGeneratorInfo) {
            _objectSpace = XPObjectSpace.FindObjectSpaceByObject(memberGeneratorInfo.PersistentMemberInfo);
            _persistentMemberInfo = memberGeneratorInfo.PersistentMemberInfo;
            _column = memberGeneratorInfo.DbColumn;
            _isPrimaryKey = CalculatePrimaryKey(memberGeneratorInfo, classGeneratorInfo);
            _dbTable = classGeneratorInfo.DbTable;
        }

        bool CalculatePrimaryKey(MemberGeneratorInfo memberGeneratorInfo, ClassGeneratorInfo classGeneratorInfo) {
            if (classGeneratorInfo.DbTable.PrimaryKey != null && memberGeneratorInfo.DbColumn != null)
                return classGeneratorInfo.DbTable.PrimaryKey.Columns.Contains(memberGeneratorInfo.DbColumn.Name);
            return false;
        }

        public void Create() {
            if (IsOneToOne())
                return;
            var isCompoundPrimaryKey = IsCompoundPrimaryKey();
            var persistentAttributeInfos = _persistentMemberInfo.TypeAttributes;
            if (!isCompoundPrimaryKey && _column.ColumnType == DBColumnType.String) {
                persistentAttributeInfos.Add(GetPersistentSizeAttribute());
            }
            if (_persistentMemberInfo.Owner.CodeTemplateInfo.CodeTemplate.TemplateType != TemplateType.Struct) {
                if (_isPrimaryKey) {
                    persistentAttributeInfos.Add(GetPersistentKeyAttribute());
                    if (!isCompoundPrimaryKey) {
                        persistentAttributeInfos.Add(GetRuleRequiredAttribute());
                    }
                }
                if ((!IsSelfRefOnTheKey(_column, _isPrimaryKey)) && ((IsSimpleForeignKey() && !isCompoundPrimaryKey) || ((isCompoundPrimaryKey && IsForeignKey()))))
                    persistentAttributeInfos.Add(GetPersistentAssociationAttribute());
            }
            else{
                if (_isPrimaryKey){
                    persistentAttributeInfos.Add(GetBrowsableAttribute());
                }
            }
            var persistentPersistentAttribute = GetPersistentPersistentAttribute();
            if (((IsCompoundForeignKey())) || isCompoundPrimaryKey)
                persistentPersistentAttribute.MapTo = String.Empty;
            persistentAttributeInfos.Add(persistentPersistentAttribute);
        }

        private IPersistentAttributeInfo GetBrowsableAttribute(){
            var persistentVisibleInDetailViewAttribute = _objectSpace.Create<IPersistentVisibleInDetailViewAttribute>();
            persistentVisibleInDetailViewAttribute.Visible = false;
            return persistentVisibleInDetailViewAttribute;
        }

        private IPersistentAttributeInfo GetRuleRequiredAttribute(){
            var requiredFieldAttribute = _objectSpace.Create<IPersistentRuleRequiredFieldAttribute>();
            requiredFieldAttribute.Context = DefaultContexts.Save.ToString();
            return requiredFieldAttribute;
        }

        bool IsOneToOne() {
            TemplateType templateType = _persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType;
            return templateType == TemplateType.XPOneToOnePropertyMember || templateType == TemplateType.XPOneToOneReadOnlyPropertyMember;
        }

        bool IsSelfRefOnTheKey(DBColumn dbColumn, bool isPrimaryKey) {
            if (!isPrimaryKey)
                return false;
            return _dbTable.ForeignKeys.FirstOrDefault(
                key => key.PrimaryKeyTable == _dbTable.Name && key.Columns.Contains(dbColumn.Name)) != null;
        }

        bool IsForeignKey() {
            return _dbTable.ForeignKeys.FirstOrDefault(key => key.Columns.Contains(_column.Name)) != null && !_isPrimaryKey;
        }

        bool IsCompoundForeignKey() {
            return _dbTable.ForeignKeys.FirstOrDefault(key => key.Columns.Contains(_column.Name) && key.Columns.Count > 1) != null;
        }

        IPersistentPersistentAttribute GetPersistentPersistentAttribute() {
            var persistentPersistentAttribute = _objectSpace.Create<IPersistentPersistentAttribute>();
            persistentPersistentAttribute.MapTo = _column.Name;
            return persistentPersistentAttribute;
        }

        IPersistentAssociationAttribute GetPersistentAssociationAttribute() {
            var persistentAssociationAttribute = _objectSpace.Create<IPersistentAssociationAttribute>();
            persistentAssociationAttribute.AssociationName =
                $"{_persistentMemberInfo.Name}-{_persistentMemberInfo.Owner.Name + _persistentMemberInfo.Name + "s"}s";
            return persistentAssociationAttribute;
        }

        bool IsSimpleForeignKey() {
            return _dbTable.ForeignKeys.FirstOrDefault(key => key.Columns.Count == 1 && key.Columns.Contains(_column.Name)) != null;
        }

        bool IsCompoundPrimaryKey() {
            var info = _persistentMemberInfo as IPersistentReferenceMemberInfo;
            if (info != null && info.ReferenceClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.Struct)
                return true;
            return false;
        }

        IPersistentKeyAttribute GetPersistentKeyAttribute() {
            var persistentAttributeInfo = _objectSpace.Create<IPersistentKeyAttribute>();
            persistentAttributeInfo.AutoGenerated = _column.IsIdentity;
            return persistentAttributeInfo;
        }

        IPersistentSizeAttribute GetPersistentSizeAttribute() {
            var persistentSizeAttribute = _objectSpace.Create<IPersistentSizeAttribute>();
            persistentSizeAttribute.Size = _column.Size;
            return persistentSizeAttribute;
        }

    }
}