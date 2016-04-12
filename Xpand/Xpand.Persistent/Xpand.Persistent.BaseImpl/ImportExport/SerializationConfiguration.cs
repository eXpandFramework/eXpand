﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General.ValueConverters;
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
            get {
                return _typeToSerialize;
            }
            set{
                SetPropertyValue("TypeToSerialize", ref _typeToSerialize, value);
                TypeNameToSerialize = value == null ? null : value.FullName;
            }
        }
        [Persistent]
        [Size(SizeAttribute.Unlimited)] protected string TypeNameToSerialize;
        [Association]
        [Aggregated]
        public XPCollection<ClassInfoGraphNode> SerializationGraph {
            get { return GetCollection<ClassInfoGraphNode>("SerializationGraph"); }
        }
        [RuleRequiredField]
        [Association("SerializationConfigurationGroup-SerializationConfigurations")]
        public SerializationConfigurationGroup SerializationConfigurationGroup {
            get { return _serializationConfigurationGroup; }
            set { SetPropertyValue("SerializationConfigurationGroup", ref _serializationConfigurationGroup, value); }
        }
        ISerializationConfigurationGroup ISerializationConfiguration.SerializationConfigurationGroup {
            get { return SerializationConfigurationGroup; }
            set { SerializationConfigurationGroup = value as SerializationConfigurationGroup; }
        }

        IList<IClassInfoGraphNode> ISerializationConfiguration.SerializationGraph {
            get {
                return new ListConverter<IClassInfoGraphNode, ClassInfoGraphNode>(SerializationGraph);

            }
        }
    }
}