using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.Persistent.BaseImpl.ImportExport {
    [DefaultProperty("TypeToSerialize")]
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "TypeNameToSerialize,SerializationConfigurationGroup")]
    public class SerializationConfiguration : XpandBaseCustomObject, ISerializationConfiguration {
        private Type _typeToSerialize;
        public SerializationConfiguration(Session session) : base(session) { }
        private SerializationConfigurationGroup _serializationConfigurationGroup;

        [Index(0)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        [VisibleInListView(true)]
        [RuleRequiredField]
        public Type TypeToSerialize {
            get => _typeToSerialize;
            set{
                SetPropertyValue("TypeToSerialize", ref _typeToSerialize, value);
                TypeNameToSerialize = value == null ? null : value.FullName;
            }
        }
        [Persistent]
        [Size(SizeAttribute.Unlimited)] protected string TypeNameToSerialize;
        [Association]
        public XPCollection<ClassInfoGraphNode> SerializationGraph => GetCollection<ClassInfoGraphNode>();

        [RuleRequiredField]
        [Association("SerializationConfigurationGroup-SerializationConfigurations")]
        [VisibleInDetailView(false)]
        public SerializationConfigurationGroup SerializationConfigurationGroup {
            get => _serializationConfigurationGroup;
            set => SetPropertyValue("SerializationConfigurationGroup", ref _serializationConfigurationGroup, value);
        }
        ISerializationConfigurationGroup ISerializationConfiguration.SerializationConfigurationGroup {
            get => SerializationConfigurationGroup;
            set => SerializationConfigurationGroup = value as SerializationConfigurationGroup;
        }

        IList<IClassInfoGraphNode> ISerializationConfiguration.SerializationGraph => new List<IClassInfoGraphNode>(SerializationGraph);
    }
}