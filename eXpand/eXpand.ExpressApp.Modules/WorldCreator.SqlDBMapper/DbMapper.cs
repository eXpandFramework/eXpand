using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using Microsoft.SqlServer.Management.Smo;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator.SqlDBMapper {
    public class DbMapper
    {
        readonly ObjectSpace _objectSpace;
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;

        public DbMapper(ObjectSpace objectSpace,IPersistentAssemblyInfo persistentAssemblyInfo) {
            _objectSpace = objectSpace;
            _persistentAssemblyInfo = persistentAssemblyInfo;
        }

        public void Map(Database database) {
            var attributeMapper = new AttributeMapper(_objectSpace);
            var tableMapper = new TableMapper(_objectSpace,database,attributeMapper);
            var dataTypeMapper = new DataTypeMapper();
            var columnMapper = new ColumnMapper(dataTypeMapper, attributeMapper);
            Tracing.Tracer.LogSeparator("DBMapper Start mapping datatbase "+database.Name);
            foreach (Table table in database.Tables.OfType<Table>().Where(table => !(table.IsSystemObject))){
                Tracing.Tracer.LogValue("Table",table.Name);
                IPersistentClassInfo persistentClassInfo = tableMapper.Create(table, _persistentAssemblyInfo);
                foreach (Column column in table.Columns) {
                    columnMapper.Create(column, persistentClassInfo);
                }
            }

            Func<ITemplateInfo, bool> templateInfoPredicate = info => info.Name == ExtraInfoBuilder.SupportPersistentObjectsAsAPartOfACompositeKey;
            CodeEngine.SupportCompositeKeyPersistentObjects(_persistentAssemblyInfo, templateInfoPredicate);
        }
    }
}