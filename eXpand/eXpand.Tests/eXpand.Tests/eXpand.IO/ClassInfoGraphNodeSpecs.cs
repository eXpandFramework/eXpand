using DevExpress.Xpo;
using eXpand.ExpressApp.IO.PersistentTypesHelpers;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Persistent.BaseImpl.ImportExport;
using Machine.Specifications;
using System.Linq;

namespace eXpand.Tests.eXpand.IO {
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph : With_Customer_Orders_Serialization_Config
    {
        static XPCollection<ClassInfoGraphNode> _memberCategories;
        Because of = () => new ClassInfoGraphNodeBuilder().Generate(SerializationConfiguration);

        It should_add_all_properties_of_master_object_as_graph_nodes=() => {
            const int oid_and_User_and_orders_and_name_property_count = 4;
            SerializationConfiguration.SerializationGraph[0].Children.Count.ShouldEqual(oid_and_User_and_orders_and_name_property_count);
        };

        It should_exclude_refenced_Master_object_of_associated_collection=() => {
            const int oid_Property_count = 1;
            ClassInfoGraphNode memberCategory = SerializationConfiguration.SerializationGraph[0].Children.Where(node => node.Name == "Orders").Single();
            memberCategory.Children.Count.ShouldEqual(oid_Property_count);
        };

        It should_set_a_SerializeAsObject_strategy_for_all_associated_reference_members =
            () => {
                _memberCategories = SerializationConfiguration.SerializationGraph[0].Children;
                _memberCategories.Where(node => node.Name == "User").Single().
                    SerializationStrategy.ShouldEqual(SerializationStrategy.SerializeAsObject);
            };
        It should_set_a_SerializeAsObject_strategy_for_all_associated_collection_members =
            () => {
                _memberCategories = SerializationConfiguration.SerializationGraph[0].Children;
                _memberCategories.Where(node => node.Name == "Orders").Single().
                    SerializationStrategy.ShouldEqual(SerializationStrategy.SerializeAsObject);
            };

        It should_mark_as_key_the_key_property =
            () => _memberCategories.Where(node => node.Name == "oid").Single().Key.ShouldBeTrue();

        It should_mark_as_simple_all_properties_that_their_type_is_not_persistent=() => {
            const int oid_and_name = 2;
            _memberCategories.Where(node => node.NodeType == NodeType.Simple).Count().ShouldEqual(oid_and_name);
        };

        It should_mark_as_collection_all_associated_collections=() => {
            const int Orders = 1;
            _memberCategories.Where(node => node.NodeType == NodeType.Collection).Count().ShouldEqual(Orders);
        };
        It should_mark_as_object_all_properties_that_their_type_is_persistent=() => {
            const int user = 1;
            _memberCategories.Where(node => node.NodeType == NodeType.Object).Count().ShouldEqual(user);
        };
    }
}