using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.IO.PersistentTypesHelpers;
using eXpand.Persistent.BaseImpl.ImportExport;
using eXpand.Tests.eXpand.WorldCreator;
using Machine.Specifications;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.IO {
    [Subject(typeof(SerializationConfiguration))]
    public class When_SerializationConfigurationGraph_ObjectToSerialize_Change:With_Isolations
    {
        static bool _generated;
        static ClassInfoGraphNodeBuilder _classInfoGraphNodeBuilder;
        static SerializationConfiguration _configuration;
        static ClassInfoGraphNode _classInfoGraphNode;

        Establish context = () => {
            new TestAppLication<SerializationConfiguration>().Setup(null, configuration => {
                _configuration = configuration;
                _classInfoGraphNode = new ClassInfoGraphNode(configuration.Session);
                configuration.SerializationGraph.Add(_classInfoGraphNode);
            }).WithArtiFacts(IOArtifacts).CreateDetailView().CreateFrame();
            _classInfoGraphNodeBuilder = Isolate.Fake.Instance<ClassInfoGraphNodeBuilder>();
            Isolate.Swap.NextInstance<ClassInfoGraphNodeBuilder>().With(_classInfoGraphNodeBuilder);
            Isolate.WhenCalled(() => _classInfoGraphNodeBuilder.Generate(_configuration)).DoInstead(callContext => _generated=true);
        };
        
        Because of = () => { _configuration.TypeToSerialize = typeof (User); };

        It should_delete_old_graph_node=() => _classInfoGraphNode.IsDeleted.ShouldBeTrue();
        It should_generate_the_graph_node_for_that_type=() => _generated.ShouldBeTrue();
    }
}