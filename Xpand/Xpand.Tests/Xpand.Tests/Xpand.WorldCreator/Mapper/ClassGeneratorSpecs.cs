using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Machine.Specifications;
using Xpand.ExpressApp.WorldCreator.DBMapper;
using Xpand.ExpressApp.WorldCreator.SqlDBMapper;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace Xpand.Tests.Xpand.WorldCreator.Mapper {
    [Subject(typeof(ClassGenerator))]
    public class When_mapping_a_table : With_In_Memory_DataStore {
        static IPersistentClassInfo _persistentClassInfo;
        static DBTable[] _dbTables;
        static ClassGeneratorHelper _generatorHelper;

        Establish context = () => {
            _generatorHelper = new ClassGeneratorHelper(ObjectSpace);
            _dbTables = new[] { _generatorHelper.DbTable };
        };

        Because of = () => {
            _persistentClassInfo = new ClassGenerator(_generatorHelper.PersistentAssemblyInfo, _dbTables).CreateAll().Select(info => info.PersistentClassInfo).First();
        };

        It should_return_a_persistent_classinfo_with_name_the_table_name = () => _persistentClassInfo.Name.ShouldEqual(_generatorHelper.DbTable.Name);

        It should_belong_to_the_passed_in_assembly =
            () => _persistentClassInfo.PersistentAssemblyInfo.ShouldEqual(_generatorHelper.PersistentAssemblyInfo);

        It should_derive_from_XpLiteObject =
            () => _persistentClassInfo.BaseType.ShouldEqual(typeof(XPLiteObject));

        It should_have_a_class_template =
            () => _persistentClassInfo.CodeTemplateInfo.CodeTemplate.TemplateType.ShouldEqual(TemplateType.Class);
    }

    public class When_mapping_a_table_with_compund_pk : With_In_Memory_DataStore {
        static DBTable[] _dbTables;
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            var dbTable = new DBTable("MainTable");
            var dbColumns = new[] { new DBColumn("col1", false, "int", 0, DBColumnType.Int32) { IsIdentity = true }, new DBColumn("col2", false, "int", 0, DBColumnType.Int32) { IsIdentity = true } };
            dbTable.Columns.AddRange(dbColumns);
            dbTable.PrimaryKey = new DBPrimaryKey(dbColumns);
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _dbTables = new[] { dbTable };
        };

        Because of = () => new ClassGenerator(_persistentAssemblyInfo, _dbTables).CreateAll().ToList();

        It should_have_a_classinfo_the_one_with_the_same_name_as_the_column_table_name_plus_KeyStruct =
            () => _persistentAssemblyInfo.PersistentClassInfos.FirstOrDefault(info => info.Name == "MainTable" + TableMapper.KeyStruct).ShouldNotBeNull();
    }
}
