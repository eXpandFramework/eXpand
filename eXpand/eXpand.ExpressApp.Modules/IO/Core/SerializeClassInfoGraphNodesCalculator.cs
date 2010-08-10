using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.IO.PersistentTypesHelpers;
using eXpand.Persistent.Base.ImportExport;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.IO.Core {
    public class SerializeClassInfoGraphNodesCalculator {
        readonly ISerializationConfigurationGroup _serializationConfigurationGroup;

        public SerializeClassInfoGraphNodesCalculator(ISerializationConfigurationGroup serializationConfigurationGroup) {
            _serializationConfigurationGroup = serializationConfigurationGroup;
        }

        ISerializationConfiguration GetConfiguration(Session session, Type type)
        {
            var serializationConfigurationType = TypesInfo.Instance.SerializationConfigurationType;
            ISerializationConfiguration configuration;
            var findObject = session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, serializationConfigurationType,
                                                SerializationConfigurationQuery.GetCriteria(type, _serializationConfigurationGroup));
            if (findObject != null)
                configuration = (ISerializationConfiguration)findObject;
            else
            {
                configuration =
                    (ISerializationConfiguration)ReflectionHelper.CreateObject(serializationConfigurationType, session);
                configuration.TypeToSerialize = type;
                new ClassInfoGraphNodeBuilder().Generate(configuration);
            }
            return configuration;
        }

        public IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(XPBaseObject theObject, string typeName)
        {
            var type = ReflectionHelper.GetType(typeName);
            ISerializationConfiguration configuration = GetConfiguration(theObject.Session, type);
            return GetSerializedClassInfoGraphNodes(configuration);
        }

        public IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(XPBaseObject baseObject, ISerializationConfiguration serializationConfiguration)
        {
            ISerializationConfiguration configuration = serializationConfiguration != null ? baseObject.GetType() == serializationConfiguration.TypeToSerialize ? serializationConfiguration
                                                                                                 : SerializationConfigurationQuery.Find(baseObject.Session, baseObject.GetType(), _serializationConfigurationGroup) : GetConfiguration(baseObject.Session, baseObject.GetType());
            return GetSerializedClassInfoGraphNodes(configuration);
        }

        IEnumerable<IClassInfoGraphNode> GetSerializedClassInfoGraphNodes(ISerializationConfiguration serializationConfiguration)
        {
            return ((IEnumerable)((XPBaseObject)serializationConfiguration).GetMemberValue(serializationConfiguration.GetPropertyName(x => x.SerializationGraph))).
                OfType<IClassInfoGraphNode>().Where(
                    node => (node.NodeType == NodeType.Simple && node.SerializationStrategy != SerializationStrategy.DoNotSerialize) ||
                            node.NodeType != NodeType.Simple).OrderBy(graphNode => graphNode.NodeType);
        }
        
    }
}