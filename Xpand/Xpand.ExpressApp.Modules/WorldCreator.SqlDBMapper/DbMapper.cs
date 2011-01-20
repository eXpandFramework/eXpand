using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Microsoft.SqlServer.Management.Smo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class DbMapper {
        readonly ObjectSpace _objectSpace;
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        IDataStoreLogonObject _dataStoreLogonObject;

        public DbMapper(ObjectSpace objectSpace, IPersistentAssemblyInfo persistentAssemblyInfo, IDataStoreLogonObject dataStoreLogonObject) {
            _objectSpace = objectSpace;
            _persistentAssemblyInfo = persistentAssemblyInfo;
            _dataStoreLogonObject = dataStoreLogonObject;
        }

        public void Map(Database database, IMapperInfo mapperInfo) {
            var attributeMapper = new AttributeMapper(_objectSpace);
            var tableMapper = new TableMapper(_objectSpace, database, attributeMapper);
            var dataTypeMapper = new DataTypeMapper();
            var columnMapper = new ColumnMapper(dataTypeMapper, attributeMapper);
            Tracing.Tracer.LogSeparator("DBMapper Start mapping database " + database.Name);
            foreach (Table table in database.Tables.OfType<Table>().Where(table => !(table.IsSystemObject))) {
                Tracing.Tracer.LogValue("Table", table.Name);
                IPersistentClassInfo persistentClassInfo = tableMapper.Create(table, _persistentAssemblyInfo, mapperInfo);
                foreach (Column column in table.Columns) {
                    columnMapper.Create(column, persistentClassInfo);
                }
            }
            _dataStoreLogonObject = (IDataStoreLogonObject)ReflectionHelper.CreateObject(XafTypesInfo.Instance.FindBussinessObjectType<IDataStoreLogonObject>(), new object[] { _persistentAssemblyInfo.Session, _dataStoreLogonObject });
            attributeMapper.Create(_persistentAssemblyInfo, _dataStoreLogonObject);
            Func<ITemplateInfo, bool> templateInfoPredicate = info => info.Name == ExtraInfoBuilder.SupportPersistentObjectsAsAPartOfACompositeKey;
            CodeEngine.SupportCompositeKeyPersistentObjects(_persistentAssemblyInfo, templateInfoPredicate);
        }
    }
}