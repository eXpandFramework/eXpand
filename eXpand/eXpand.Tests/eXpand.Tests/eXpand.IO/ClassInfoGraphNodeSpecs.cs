using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.PersistentTypesHelpers;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Persistent.BaseImpl.ImportExport;
using eXpand.Persistent.BaseImpl.PersistentMetaData;
using eXpand.Tests.eXpand.WorldCreator;
using Machine.Specifications;
using System.Linq;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.IO {
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph:With_Isolations
    {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () => {
            
            _serializationConfiguration = new SerializationConfiguration(unitOfWork){TypeToSerialize = customerType};
            
        };
        
        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);
        
        It should_add_all_properties_of_master_object_as_graph_nodes=() => {
            const int oid_and_User_and_orders_and_name_property_count = 4;
            _serializationConfiguration.SerializationGraph[0].Children.Count.ShouldEqual(oid_and_User_and_orders_and_name_property_count);
        };
        It should_exclude_refenced_Master_object_of_associated_collection=() => {
            const int oid_Property_count = 1;
            ClassInfoGraphNode classInfoGraphNode = _serializationConfiguration.SerializationGraph[0].Children.Where(node => node.Name == "Orders").Single();
            classInfoGraphNode.Children.Count.ShouldEqual(oid_Property_count);
        };

        It should_set_a_SerializeAsObject_strategy_for_all_associated_reference_members =
            () =>
            _serializationConfiguration.SerializationGraph[0].Children.Where(node => node.Name == "User").Single().
                SerializationStrategy.ShouldEqual(SerializationStrategy.SerializeAsObject);
    }
}