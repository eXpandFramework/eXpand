using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.ImportExport;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.ImportExport {
    [DefaultClassOptions]
    [NavigationItem("ImportExport")]
    [DefaultProperty("Name")]
    public class SerializationConfigurationGroup : XpandCustomObject, ISerializationConfigurationGroup {
        private string _name;

        public SerializationConfigurationGroup(Session session)
            : base(session) {
        }

        [RuleUniqueValue(null, DefaultContexts.Save)]
        [RuleRequiredField]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Association("SerializationConfigurationGroup-SerializationConfigurations")]
        [Aggregated]
        public XPCollection<SerializationConfiguration> SerializationConfigurations {
            get { return GetCollection<SerializationConfiguration>("SerializationConfigurations"); }
        }

        IList<ISerializationConfiguration> ISerializationConfigurationGroup.Configurations {
            get { return new ListConverter<ISerializationConfiguration, SerializationConfiguration>(SerializationConfigurations); }
        }

    }
}