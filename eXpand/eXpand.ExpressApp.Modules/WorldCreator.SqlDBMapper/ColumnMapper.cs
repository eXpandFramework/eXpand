using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Microsoft.SqlServer.Management.Smo;
using System.Linq;
using eXpand.ExpressApp.WorldCreator.Core;

namespace eXpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class ColumnMapper {
        readonly DataTypeMapper _dataTypeMapper;
        readonly AttributeMapper _attributeMapper;
        readonly ObjectSpace _objectSpace;
        readonly ForeignKeyCalculator _foreignKeyCalculator=new ForeignKeyCalculator();
        public ColumnMapper(DataTypeMapper dataTypeMapper, AttributeMapper attributeMapper) {
            _dataTypeMapper = dataTypeMapper;
            _attributeMapper = attributeMapper;
            _objectSpace = _attributeMapper.ObjectSpace;
        }

        public IPersistentMemberInfo Create(Column column, IPersistentClassInfo owner) {
            IPersistentMemberInfo persistentMemberInfo = null;
            if (IsCoumboundPKColumn(column)){
                var structClassInfo = GetReferenceClassInfo(((Table)column.Parent).Name + TableMapper.KeyStruct);
                var structMemberInfo = CreateStructMemberInfo(column, structClassInfo,owner);
                if (structMemberInfo!= null)
                    persistentMemberInfo = CreateComboundKeyMemberInfo(column, structClassInfo, owner);
            }
            else{
                persistentMemberInfo = CreateMember(column, owner, TemplateType.XPReadWritePropertyMember);
                if (persistentMemberInfo != null) {
                    AddAttributes(column, persistentMemberInfo);
                    if (column.IsForeignKey) {
                        CreateExtraInfos(column, persistentMemberInfo);
                    }
                }
            }
            return persistentMemberInfo;
        }

        void CreateExtraInfos(Column column, IPersistentMemberInfo persistentMemberInfo) {
            var persistentReferenceMemberInfo = (IPersistentReferenceMemberInfo) persistentMemberInfo;
            if (persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.XPOneToOnePropertyMember) {
                CreateTemplateInfo(persistentReferenceMemberInfo, column);
            }
            else if (!column.InPrimaryKey&& persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.XPReadWritePropertyMember)
                CreateCollection(persistentReferenceMemberInfo, persistentReferenceMemberInfo.Owner);
            else {
                throw new NotImplementedException();
            }
        }

        void AddAttributes(Column column, IPersistentMemberInfo persistentMemberInfo) {
            var persistentAttributeInfos = _attributeMapper.Create(column, persistentMemberInfo,_dataTypeMapper);
            foreach (var persistentAttributeInfo in persistentAttributeInfos) {
                persistentMemberInfo.TypeAttributes.Add(persistentAttributeInfo);    
            }
        }

        IPersistentMemberInfo CreateComboundKeyMemberInfo(Column column, IPersistentClassInfo structClassInfo, IPersistentClassInfo owner) {
            IPersistentMemberInfo persistentMemberInfo;
            if (owner.OwnMembers.Where(info => info.Name == structClassInfo.Name).FirstOrDefault() == null) {
                var persistentReferenceMemberInfo = CreatePersistentReferenceMemberInfo(structClassInfo.Name, owner, structClassInfo, TemplateType.FieldMember);
                AddAttributes(column, persistentReferenceMemberInfo);
                persistentMemberInfo= persistentReferenceMemberInfo;
            }
            else
                persistentMemberInfo = null;
            return persistentMemberInfo;
        }

        IPersistentMemberInfo CreateStructMemberInfo(Column column, IPersistentClassInfo structClassInfo, IPersistentClassInfo owner) {
            IPersistentMemberInfo structMemberInfo = CreateMember(column, structClassInfo, TemplateType.ReadWriteMember);
            if (structMemberInfo != null) {
                AddAttributes(column, structMemberInfo);
                if (column.IsForeignKey)
                    CreateCollection((IPersistentReferenceMemberInfo)structMemberInfo, owner);
            }
            return structMemberInfo;
        }



