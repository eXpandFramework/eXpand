using System;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using eXpand.ExpressApp.Core;
using eXpand.Persistent.Base.ImportExport;
using System.ComponentModel;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl.ImportExport {
    [DefaultProperty("TypeToSerialize")]
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "TypeToSerialize,SerializationConfigurationGroup")]
    public class SerializationConfiguration : BaseObject, ISerializationConfiguration {
        private Type _typeToSerialize;
        public SerializationConfiguration(Session session) : base(session) { }
        private SerializationConfigurationGroup _serializationConfigurationGroup;
        
        
        [Index(0)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        [VisibleInListView(true)]
        public Type TypeToSerialize
        {
            get {
                return _typeToSerialize;
            }
            set { SetPropertyValue("TypeToSerialize", ref _typeToSerialize, value); }
        }
        [Association][Aggregated]
        public XPCollection<ClassInfoGraphNode> SerializationGraph
        {
            get { return GetCollection<ClassInfoGraphNode>("SerializationGraph"); }
        }
        [RuleRequiredField]
        [Association("SerializationConfigurationGroup-SerializationConfigurations")]
        public SerializationConfigurationGroup SerializationConfigurationGroup
        {
            get { return _serializationConfigurationGroup; }
            set { SetPropertyValue("SerializationConfigurationGroup", ref _serializationConfigurationGroup, value); }
        }
        ISerializationConfigurationGroup ISerializationConfiguration.SerializationConfigurationGroup {
            get { return SerializationConfigurationGroup; }
            set { SerializationConfigurationGroup=value as SerializationConfigurationGroup; }
        }

        IList<IClassInfoGraphNode> ISerializationConfiguration.SerializationGraph {
            get {
                return new ListConverter<IClassInfoGraphNode,ClassInfoGraphNode>(SerializationGraph);
            
            }
        }
    }
}