using System.Linq;
using DevExpress.ExpressApp.Xpo;
using Microsoft.SqlServer.Management.Smo;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class ExtraInfoBuilder {
        public const string SupportPersistentObjectsAsAPartOfACompositeKey = "Support Persistent objects as a part of a composite key";
        readonly XPObjectSpace _objectSpace;
        readonly AttributeMapper _attributeMapper;

        public ExtraInfoBuilder(XPObjectSpace objectSpace, AttributeMapper attributeMapper) {
            _attributeMapper = attributeMapper;
            _objectSpace = objectSpace;
        }

        public void CreateCollection(IPersistentReferenceMemberInfo persistentReferenceMemberInfo, IPersistentClassInfo owner) {
            var persistentCollectionMemberInfo = _objectSpace.CreateWCObject<IPersistentCollectionMemberInfo>();
            persistentReferenceMemberInfo.ReferenceClassInfo.OwnMembers.Add(persistentCollectionMemberInfo);
            persistentCollectionMemberInfo.Name = persistentReferenceMemberInfo.Owner.Name + persistentReferenceMemberInfo.Name + "s";
            persistentCollectionMemberInfo.Owner = persistentReferenceMemberInfo.ReferenceClassInfo;
            persistentCollectionMemberInfo.CollectionClassInfo = owner;
            persistentCollectionMemberInfo.SetDefaultTemplate(TemplateType.XPCollectionMember);
            var persistentAssociationAttribute = _objectSpace.CreateWCObject<IPersistentAssociationAttribute>();
            persistentCollectionMemberInfo.TypeAttributes.Add(persistentAssociationAttribute);
            persistentAssociationAttribute.AssociationName = persistentReferenceMemberInfo.TypeAttributes.OfType<IPersistentAssociationAttribute>().Single().AssociationName;
        }

        public void CreateExtraInfos(Table table, IPersistentClassInfo persistentClassInfo, IMapperInfo mapperInfo) {
            var persistentAttributeInfos = _attributeMapper.Create(table, persistentClassInfo, mapperInfo);
            foreach (var persistentAttributeInfo in persistentAttributeInfos) {
                persistentClassInfo.TypeAttributes.Add(persistentAttributeInfo);
            }
            var count = table.Columns.OfType<Column>().Count(column => column.InPrimaryKey);
            if (count > 0) {
                if (persistentClassInfo.TemplateInfos.FirstOrDefault(info => info.Name==SupportPersistentObjectsAsAPartOfACompositeKey)==null) {
                    var templateInfo = (ITemplateInfo)_objectSpace.CreateObject(WCTypesInfo.Instance.FindBussinessObjectType(typeof(ITemplateInfo)));
                    templateInfo.Name = SupportPersistentObjectsAsAPartOfACompositeKey;
                    persistentClassInfo.TemplateInfos.Add(templateInfo);
                }
            }
        }

        public void CreateExtraInfos(Column column, IPersistentMemberInfo persistentMemberInfo, ForeignKeyCalculator foreignKeyCalculator) {
            var persistentReferenceMemberInfo = (IPersistentReferenceMemberInfo)persistentMemberInfo;
            if (persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.XPOneToOnePropertyMember) {
                CreateTemplateInfo(persistentReferenceMemberInfo, column, foreignKeyCalculator);
            } else if (!column.InPrimaryKey && persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.XPReadWritePropertyMember)
                CreateCollection(persistentReferenceMemberInfo, persistentReferenceMemberInfo.Owner);

        }
        void CreateTemplateInfo(IPersistentReferenceMemberInfo persistentReferenceMemberInfo, Column column, ForeignKeyCalculator _foreignKeyCalculator) {
            var table = (Table)column.Parent;
            var database = table.Parent;
            var foreignKey = _foreignKeyCalculator.GetForeignKey(database, column.Name, table);
            var templateInfo = _objectSpace.CreateWCObject<ITemplateInfo>();
            templateInfo.Name = persistentReferenceMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ToString();
            templateInfo.TemplateCode =
                _foreignKeyCalculator.GetRefTableForeignKey(foreignKey, column.Name).Columns.OfType<ForeignKeyColumn>().Single(keyColumn => keyColumn.ReferencedColumn == column.Name).Name;
            persistentReferenceMemberInfo.TemplateInfos.Add(templateInfo);
        }

    }
}
