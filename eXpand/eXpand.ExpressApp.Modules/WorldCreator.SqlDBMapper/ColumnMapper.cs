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
            IPersistentMemberInfo persistentMemberInfo;
            if (IsCoumboundPKColumn(column)){
                var structClassInfo = GetReferenceClassInfo(((Table)column.Parent).Name + TableMapper.KeyStruct);
                CreateStructMemberInfo(column, structClassInfo);
                persistentMemberInfo = CreateComboundKeyMemberInfo(column, structClassInfo, owner);
            }
            else{
                persistentMemberInfo = CreateMember(column, owner, TemplateType.XPReadWritePropertyMember);
                AddAttributes(column, persistentMemberInfo);
                if (column.IsForeignKey) {
                    CreateExtraInfos(column, owner, persistentMemberInfo);
                }
            }
            return persistentMemberInfo;
        }

        void CreateExtraInfos(Column column, IPersistentClassInfo owner, IPersistentMemberInfo persistentMemberInfo) {
            if (persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.XPOneToOnePropertyMember) {
                CreateTemplateInfo((IPersistentReferenceMemberInfo)persistentMemberInfo, column);
            }
            else if (persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType != TemplateType.XPOneToOnePropertyMember)
                CreateCollection((IPersistentReferenceMemberInfo) persistentMemberInfo);
            else {
                var table = (Table) column.Parent;
                var foreignKey = _foreignKeyCalculator.GetForeignKey(table.Parent, column.Name, table.Name);
                var persistentClassInfoOwner = GetReferenceClassInfo(foreignKey.ReferencedTable);
                CreatePersistentReferenceMemberInfo(foreignKey.ReferencedKey, persistentClassInfoOwner, owner,TemplateType.XPOneToOnePropertyMember);
            }
        }

        void AddAttributes(Column column, IPersistentMemberInfo persistentMemberInfo) {
            var persistentAttributeInfos = _attributeMapper.Create(column, persistentMemberInfo);
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

        void CreateStructMemberInfo(Column column, IPersistentClassInfo structClassInfo) {
            IPersistentMemberInfo structMemberInfo = CreateMember(column, structClassInfo, TemplateType.ReadWriteMember);
            AddAttributes(column, structMemberInfo);
            if (column.IsForeignKey)
                CreateCollection((IPersistentReferenceMemberInfo)structMemberInfo);
        }



        bool IsCoumboundPKColumn(Column column) {
            return column.InPrimaryKey&&HasMoreThanOnePK((Table) column.Parent);
        }

        IPersistentMemberInfo CreateMember(Column column, IPersistentClassInfo owner,TemplateType templateType) {
            if (!(column.IsForeignKey)){
                return CreatePersistentCoreTypeMemberInfo(column, owner, templateType);
            }
            var table = (Table)column.Parent;
            ForeignKey foreignKey = _foreignKeyCalculator.GetForeignKey(table.Parent, column.Name, table.Name);
            if (owner.CodeTemplateInfo.CodeTemplate.TemplateType != TemplateType.Struct && _foreignKeyCalculator.IsOneToOne(foreignKey))
                templateType=TemplateType.XPOneToOnePropertyMember;
            IPersistentClassInfo referenceClassInfo = GetReferenceClassInfo(foreignKey.ReferencedTable);
            var persistentReferenceMemberInfo = CreatePersistentReferenceMemberInfo(column.Name, owner, referenceClassInfo,templateType);
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

        void CreateCollection(IPersistentReferenceMemberInfo persistentReferenceMemberInfo) {
            var persistentCollectionMemberInfo = _objectSpace.CreateWCObject<IPersistentCollectionMemberInfo>();
            persistentReferenceMemberInfo.ReferenceClassInfo.OwnMembers.Add(persistentCollectionMemberInfo);
            persistentCollectionMemberInfo.Name = persistentReferenceMemberInfo.Name + "s";
            persistentCollectionMemberInfo.Owner = persistentReferenceMemberInfo.ReferenceClassInfo;
            persistentCollectionMemberInfo.CollectionClassInfo = persistentReferenceMemberInfo.Owner;
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