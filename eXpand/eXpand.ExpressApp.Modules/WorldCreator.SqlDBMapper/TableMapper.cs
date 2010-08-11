using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using Microsoft.SqlServer.Management.Smo;
using eXpand.ExpressApp.WorldCreator.Core;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class TableMapper
    {
        public const string KeyStruct = "KeyStruct";
        readonly ObjectSpace _objectSpace;
        readonly Database _database;
        readonly AttributeMapper _attributeMapper;

        public TableMapper(ObjectSpace objectSpace,Database database,AttributeMapper attributeMapper) {
            _objectSpace = objectSpace;
            _database = database;
            _attributeMapper = attributeMapper;
        }

        public IPersistentClassInfo Create(Table table, IPersistentAssemblyInfo persistentAssemblyInfo) {

            var persistentClassInfo = CreateCore(table, persistentAssemblyInfo);
            foreach (ForeignKey foreignKey in table.ForeignKeys) {
                var referencedTable = foreignKey.ReferencedTable;
                CreateCore(_database.Tables[referencedTable], persistentAssemblyInfo);
            }
            return persistentClassInfo;
        }

        private IPersistentClassInfo CreateCore(Table table, IPersistentAssemblyInfo persistentAssemblyInfo)
        {
            int count = table.Columns.OfType<Column>().Where(column => column.InPrimaryKey).Count();
            if (count>=1) {
                IPersistentClassInfo persistentClassInfo = CreatePersistentClassInfo(table.Name,TemplateType.Class, persistentAssemblyInfo);
                persistentClassInfo.BaseType = typeof (XPLiteObject);
                if (count>1)
                    CreatePersistentClassInfo(table.Name + KeyStruct, TemplateType.Struct,persistentAssemblyInfo);
                CreateExtraInfos(table, persistentClassInfo);
                return persistentClassInfo;
            }
            throw new NotImplementedException(table.Name);    
        }

        void CreateExtraInfos(Table table, IPersistentClassInfo persistentClassInfo) {
            var persistentAttributeInfos = _attributeMapper.Create(table,persistentClassInfo);
            foreach (var persistentAttributeInfo in persistentAttributeInfos) {
                persistentClassInfo.TypeAttributes.Add(persistentAttributeInfo);
            }
        }


        IPersistentClassInfo CreatePersistentClassInfo(string name, TemplateType templateType, IPersistentAssemblyInfo persistentAssemblyInfo)
        {
            var findBussinessObjectType = WCTypesInfo.Instance.FindBussinessObjectType<IPersistentClassInfo>();
            var info = _objectSpace.Session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction,
                                                                        findBussinessObjectType, CriteriaOperator.Parse("Name=?", name)) as IPersistentClassInfo;
            if (info!= null)
                return info;
            var persistentClassInfo = CreateNew(name);
            persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            persistentClassInfo.SetDefaultTemplate(templateType);
            return persistentClassInfo;
        }

        IPersistentClassInfo CreateNew(string name) {
            var persistentClassInfo = _objectSpace.CreateWCObject<IPersistentClassInfo>();
            persistentClassInfo.Name = name;
            return persistentClassInfo;
        }

    }
}