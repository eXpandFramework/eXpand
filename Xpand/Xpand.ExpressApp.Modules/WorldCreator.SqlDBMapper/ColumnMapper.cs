using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Microsoft.SqlServer.Management.Smo;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class ColumnMapper {
        readonly DataTypeMapper _dataTypeMapper;
        readonly AttributeMapper _attributeMapper;
        readonly XPObjectSpace _objectSpace;
        readonly ForeignKeyCalculator _foreignKeyCalculator = new ForeignKeyCalculator();
        readonly ExtraInfoBuilder _extraInfoBuilder;
        public ColumnMapper(DataTypeMapper dataTypeMapper, AttributeMapper attributeMapper) {
            _dataTypeMapper = dataTypeMapper;
            _attributeMapper = attributeMapper;
            _objectSpace = _attributeMapper.ObjectSpace;
            _extraInfoBuilder = new ExtraInfoBuilder(_objectSpace, _attributeMapper);
        }

        public IPersistentMemberInfo Create(Column column, IPersistentClassInfo owner) {
            IPersistentMemberInfo persistentMemberInfo = null;
            if (IsCoumboundPKColumn(column)) {
                var structClassInfo = GetReferenceClassInfo(((Table)column.Parent).Name + TableMapper.KeyStruct);
                var structMemberInfo = CreateStructMemberInfo(column, structClassInfo, owner);
                if (structMemberInfo != null)
                    persistentMemberInfo = CreateComboundKeyMemberInfo(column, structClassInfo, owner);
            } else {
                persistentMemberInfo = CreateMember(column, owner, TemplateType.XPReadWritePropertyMember);
                if (persistentMemberInfo != null) {
                    AddAttributes(column, persistentMemberInfo);
                    if (column.IsForeignKey) {
                        _extraInfoBuilder.CreateExtraInfos(column, persistentMemberInfo, _foreignKeyCalculator);
                    }
                }
            }
            return persistentMemberInfo;
        }


        void AddAttributes(Column column, IPersistentMemberInfo persistentMemberInfo) {
            var persistentAttributeInfos = _attributeMapper.Create(column, persistentMemberInfo, _dataTypeMapper);
            foreach (var persistentAttributeInfo in persistentAttributeInfos) {
                persistentMemberInfo.TypeAttributes.Add(persistentAttributeInfo);
            }
        }

        IPersistentMemberInfo CreateComboundKeyMemberInfo(Column column, IPersistentClassInfo structClassInfo, IPersistentClassInfo owner) {
            IPersistentMemberInfo persistentMemberInfo;
            if (owner.OwnMembers.Where(info => info.Name == structClassInfo.Name).FirstOrDefault() == null) {
                var persistentReferenceMemberInfo = CreatePersistentReferenceMemberInfo(structClassInfo.Name, owner, structClassInfo, TemplateType.FieldMember);
                AddAttributes(column, persistentReferenceMemberInfo);
                persistentMemberInfo = persistentReferenceMemberInfo;
            } else
                persistentMemberInfo = null;
            return persistentMemberInfo;
        }

        IPersistentMemberInfo CreateStructMemberInfo(Column column, IPersistentClassInfo structClassInfo, IPersistentClassInfo owner) {
            IPersistentMemberInfo structMemberInfo = CreateMember(column, structClassInfo, TemplateType.ReadWriteMember);
            if (structMemberInfo != null) {
                AddAttributes(column, structMemberInfo);
                if (column.IsForeignKey)
                    _extraInfoBuilder.CreateCollection((IPersistentReferenceMemberInfo)structMemberInfo, owner);
            }
            return structMemberInfo;
        }



        bool IsCoumboundPKColumn(Column column) {
            return column.InPrimaryKey && HasMoreThanOnePK((Table)column.Parent);
        }

        IPersistentMemberInfo CreateMember(Column column, IPersistentClassInfo owner, TemplateType templateType) {

            var columnName = column.Name;
            ForeignKey foreignKey = _foreignKeyCalculator.GetForeignKey(column);
            if (column.IsForeignKey && owner.CodeTemplateInfo.CodeTemplate.TemplateType != TemplateType.Struct && _foreignKeyCalculator.IsOneToOne(foreignKey, columnName))
                templateType = TemplateType.XPOneToOnePropertyMember;
            else if (foreignKey != null && foreignKey.Columns.Count > 1) {
                columnName = foreignKey.ReferencedTable;
            }
            if (_objectSpace.FindObject<IPersistentMemberInfo>(info => info.Name == columnName && info.Owner == owner, PersistentCriteriaEvaluationBehavior.InTransaction) != null)
                return null;
            if (!(column.IsForeignKey)) {
                return CreatePersistentCoreTypeMemberInfo(column, owner, templateType);
            }
            if (foreignKey != null) {
                IPersistentClassInfo referenceClassInfo = GetReferenceClassInfo(foreignKey.ReferencedTable);
                var persistentReferenceMemberInfo = CreatePersistentReferenceMemberInfo(columnName, owner, referenceClassInfo, templateType);
                return persistentReferenceMemberInfo;
            }
            throw new NotImplementedException(column.Name + " " + ((Table)column.Parent).Name);
        }



        IPersistentClassInfo GetReferenceClassInfo(string name) {
            return (IPersistentClassInfo)_objectSpace.FindObject(WCTypesInfo.Instance.FindBussinessObjectType<IPersistentClassInfo>(),
                                                                 CriteriaOperator.Parse("Name=?", name), true);
        }

        IPersistentCoreTypeMemberInfo CreatePersistentCoreTypeMemberInfo(Column column, IPersistentClassInfo owner, TemplateType templateType) {
            var persistentCoreTypeMemberInfo = _objectSpace.CreateWCObject<IPersistentCoreTypeMemberInfo>();
            persistentCoreTypeMemberInfo.Name = column.Name;
            persistentCoreTypeMemberInfo.DataType = _dataTypeMapper.GetDataType(column);
            owner.OwnMembers.Add(persistentCoreTypeMemberInfo);
            persistentCoreTypeMemberInfo.SetDefaultTemplate(templateType);
            return persistentCoreTypeMemberInfo;
        }

        bool HasMoreThanOnePK(Table table) {
            return table.Columns.OfType<Column>().Where(column => column.InPrimaryKey).Count() > 1;
        }


        IPersistentReferenceMemberInfo CreatePersistentReferenceMemberInfo(string name, IPersistentClassInfo owner, IPersistentClassInfo referenceClassInfo, TemplateType templateType) {
            var persistentReferenceMemberInfo = _objectSpace.CreateWCObject<IPersistentReferenceMemberInfo>();
            persistentReferenceMemberInfo.Name = name;
            persistentReferenceMemberInfo.ReferenceClassInfo = referenceClassInfo;
            owner.OwnMembers.Add(persistentReferenceMemberInfo);
            persistentReferenceMemberInfo.SetDefaultTemplate(templateType);
            return persistentReferenceMemberInfo;
        }




    }
}