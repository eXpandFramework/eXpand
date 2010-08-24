using System.Collections.Generic;
using DevExpress.Xpo;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Machine.Specifications;
using Microsoft.SqlServer.Management.Smo;
using TypeMock.ArrangeActAssert;
using System.Linq;

namespace eXpand.Tests.eXpand.WorldCreator.DbMapper
{
    [Subject(typeof(ExpressApp.WorldCreator.SqlDBMapper.DbMapper))]
    public class When_mapping_a_database_to_an_assembly:With_DataBase
    {
        static DataStoreLogonObject _dataStoreLogonObject;
        static PersistentAssemblyDataStoreAttributeInfo _persistentAssemblyDataStoreAttributeInfo;
        static PersistentClassInfo _persistentClassInfo2;
        static PersistentClassInfo _persistentClassInfo1;
        static PersistentAssemblyInfo _persistentAssemblyInfo;

        Establish context = () => {
            _database = Isolate.Fake.Instance<Database>();
            Table table1 = GetTable("table1","column1");
            Table table2 = GetTable("table2","column2");
            Isolate.WhenCalled(() => _database.Tables).WillReturnCollectionValuesOf(new List<Table>{table1,table2});
            _persistentAssemblyInfo = ObjectSpace.CreateObject<PersistentAssemblyInfo>();
            _dataStoreLogonObject = ObjectSpace.CreateObject<DataStoreLogonObject>();
            _dataStoreLogonObject.DataBase = ObjectSpace.CreateObject<DataBase>();
        };

        static Table GetTable(string name, string columnName) {
            var table1 = Isolate.Fake.Instance<Table>();
            table1.Name = name;
            var column = Isolate.Fake.Instance<Column>();
            Isolate.WhenCalled(() => column.InPrimaryKey).WillReturn(true);
            Isolate.WhenCalled(() => column.Parent).WillReturn(table1);
            column.DataType.SqlDataType=SqlDataType.Int;
            column.Name = columnName;
            Isolate.WhenCalled(() => table1.Columns).WillReturnCollectionValuesOf(new List<Column>{column});
            return table1;
        }

        Because of = () => new ExpressApp.WorldCreator.SqlDBMapper.DbMapper(ObjectSpace, _persistentAssemblyInfo, _dataStoreLogonObject).Map(_database);

        It should_create_classinfos_for_all_tables_in_the_assembly =
            () => {
                XPCollection<PersistentClassInfo> persistentClassInfos = _persistentAssemblyInfo.PersistentClassInfos;
                persistentClassInfos.Count.ShouldEqual(2);
                _persistentClassInfo1 = persistentClassInfos.First();
                _persistentClassInfo1.Name.ShouldEqual("table1");
                _persistentClassInfo2 = persistentClassInfos.Last();
                _persistentClassInfo2.Name.ShouldEqual("table2");
            };

        It should_create_memberinfos_for_each_classinfo = () => {
            _persistentClassInfo2.OwnMembers.Count.ShouldEqual(1);
            _persistentClassInfo2.OwnMembers.Where(info => info.Name=="column2").FirstOrDefault().ShouldNotBeNull();
            _persistentClassInfo1.OwnMembers.Count.ShouldEqual(1);
            _persistentClassInfo1.OwnMembers.Where(info => info.Name=="column1").FirstOrDefault().ShouldNotBeNull();
        };

        It should_have_a_persistent_datastore_attribute = () => {
            _persistentAssemblyDataStoreAttributeInfo = _persistentAssemblyInfo.Attributes.OfType<PersistentAssemblyDataStoreAttributeInfo>().SingleOrDefault();
            _persistentAssemblyDataStoreAttributeInfo.ShouldNotBeNull();            
        };

        It should_have_as_datastorelogon_object_the_one_that_points_to_the_mapped_database =
            () => _persistentAssemblyDataStoreAttributeInfo.DataStoreLogon.ShouldEqual(_dataStoreLogonObject);
    }

}
