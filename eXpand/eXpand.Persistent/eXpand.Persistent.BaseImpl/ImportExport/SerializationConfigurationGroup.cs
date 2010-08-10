using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.Persistent.BaseImpl.ImportExport {
    [DefaultClassOptions]
    [NavigationItem("ImportExport")]
    public class SerializationConfigurationGroup:BaseObject, ISerializationConfigurationGroup {
        private string _name;

        public SerializationConfigurationGroup(Session session) : base(session) {
        }

        [RuleUniqueValue(null,DefaultContexts.Save)]
        [RuleRequiredField]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Association("SerializationConfigurationGroup-SerializationConfigurations")]
        public XPCollection<SerializationConfiguration> SerializationConfigurations {
            get { return GetCollection<SerializationConfiguration>("SerializationConfigurations"); }
        }

        IList<ISerializationConfiguration> ISerializationConfigurationGroup.Configurations
        {
            get { return new ListConverter<ISerializationConfiguration, SerializationConfiguration>(SerializationConfigurations); }
        }

    }
}