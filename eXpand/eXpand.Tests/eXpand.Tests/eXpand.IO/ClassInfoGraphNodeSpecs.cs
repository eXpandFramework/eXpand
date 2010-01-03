using System.IO;
using System.Windows.Forms;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.PersistentTypesHelpers;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Persistent.BaseImpl.ImportExport;
using eXpand.Tests.eXpand.WorldCreator;
using Machine.Specifications;
using System.Linq;

namespace eXpand.Tests.eXpand.IO {
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_creating_a_graph : With_Customer_Orders
    {
        static SerializationConfiguration _serializationConfiguration;
        static XPCollection<ClassInfoGraphNode> _memberCategories;
        Establish context = () => { _serializationConfiguration = new SerializationConfiguration(ObjectSpace.Session) { TypeToSerialize = CustomerType }; };
        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_add_all_properties_of_master_object_as_graph_nodes=() => {
            const int oid_and_User_and_orders_and_name_property_count = 4;
            _serializationConfiguration.SerializationGraph[0].Children.Count.ShouldEqual(oid_and_User_and_orders_and_name_property_count);
        };

        It should_exclude_refenced_Master_object_of_associated_collection=() => {
            const int oid_Property_count = 1;
            ClassInfoGraphNode memberCategory = _serializationConfiguration.SerializationGraph[0].Children.Where(node => node.Name == "Orders").Single();
            memberCategory.Children.Count.ShouldEqual(oid_Property_count);
        };

        It should_set_a_SerializeAsObject_strategy_for_all_associated_reference_members =
            () => {
                _memberCategories = _serializationConfiguration.SerializationGraph[0].Children;
                _memberCategories.Where(node => node.Name == "User").Single().
                    SerializationStrategy.ShouldEqual(SerializationStrategy.SerializeAsObject);
            };

        It should_set_a_SerializeAsObject_strategy_for_all_associated_collection_members =
            () => {
                _memberCategories = _serializationConfiguration.SerializationGraph[0].Children;
                _memberCategories.Where(node => node.Name == "Orders").Single().
                    SerializationStrategy.ShouldEqual(SerializationStrategy.SerializeAsObject);
            };

        It should_mark_as_key_the_key_property =
            () => _memberCategories.Where(node => node.Name == "oid").Single().Key.ShouldBeTrue();

        It should_mark_as_natural_key_the_key_property = () => _memberCategories.Where(node => node.Name == "oid").Single().NaturalKey.ShouldBeTrue();
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
    [Subject(typeof(ClassInfoGraphNode))]
    public class When_generating_graph_for_self_reference_object:With_Isolations {
        static SerializationConfiguration _serializationConfiguration;

        Establish context = () =>
        {
            var artifactHandler = new TestAppLication<ClassInfoGraphNode>().Setup();
            var objectSpace = artifactHandler.ObjectSpace;
            var persistentAssemblyBuilder = PersistentAssemblyBuilder.BuildAssembly(objectSpace, GetUniqueAssemblyName());
            var classHandler = persistentAssemblyBuilder.CreateClasses(new[] { "CustomerSelfRef" });
            var persistentClassInfo = persistentAssemblyBuilder.PersistentAssemblyInfo.PersistentClassInfos[0];
            classHandler.CreateRefenenceMember(persistentClassInfo, "Parent",persistentClassInfo);
            classHandler.CreateCollectionMember(persistentClassInfo, "Collection",persistentClassInfo);
            
            artifactHandler.ObjectSpace.CommitChanges();
            var compileModule = new CompileEngine().CompileModule(persistentAssemblyBuilder,Path.GetDirectoryName(Application.ExecutablePath));
            var customerType = compileModule.Assembly.GetTypes().Where(type => type.Name == "CustomerSelfRef").Single();
            _serializationConfiguration = new SerializationConfiguration(artifactHandler.UnitOfWork) { TypeToSerialize = customerType };
        };

        Because of = () => new ClassInfoGraphNodeBuilder().Generate(_serializationConfiguration);

        It should_create_an_object_node_with_DoNotSerialize_strategy =
            () =>
            _serializationConfiguration.SerializationGraph[0].Children.Where(
                node =>
                node.NodeType == NodeType.Object && node.SerializationStrategy == SerializationStrategy.DoNotSerialize).
                FirstOrDefault().ShouldNotBeNull();

        It should_create_an_collection_node_with_DoNotSerialize_strategy = () => _serializationConfiguration.SerializationGraph[0].Children.Where(
                                                                                     node =>
                                                                                     node.NodeType == NodeType.Collection && node.SerializationStrategy == SerializationStrategy.DoNotSerialize).
                                                                                     FirstOrDefault().ShouldNotBeNull();
    }
}