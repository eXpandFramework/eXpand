using System;
using System.Xml.Serialization;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.BaseImpl{
    [Serializable]
    public abstract class PersistentKeyValuePair : eXpandLiteObject{
        private string key;

        private PersistentKeyValuePairStatus status;
        private string _value;
        protected PersistentKeyValuePair(Session session) : base(session) {}
        protected PersistentKeyValuePair() {}

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