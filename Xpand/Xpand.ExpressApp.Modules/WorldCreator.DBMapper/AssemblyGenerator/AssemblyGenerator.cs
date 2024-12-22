using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.DBMapper.AssemblyGenerator {
    public class AssemblyGenerator {
        private readonly string _connectionString;
        private readonly string _navigationPath;
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        readonly IObjectSpace _objectSpace;
        readonly DBTable[] _storageTables;


        public AssemblyGenerator(string connectionString,string navigationPath, IPersistentAssemblyInfo persistentAssemblyInfo, string[] tables) {
            
            _connectionString = connectionString;
            _navigationPath = navigationPath;
            _persistentAssemblyInfo = persistentAssemblyInfo;
            var dataStoreSchemaExplorer = ((IDataStoreSchemaExplorer)XpoDefault.GetConnectionProvider(_connectionString, AutoCreateOption.None));
            dataStoreSchemaExplorer = GetDataStoreSchemaExplorer(dataStoreSchemaExplorer);
            _storageTables = dataStoreSchemaExplorer.GetStorageTables(tables).Where(table => table.PrimaryKey != null).ToArray();
            _objectSpace = XPObjectSpace.FindObjectSpaceByObject(persistentAssemblyInfo);
        }

        public static IDataStoreSchemaExplorer GetDataStoreSchemaExplorer(IDataStoreSchemaExplorer dataStoreSchemaExplorer) {
            if (dataStoreSchemaExplorer is MSSqlConnectionProvider schemaExplorer) {
                var msSqlConnectionProvider = schemaExplorer;
                dataStoreSchemaExplorer = new MSSqlConnectionProvider(msSqlConnectionProvider.Connection, msSqlConnectionProvider.AutoCreateOption);
            } else if (dataStoreSchemaExplorer is OracleConnectionProvider provider) {
                dataStoreSchemaExplorer = new Xpo.ConnectionProviders.OracleConnectionProvider(provider.Connection, provider.AutoCreateOption);
            } else if (dataStoreSchemaExplorer is MySqlConnectionProvider msSqlConnectionProvider) {
                dataStoreSchemaExplorer = new Xpo.ConnectionProviders.MySqlConnectionProvider(msSqlConnectionProvider.Connection, msSqlConnectionProvider.AutoCreateOption);
            }
            return dataStoreSchemaExplorer;
        }

        public void Create() {
            Dictionary<string, ClassGeneratorInfo> generatorInfos = new ClassGenerator(_persistentAssemblyInfo, _storageTables).CreateAll().ToDictionary(classGeneratorInfo => classGeneratorInfo.PersistentClassInfo.Name);
            foreach (var classGeneratorInfo in generatorInfos.Where(pair => pair.Value.PersistentClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.Class)) {
                var generatorInfo = classGeneratorInfo.Value;
                new ClassAtrributeGenerator(generatorInfo, _navigationPath).Create().Each(info => generatorInfo.PersistentClassInfo.TypeAttributes.Add(info));
                var memberGeneratorInfos = new MemberGenerator(classGeneratorInfo.Value.DbTable, generatorInfos).Create();
                memberGeneratorInfos.Each(info => new MemberAttributeGenerator(info, generatorInfo).Create());
            }
            var oneToOneMemberInfos = _persistentAssemblyInfo.PersistentClassInfos.SelectMany(info => info.OwnMembers.OfType<IPersistentReferenceMemberInfo>()).Where(info => info.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.XPOneToOnePropertyMember);
            foreach (var oneToOneMemberInfo in oneToOneMemberInfos) {
                var codeTemplate = _objectSpace.Create<ICodeTemplate>();
                codeTemplate.TemplateCode = oneToOneMemberInfo.ReferenceClassInfo.OwnMembers.OfType<IPersistentReferenceMemberInfo>().Single(info => info.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.XPOneToOnePropertyMember).Name;
                oneToOneMemberInfo.TemplateInfos.Add(codeTemplate);
            }
            _persistentAssemblyInfo.SupportCompositeKeyPersistentObjects();
            CreateAssemblyAttributes();
        }


        void CreateAssemblyAttributes() {
            if (_persistentAssemblyInfo.PersistentClassInfos.Any()) {
                var persistentAssemblyDataStoreAttributeInfo =
                    _objectSpace.Create<IPersistentAssemblyDataStoreAttribute>();
                persistentAssemblyDataStoreAttributeInfo.ConnectionString = _connectionString;
                persistentAssemblyDataStoreAttributeInfo.PersistentClassInfo = _persistentAssemblyInfo.PersistentClassInfos[0];
                _persistentAssemblyInfo.Attributes.Add(persistentAssemblyDataStoreAttributeInfo);
            }
        }
    }

}