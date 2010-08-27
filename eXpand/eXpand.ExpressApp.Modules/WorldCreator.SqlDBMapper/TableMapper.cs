using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
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
        readonly ExtraInfoBuilder _extraInfoBuilder;

        public TableMapper(ObjectSpace objectSpace,Database database,AttributeMapper attributeMapper) {
            _objectSpace = objectSpace;
            _database = database;
            _attributeMapper = attributeMapper;
            _extraInfoBuilder = new ExtraInfoBuilder(_objectSpace, _attributeMapper);
        }

        public IPersistentClassInfo Create(Table table, IPersistentAssemblyInfo persistentAssemblyInfo, IMapperInfo mapperInfo) {

            var persistentClassInfo = CreateCore(table, persistentAssemblyInfo,mapperInfo);
            foreach (ForeignKey foreignKey in table.ForeignKeys) {
                CreateCore(_database.Tables[foreignKey.ReferencedTable], persistentAssemblyInfo, mapperInfo);
            }
            return persistentClassInfo;
        }

        private IPersistentClassInfo CreateCore(Table table, IPersistentAssemblyInfo persistentAssemblyInfo, IMapperInfo mapperInfo)
        {
            int count = table.Columns.OfType<Column>().Where(column => column.InPrimaryKey).Count();
            IPersistentClassInfo persistentClassInfo = CreatePersistentClassInfo(table.Name,TemplateType.Class, persistentAssemblyInfo);
            persistentClassInfo.BaseType = typeof (XPLiteObject);
            if (count>1)
                CreatePersistentClassInfo(table.Name + KeyStruct, TemplateType.Struct,persistentAssemblyInfo);
            _extraInfoBuilder.CreateExtraInfos(table, persistentClassInfo,mapperInfo);
            if (count==0) 
                Tracing.Tracer.LogError("No primary keys found for "+table.Name);
            return persistentClassInfo;
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