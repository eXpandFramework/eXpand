using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using Microsoft.SqlServer.Management.Smo;
using System.Linq;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class DbMapper
    {
        readonly ObjectSpace _objectSpace;
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        IDataStoreLogonObject _dataStoreLogonObject;

        public DbMapper(ObjectSpace objectSpace, IPersistentAssemblyInfo persistentAssemblyInfo, IDataStoreLogonObject dataStoreLogonObject) {
            _objectSpace = objectSpace;
            _persistentAssemblyInfo = persistentAssemblyInfo;
            _dataStoreLogonObject = dataStoreLogonObject;
        }

        public void Map(Database database) {
            var attributeMapper = new AttributeMapper(_objectSpace);
            var tableMapper = new TableMapper(_objectSpace,database,attributeMapper);
            var dataTypeMapper = new DataTypeMapper();
            var columnMapper = new ColumnMapper(dataTypeMapper, attributeMapper);
            Tracing.Tracer.LogSeparator("DBMapper Start mapping database "+database.Name);
            foreach (Table table in database.Tables.OfType<Table>().Where(table => !(table.IsSystemObject))){
                Tracing.Tracer.LogVerboseValue("Table",table.Name);
                IPersistentClassInfo persistentClassInfo = tableMapper.Create(table, _persistentAssemblyInfo);
                foreach (Column column in table.Columns) {
                    columnMapper.Create(column, persistentClassInfo);
                }
            }
            _dataStoreLogonObject = (IDataStoreLogonObject) ReflectionHelper.CreateObject(XafTypesInfo.Instance.FindBussinessObjectType<IDataStoreLogonObject>(), new object[] { _persistentAssemblyInfo.Session, _dataStoreLogonObject });
            attributeMapper.Create(_persistentAssemblyInfo, _dataStoreLogonObject);
            Func<ITemplateInfo, bool> templateInfoPredicate = info => info.Name == ExtraInfoBuilder.SupportPersistentObjectsAsAPartOfACompositeKey;
            CodeEngine.SupportCompositeKeyPersistentObjects(_persistentAssemblyInfo, templateInfoPredicate);
        }
    }
}