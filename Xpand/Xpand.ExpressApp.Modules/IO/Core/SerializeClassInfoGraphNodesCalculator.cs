using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.IO.Core {
    public class SerializeClassInfoGraphNodesCalculator {
        readonly ISerializationConfigurationGroup _serializationConfigurationGroup;
        private readonly IObjectSpace _objectSpace;

        public SerializeClassInfoGraphNodesCalculator(ISerializationConfigurationGroup serializationConfigurationGroup, IObjectSpace objectSpace){
            _serializationConfigurationGroup = serializationConfigurationGroup;
            _objectSpace = objectSpace;
        }

	    ISerializationConfiguration GetConfiguration(Type type) {
		    var configuration = _serializationConfigurationGroup.Configurations.FirstOrDefault(config => config.TypeToSerialize == type);

		    if (configuration == null) {
			    configuration = _objectSpace.Create<ISerializationConfiguration>();
			    configuration.SerializationConfigurationGroup = _serializationConfigurationGroup;
			    configuration.TypeToSerialize = type;
			    new ClassInfoGraphNodeBuilder().Generate(configuration);
		    }
		    return configuration;
	    }

		public IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes( object theObject, string typeName) {
            var type = ReflectionHelper.GetType(typeName);
            ISerializationConfiguration configuration = GetConfiguration(type);
            return GetSerializedClassInfoGraphNodes(configuration);
        }

        public IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(object baseObject) {
            var configuration = GetConfiguration(baseObject.GetType());
            return GetSerializedClassInfoGraphNodes(configuration);
        }

	    IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(ISerializationConfiguration serializationConfiguration) {
		    return serializationConfiguration.SerializationGraph.Where(
			    node => (node.NodeType == NodeType.Simple && node.SerializationStrategy != SerializationStrategy.DoNotSerialize) ||
			            node.NodeType != NodeType.Simple).OrderBy(graphNode => graphNode.NodeType);
	    }
    }
}