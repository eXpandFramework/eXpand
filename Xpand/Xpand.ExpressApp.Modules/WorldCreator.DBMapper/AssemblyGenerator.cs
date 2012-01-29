using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Utils.Helpers;
using System.Linq;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    public class AssemblyGenerator {
        readonly IDataStoreSchemaExplorer _dataStoreSchemaExplorer;
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        readonly IObjectSpace _objectSpace;
        readonly LogonObject _logonObject;


        public AssemblyGenerator(LogonObject logonObject, IPersistentAssemblyInfo persistentAssemblyInfo) {
            var connectionProvider = XpoDefault.GetConnectionProvider(logonObject.ConnectionString, AutoCreateOption.None);
            _dataStoreSchemaExplorer = (IDataStoreSchemaExplorer)connectionProvider;
            _persistentAssemblyInfo = persistentAssemblyInfo;
            _logonObject = logonObject;
            _objectSpace = ObjectSpace.FindObjectSpaceByObject(persistentAssemblyInfo);
        }

        public void Create() {
            var storageTables = GetStorageTables();
            Dictionary<string, ClassGeneratorInfo> generatorInfos = new ClassGenerator(_persistentAssemblyInfo, storageTables).CreateAll().ToDictionary(classGeneratorInfo => classGeneratorInfo.PersistentClassInfo.Name);
            foreach (var classGeneratorInfo in generatorInfos.Where(pair => pair.Value.PersistentClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType == TemplateType.Class)) {
                var generatorInfo = classGeneratorInfo.Value;
                new ClassAtrributeGenerator(generatorInfo, _logonObject.NavigationPath).Create().Each(info => generatorInfo.PersistentClassInfo.TypeAttributes.Add(info));
                var memberGeneratorInfos = new MemberGenerator(classGeneratorInfo.Value.DbTable, generatorInfos).Create();
                var list = classGeneratorInfo.Value.PersistentClassInfo.OwnMembers.Select(info => info.Name).ToList();
                memberGeneratorInfos.Each(info => new MemberAttributeGenerator(info, generatorInfo).Create());
                Debug.Print(classGeneratorInfo.Value.DbTable.Name);
            }
            CodeEngine.SupportCompositeKeyPersistentObjects(_persistentAssemblyInfo);
            CreateAssemblyAttributes();
        }

        DBTable[] GetStorageTables() {
            var systemTalbes = new List<string> { "sysdiagrams", "xpobjecttype" };
            var tableNames = _dataStoreSchemaExplorer.GetStorageTablesList().Where(s => !systemTalbes.Contains(s.ToLower()));
            var storageTables = _dataStoreSchemaExplorer.GetStorageTables(tableNames.ToArray()).Where(table => table.PrimaryKey != null).ToArray();
            return storageTables;
        }

        void CreateAssemblyAttributes() {
            if (_persistentAssemblyInfo.PersistentClassInfos.Count() > 0) {
                var persistentAssemblyDataStoreAttributeInfo =
                    _objectSpace.CreateWCObject<IPersistentAssemblyDataStoreAttribute>();
                persistentAssemblyDataStoreAttributeInfo.ConnectionString = _logonObject.ConnectionString;
                persistentAssemblyDataStoreAttributeInfo.PersistentClassInfo = _persistentAssemblyInfo.PersistentClassInfos[0];
                _persistentAssemblyInfo.Attributes.Add(persistentAssemblyDataStoreAttributeInfo);
            }
        }
    }

}