using System;
using System.Xml.Serialization;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [Serializable]
    public class BasicInfo : eXpandLiteObject {
        private BaseObject owner;
        private InfoValidity validity;
        public BasicInfo() {}

        // ReSharper disable InconsistentNaming
        private string _value;
        // ReSharper restore InconsistentNaming
        private string key;

        protected BasicInfo(Session session) : base(session) {}

        [Association(Associations.BaseObjectsBasicInfos)]
        [XmlIgnore]
        public BaseObject Owner{
            get { return owner; }
            set { SetPropertyValue("Owner", ref owner, value); }
        }

        [XmlAttribute]
        public string Key {
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        public string Value {
            get { return _value; }
            set { SetPropertyValue("Value", ref _value, value); }
        }
 
        [XmlAttribute]
        public InfoValidity Validity {
            get { return validity; }
            set { SetPropertyValue("Validity", ref validity, value); }
        }
    }
}