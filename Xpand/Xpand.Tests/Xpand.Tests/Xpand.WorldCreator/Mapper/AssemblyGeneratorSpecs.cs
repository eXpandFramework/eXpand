using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.WorldCreator.DBMapper;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Tests.Xpand.WorldCreator.Mapper {
    public class When_mapping_a_schema : With_In_Memory_DataStore {
        static MSSqlConnectionProvider _dataStoreSchemaExplorer;
        static List<IPersistentAttributeInfo> _memberPersistentAttributeInfos;
        static PersistentClassInfo _persistentClassInfo1;
        static IList<IPersistentClassInfo> _persistentClassInfos;
        static LogonObject _logonObject;
        static PersistentClassInfo _persistentClassInfo2;
        static PersistentMemberInfo _persistentMemberInfo1;
        static List<IPersistentAttributeInfo> _classPersistentAttributeInfos;
        static PersistentMemberInfo _persistentMemberInfo2;
        static IPersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _dataStoreSchemaExplorer = Isolate.Fake.Instance<MSSqlConnectionProvider>();
            Isolate.Fake.StaticMethods(typeof(XpoDefault));
            Isolate.WhenCalled(() => XpoDefault.GetConnectionProvider("", AutoCreateOption.None)).WillReturn(_dataStoreSchemaExplorer);
            var classGenerator = Isolate.Fake.Instance<ClassGenerator>();
            var dbTable1 = new DBTable("tbl1");
            var dbColumn1 = new DBColumn { Name = "col1" };
            dbTable1.Columns.Add(dbColumn1);
            dbTable1.PrimaryKey = new DBPrimaryKey();
            var dbTable2 = new DBTable("tbl2");
            var dbColumn2 = new DBColumn { Name = "col2" };
            dbTable2.Columns.Add(dbColumn2);
            dbTable2.PrimaryKey = new DBPrimaryKey();
            Isolate.WhenCalled(() => _dataStoreSchemaExplorer.GetStorageTablesList()).WillReturn(new[] { "tbl1", "tbl2" });
            Isolate.WhenCalled(() => _dataStoreSchemaExplorer.GetStorageTables()).WillReturn(new[] { dbTable1, dbTable2 });
            _persistentClassInfo1 = ObjectSpace.CreateObject<PersistentClassInfo>();
            _persistentClassInfo2 = ObjectSpace.CreateObject<PersistentClassInfo>();
            Isolate.WhenCalled(() => classGenerator.CreateAll()).WillReturn(new List<ClassGeneratorInfo> { new ClassGeneratorInfo(_persistentClassInfo1, dbTable1), new ClassGeneratorInfo(_persistentClassInfo2, dbTable2) });
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();

            var classAtrributeGenerator = Isolate.Fake.Instance<ClassAtrributeGenerator>();
            Isolate.Swap.AllInstances<ClassAtrributeGenerator>().With(classAtrributeGenerator);
            _classPersistentAttributeInfos = new List<IPersistentAttributeInfo>();
            Isolate.WhenCalled(() => classAtrributeGenerator.Create()).WillReturn(_classPersistentAttributeInfos);

            var memberGenerator = Isolate.Fake.Instance<MemberGenerator>();
            Isolate.Swap.AllInstances<MemberGenerator>().With(memberGenerator);
            _persistentMemberInfo1 = ObjectSpace.CreateObject<PersistentCoreTypeMemberInfo>();
            _persistentMemberInfo2 = ObjectSpace.CreateObject<PersistentCoreTypeMemberInfo>();
            Isolate.WhenCalled(() => memberGenerator.Create()).WillReturn(new List<MemberGeneratorInfo> { new MemberGeneratorInfo(_persistentMemberInfo1, dbColumn1), new MemberGeneratorInfo(_persistentMemberInfo2, dbColumn2) });

            var memberAttributeGenerator = Isolate.Fake.Instance<MemberAttributeGenerator>();
            Isolate.Swap.AllInstances<MemberAttributeGenerator>().With(memberAttributeGenerator);
            _memberPersistentAttributeInfos = new List<IPersistentAttributeInfo> { ObjectSpace.CreateObject<PersistentAggregatedAttribute>() };
            Isolate.WhenCalled(() => ((IPersistentMemberInfo)_persistentClassInfo1.OwnMembers[0]).TypeAttributes).WillReturn(_memberPersistentAttributeInfos);
            Isolate.WhenCalled(() => ((IPersistentMemberInfo)_persistentClassInfo1.OwnMembers[1]).TypeAttributes).WillReturn(_memberPersistentAttributeInfos);
            Isolate.WhenCalled(() => ((IPersistentMemberInfo)_persistentClassInfo2.OwnMembers[0]).TypeAttributes).WillReturn(_memberPersistentAttributeInfos);
            Isolate.WhenCalled(() => ((IPersistentMemberInfo)_persistentClassInfo2.OwnMembers[1]).TypeAttributes).WillReturn(_memberPersistentAttributeInfos);
            _logonObject = ObjectSpace.CreateObject<LogonObject>();
        };

        Because of = () => new AssemblyGenerator(_logonObject, _persistentAssemblyInfo).Create();

        It should_generate_classes_for_all_tables = () => {
            _persistentClassInfos = _persistentAssemblyInfo.PersistentClassInfos;
            _persistentClassInfos.Count.ShouldEqual(2);
            _persistentClassInfos[0].ShouldEqual(_persistentClassInfo1);
            _persistentClassInfos[1].ShouldEqual(_persistentClassInfo2);
        };

        It should_generate_attributes_for_classes = () => {
            _persistentClassInfos[0].TypeAttributes.ShouldEqual(_classPersistentAttributeInfos);
            _persistentClassInfos[1].TypeAttributes.ShouldEqual(_classPersistentAttributeInfos);
        };

        //        It should_generate_members_for_all_columns = () => {
        //            _persistentClassInfos[1].OwnMembers.Count.ShouldEqual(2);
        //            _persistentClassInfos[1].OwnMembers[0].ShouldEqual(_persistentMemberInfo1);
        //            _persistentClassInfos[1].OwnMembers[1].ShouldEqual(_persistentMemberInfo2);
        //        };
        //
        //        It should_generate_attributes_for_members = () => {
        //            var persistentAttributeInfos = _persistentClassInfos[1].OwnMembers[1].TypeAttributes;
        //            persistentAttributeInfos.Count.ShouldEqual(1);
        //            persistentAttributeInfos[0].ShouldEqual(_memberPersistentAttributeInfos[0]);
        //        };
    }
}
