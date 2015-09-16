using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.IO.PersistentTypesHelpers;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Utils.Helpers;
using Fasterflect;

namespace Xpand.ExpressApp.IO.Core {
    public class SerializeClassInfoGraphNodesCalculator {
        readonly ISerializationConfigurationGroup _serializationConfigurationGroup;

        public SerializeClassInfoGraphNodesCalculator(ISerializationConfigurationGroup serializationConfigurationGroup) {
            _serializationConfigurationGroup = serializationConfigurationGroup;
        }

        ISerializationConfiguration GetConfiguration(Session session, Type type) {
            var serializationConfigurationType = TypesInfo.Instance.SerializationConfigurationType;
            ISerializationConfiguration configuration;

            var findObject = _serializationConfigurationGroup.Configurations.FirstOrDefault(a => a.TypeToSerialize == type);

            if (findObject != null)
                configuration = (ISerializationConfiguration)findObject;
            else {
                configuration =
                    (ISerializationConfiguration)serializationConfigurationType.CreateInstance(session);
                configuration.SerializationConfigurationGroup = _serializationConfigurationGroup;
                configuration.TypeToSerialize = type;
                new ClassInfoGraphNodeBuilder().Generate(configuration);
            }
            return configuration;
        }

        public IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(XPBaseObject theObject, string typeName) {
            var type = ReflectionHelper.GetType(typeName);
            ISerializationConfiguration configuration = GetConfiguration(theObject.Session, type);
            return GetSerializedClassInfoGraphNodes(configuration);
        }

        public IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(XPBaseObject baseObject) {
            var configuration = GetConfiguration(baseObject.Session, baseObject.GetType());
            return GetSerializedClassInfoGraphNodes(configuration);
        }

        IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(ISerializationConfiguration serializationConfiguration) {
            return ((IEnumerable)((XPBaseObject)serializationConfiguration).GetMemberValue(serializationConfiguration.GetPropertyName(x => x.SerializationGraph))).
                OfType<IClassInfoGraphNode>().Where(
                    node => (node.NodeType == NodeType.Simple && node.SerializationStrategy != SerializationStrategy.DoNotSerialize) ||
                            node.NodeType != NodeType.Simple).OrderBy(graphNode => graphNode.NodeType);
        }

    }
}