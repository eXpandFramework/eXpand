using System;
using System.Xml.Serialization;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [Serializable]
    public class PersistentKeyValuePair : BaseObjectInfo {
// ReSharper disable InconsistentNaming
        private string _value;
// ReSharper restore InconsistentNaming
        private string key;

        public PersistentKeyValuePair(Session session) : base(session) {}
        public PersistentKeyValuePair() {}

        public override string Key{
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        public override string Value{
            get { return _value; }
            set { SetPropertyValue("Value", ref _value, value); }
        }
    }
}