using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo.DB;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.WorldCreator.DBMapper;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.BaseImpl.PersistentMetaData;

namespace Xpand.Tests.Xpand.WorldCreator.Mapper {
    public class When_table_owner_has_a_persiste_attribute : With_In_Memory_DataStore {
        static DBTable _dbTable;
        static IPersistentClassInfo _persistentClassInfo;
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;

        Establish context = () => {
            _persistentClassInfo = XPObjectSpace.CreateObject<PersistentClassInfo>();
            _dbTable = new DBTable("test");
            Isolate.WhenCalled(() => _persistentClassInfo.TypeAttributes).WillReturn(new List<IPersistentAttributeInfo> { Isolate.Fake.Instance<IPersistentPersistentAttribute>() });
        };

        Because of = () => {
            _persistentAttributeInfos = new ClassAtrributeGenerator(new ClassGeneratorInfo(_persistentClassInfo, _dbTable), null).Create().ToList();
        };

        It should_not_return_any_attribute = () => _persistentAttributeInfos.OfType<IPersistentPersistentAttribute>().Count().ShouldEqual(0);
    }

    public class When_table_is_mapped : With_In_Memory_DataStore {
        static PersistentClassInfo _persistentClassInfo;
        static IPersistentPersistentAttribute _persistentPersistentAttribute;
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;

        static DBTable _dbTable;

        Establish context = () => {
            _persistentClassInfo = XPObjectSpace.CreateObject<PersistentClassInfo>();
            _dbTable = new DBTable("test");
        };

        Because of = () => {
            _persistentAttributeInfos = new ClassAtrributeGenerator(new ClassGeneratorInfo(_persistentClassInfo, _dbTable), null).Create().ToList();
        };

        It should_create_a_persistent_attribute = () => {
            _persistentPersistentAttribute =
                _persistentAttributeInfos.OfType<IPersistentPersistentAttribute>().FirstOrDefault();
            _persistentPersistentAttribute.ShouldNotBeNull();
        };

        It should_map_it_to_column_name = () => _persistentPersistentAttribute.MapTo.ShouldEqual(_dbTable.Name);
    }

}
