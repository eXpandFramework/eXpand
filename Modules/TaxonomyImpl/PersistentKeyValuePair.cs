using System;
using System.Xml.Serialization;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    public interface IPersistentProperty {
        [XmlAttribute]
        string Key { get; set; }

        [XmlAttribute]
        string Value { get; set; }

        [XmlAttribute]
        PersistentKeyValuePairStatus Status { get; set; }
    }

    [Serializable]
    public class PersistentKeyValuePair : eXpandLiteObject, IPersistentProperty{
        private string key;

        private PersistentKeyValuePairStatus status;
// ReSharper disable InconsistentNaming
        private string _value;
// ReSharper restore InconsistentNaming
        public PersistentKeyValuePair(Session session) : base(session) {}
        public PersistentKeyValuePair() {}

        [XmlAttribute]
        public string Key{
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        [XmlAttribute]
        public string Value{
            get { return _value; }
            set { SetPropertyValue("Value", ref _value, value); }
        }

        [XmlAttribute]
        public PersistentKeyValuePairStatus Status{
            get { return status; }
            set { SetPropertyValue("Status", ref status, value); }
        }
    }
}