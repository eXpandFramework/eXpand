using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.DBMapper {
    public struct ClassGeneratorInfo {
        readonly IPersistentClassInfo _persistentClassInfo;
        readonly DBTable _dbTable;
        public ClassGeneratorInfo(IPersistentClassInfo persistentClassInfo, DBTable dbTable)
            : this() {
            _persistentClassInfo = persistentClassInfo;
            _dbTable = dbTable;
        }

        public IPersistentClassInfo PersistentClassInfo {
            get { return _persistentClassInfo; }
        }

        public DBTable DbTable {
            get { return _dbTable; }
        }

    }


    public class ClassGenerator {
        public const string KeyStruct = "KeyStruct";

        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        readonly DBTable[] _dbTables;
        readonly IObjectSpace _objectSpace;

        public ClassGenerator(IPersistentAssemblyInfo persistentAssemblyInfo, DBTable[] dbTables) {
            _persistentAssemblyInfo = persistentAssemblyInfo;
            _dbTables = dbTables;
            _objectSpace = XPObjectSpace.FindObjectSpaceByObject(persistentAssemblyInfo);
        }

        public IEnumerable<ClassGeneratorInfo> CreateAll() {
            foreach (var storageTable in _dbTables) {
                yield return new ClassGeneratorInfo(CreateClassInfo(storageTable.Name), storageTable);
                if (storageTable.PrimaryKey != null && storageTable.PrimaryKey.Columns.Count > 1)
                    yield return new ClassGeneratorInfo(CreateClassInfo(storageTable.Name + KeyStruct, TemplateType.Struct), storageTable);
            }
        }

        public static string GetTableName(string name) {
            var indexOf = name.IndexOf(".", System.StringComparison.Ordinal);
            if (indexOf > -1)
                name = name.Substring(indexOf + 1);
            return CodeEngine.CleanName(name);
        }

        public IPersistentClassInfo CreateClassInfo(string name, TemplateType templateType = TemplateType.Class) {
            var persistentClassInfo = _objectSpace.CreateWCObject<IPersistentClassInfo>();
            _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
            persistentClassInfo.SetDefaultTemplate(templateType);
            persistentClassInfo.Name = GetTableName(name);
            persistentClassInfo.PersistentAssemblyInfo = _persistentAssemblyInfo;
            persistentClassInfo.BaseType = typeof(XPLiteObject);
            return persistentClassInfo;
        }
    }
}