using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using Microsoft.SqlServer.Management.Smo;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.WorldCreator.Core;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class TableMapper
    {
        public const string KeyStruct = "KeyStruct";
        readonly ObjectSpace _objectSpace;
        readonly Database _database;

        public TableMapper(ObjectSpace objectSpace,Database database) {
            _objectSpace = objectSpace;
            _database = database;
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
                IPersistentClassInfo persistentClassInfo = GetPersistentClassInfo(table.Name);
                persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
                persistentClassInfo.BaseType = typeof (XPLiteObject);
                persistentClassInfo.SetDefaultTemplate(TemplateType.Class);
                if (count>1)
                    CreateStructureClassInfo(persistentClassInfo.Name,persistentAssemblyInfo);
                return persistentClassInfo;
            }
            throw new NotImplementedException(table.Name);    
        }

        void CreateStructureClassInfo(string name, IPersistentAssemblyInfo persistentAssemblyInfo) {
            var persistentClassInfo = _objectSpace.CreateObjectFromInterface<IPersistentClassInfo>();
            persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            persistentClassInfo.SetDefaultTemplate(TemplateType.Struct);
            persistentClassInfo.Name = name + KeyStruct;
        }

        IPersistentClassInfo GetPersistentClassInfo(string name) {
            var findBussinessObjectType = XafTypesInfo.Instance.FindBussinessObjectType<IPersistentClassInfo>();
            return (IPersistentClassInfo) (_objectSpace.FindObject(findBussinessObjectType,CriteriaOperator.Parse("Name=?",name),true) ?? CreateNew(name));
        }

        IPersistentClassInfo CreateNew(string name) {
            var persistentClassInfo = _objectSpace.CreateObjectFromInterface<IPersistentClassInfo>();
            persistentClassInfo.Name = name;
            return persistentClassInfo;
        }
    }
}