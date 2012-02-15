using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.DBMapper;
using Xpand.ExpressApp.WorldCreator.SqlDBMapper;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.ExpressApp.WorldCreator.Core;

namespace Xpand.Tests.Xpand.WorldCreator.Mapper {
    public class ClassGeneratorHelper {
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        readonly DBTable _dbTable = new DBTable { Name = "MainTable" };
        readonly PersistentClassInfo _persistentClassInfo;
        readonly Dictionary<string, ClassGeneratorInfo> _classGeneratorInfos;

        public ClassGeneratorHelper(IObjectSpace objectSpace) {
            _persistentAssemblyInfo = objectSpace.CreateObject<PersistentAssemblyInfo>();
            _persistentAssemblyInfo.Name = "PersistentAssemblyInfo";
            _persistentClassInfo = objectSpace.CreateObject<PersistentClassInfo>();
            _persistentClassInfo.Name = "MainTable";
            _persistentAssemblyInfo.PersistentClassInfos.Add(_persistentClassInfo);
            _persistentClassInfo.SetDefaultTemplate(TemplateType.Class);
            _classGeneratorInfos = new Dictionary<string, ClassGeneratorInfo>();
            var classGeneratorInfo = new ClassGeneratorInfo(PersistentClassInfo, DbTable);
            _classGeneratorInfos.Add(PersistentClassInfo.Name, classGeneratorInfo);
        }
        public Dictionary<string, ClassGeneratorInfo> ClassGeneratorInfos {
            get { return _classGeneratorInfos; }
        }

        public PersistentClassInfo PersistentClassInfo {
            get { return _persistentClassInfo; }
        }

        public DBTable DbTable {
            get { return _dbTable; }
        }

        public IPersistentAssemblyInfo PersistentAssemblyInfo {
            get { return _persistentAssemblyInfo; }
        }
    }

    public class CompoundPKMemberGeneratorHelper : ClassGeneratorHelper {
        readonly DBColumn _dbColumn2;
        readonly DBColumn _dbColumn1;
        readonly PersistentClassInfo _structPersistentClassInfo;

        public CompoundPKMemberGeneratorHelper(IObjectSpace objectSpace)
            : base(objectSpace) {
            _dbColumn1 = new DBColumn { Name = "DBColumn1", ColumnType = DBColumnType.Int32 };
            _dbColumn2 = new DBColumn { Name = "DBColumn2", ColumnType = DBColumnType.Int32 };
            var dbColumns = new[] { _dbColumn1, _dbColumn2 };
            DbTable.Columns.AddRange(dbColumns);
            DbTable.PrimaryKey = new DBPrimaryKey(dbColumns);
            _structPersistentClassInfo = objectSpace.CreateObject<PersistentClassInfo>();
            _structPersistentClassInfo.Name = "MainTable" + TableMapper.KeyStruct;
            _structPersistentClassInfo.PersistentAssemblyInfo = (PersistentAssemblyInfo)PersistentAssemblyInfo;
            _structPersistentClassInfo.SetDefaultTemplate(TemplateType.Struct);
            ClassGeneratorInfos.Add(_structPersistentClassInfo.Name, new ClassGeneratorInfo(StructPersistentClassInfo, DbTable));
        }

        public PersistentClassInfo StructPersistentClassInfo {
            get { return _structPersistentClassInfo; }
        }

        public DBColumn DbColumn2 {
            get { return _dbColumn2; }
        }

        public DBColumn DbColumn1 {
            get { return _dbColumn1; }
        }
    }
    public class RefMemberGeneratorHelper : ClassGeneratorHelper {
        readonly DBTable _refDbTable = new DBTable("RefDbTable");
        readonly PersistentClassInfo _refPersistentClassInfo;


        public RefMemberGeneratorHelper(IObjectSpace objectSpace)
            : base(objectSpace) {
            var column = new DBColumn("Oid", false, "int", 0, DBColumnType.Int32);
            DbTable.PrimaryKey = new DBPrimaryKey(new[] { column });
            var dbColumn = new DBColumn { Name = "DBColumn", ColumnType = DBColumnType.Int32 };
            DbTable.Columns.Add(dbColumn);
            DbTable.Columns.Add(column);
            var refDbColumn = new DBColumn { Name = "refDbColumn", ColumnType = DBColumnType.Int32, IsKey = true };
            DbTable.Columns.Add(refDbColumn);
            var primaryKeyTableKeyColumns = new StringCollection { "refDbColumn" };
            DbTable.ForeignKeys.Add(new DBForeignKey(new List<DBColumn> { refDbColumn }, _refDbTable.Name, primaryKeyTableKeyColumns));

            var item = new DBColumn("Oid", true, "int", 0, DBColumnType.Int32);
            _refDbTable.Columns.Add(item);
            _refDbTable.PrimaryKey = new DBPrimaryKey(new[] { item });
            _refPersistentClassInfo = objectSpace.CreateObject<PersistentClassInfo>();
            _refPersistentClassInfo.Name = RefDbTable.Name;
            PersistentAssemblyInfo.PersistentClassInfos.Add(_refPersistentClassInfo);
            _refPersistentClassInfo.SetDefaultTemplate(TemplateType.Class);


            ClassGeneratorInfos.Add(_refPersistentClassInfo.Name, new ClassGeneratorInfo(_refPersistentClassInfo, RefDbTable));
        }


        public PersistentClassInfo RefPersistentClassInfo {
            get { return _refPersistentClassInfo; }
        }

        public DBTable RefDbTable {
            get { return _refDbTable; }
        }

        public DBColumn Column {
            get { return DbTable.Columns.First(); }
        }
        public DBColumn RefColumn {
            get { return DbTable.Columns.Single(column => column.Name == "refDbColumn"); }
        }


    }

}