        bool IsCoumboundPKColumn(Column column) {
            return column.InPrimaryKey&&HasMoreThanOnePK((Table) column.Parent);
        }

        IPersistentMemberInfo CreateMember(Column column, IPersistentClassInfo owner,TemplateType templateType) {
            if (_objectSpace.)
            if (!(column.IsForeignKey)){
                return CreatePersistentCoreTypeMemberInfo(column, owner, templateType);
            }
            var table = (Table)column.Parent;
            var columnName = column.Name;
            ForeignKey foreignKey = _foreignKeyCalculator.GetForeignKey(table.Parent, columnName, table.Name);
            if (owner.CodeTemplateInfo.CodeTemplate.TemplateType != TemplateType.Struct && _foreignKeyCalculator.IsOneToOne(foreignKey))
                templateType=TemplateType.XPOneToOnePropertyMember;
            else if (foreignKey.Columns.Count>1) {
                columnName = foreignKey.ReferencedTable;
            }
            IPersistentClassInfo referenceClassInfo = GetReferenceClassInfo(foreignKey.ReferencedTable);
            var persistentReferenceMemberInfo = CreatePersistentReferenceMemberInfo(columnName, owner, referenceClassInfo,templateType);
            return persistentReferenceMemberInfo;
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
            return table.Columns.OfType<Column>().Where(column => column.InPrimaryKey).Count()>1;
        }

        void CreateCollection(IPersistentReferenceMemberInfo persistentReferenceMemberInfo, IPersistentClassInfo owner) {
            var persistentCollectionMemberInfo = _objectSpace.CreateWCObject<IPersistentCollectionMemberInfo>();
            persistentReferenceMemberInfo.ReferenceClassInfo.OwnMembers.Add(persistentCollectionMemberInfo);
            persistentCollectionMemberInfo.Name = persistentReferenceMemberInfo.Owner.Name + persistentReferenceMemberInfo.Name + "s";
            persistentCollectionMemberInfo.Owner = persistentReferenceMemberInfo.ReferenceClassInfo;
            persistentCollectionMemberInfo.CollectionClassInfo = owner;
            persistentCollectionMemberInfo.SetDefaultTemplate(TemplateType.XPCollectionMember);
            var persistentAssociationAttribute = _objectSpace.CreateWCObject<IPersistentAssociationAttribute>();
            persistentCollectionMemberInfo.TypeAttributes.Add(persistentAssociationAttribute);
            persistentAssociationAttribute.AssociationName =persistentReferenceMemberInfo.TypeAttributes.OfType<IPersistentAssociationAttribute>().Single().AssociationName;
        }

        IPersistentReferenceMemberInfo CreatePersistentReferenceMemberInfo(string name, IPersistentClassInfo owner, IPersistentClassInfo referenceClassInfo, TemplateType templateType)
        {
            var persistentReferenceMemberInfo = _objectSpace.CreateWCObject<IPersistentReferenceMemberInfo>();
            persistentReferenceMemberInfo.Name = name;
            persistentReferenceMemberInfo.ReferenceClassInfo=referenceClassInfo;
            owner.OwnMembers.Add(persistentReferenceMemberInfo);
            persistentReferenceMemberInfo.SetDefaultTemplate(templateType);
            return persistentReferenceMemberInfo;
        }

        void CreateTemplateInfo(IPersistentReferenceMemberInfo persistentReferenceMemberInfo, Column column) {
            var table = (Table)column.Parent;
            var database = table.Parent;
            var foreignKey = _foreignKeyCalculator.GetForeignKey(database, column.Name, table.Name);
            var templateInfo = _objectSpace.CreateWCObject<ITemplateInfo>();
            templateInfo.Name = persistentReferenceMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ToString();
            templateInfo.TemplateCode = _foreignKeyCalculator.GetRefTableForeignKey(foreignKey).Columns.OfType<ForeignKeyColumn>().Single().Name;
            persistentReferenceMemberInfo.TemplateInfos.Add(templateInfo);
        }



    }
}