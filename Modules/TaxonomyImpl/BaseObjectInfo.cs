using System;
using System.Xml.Serialization;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [Serializable]
    public abstract class BaseObjectInfo : eXpandLiteObject, IPersistentKeyValuePair{
        private BaseObject owner;
        private BaseObjectInfoValidity validity;
        protected BaseObjectInfo() {}

        protected BaseObjectInfo(Session session) : base(session) {}

        [Association("ObjectInfos")]
        [XmlIgnore]
        public BaseObject Owner{
            get { return owner; }
            set { SetPropertyValue("Owner", ref owner, value); }
        }
        #region IPersistentKeyValuePair Members
        [XmlAnyAttribute]
        public abstract string Key { get; set; }
        [XmlAnyAttribute]
        public abstract string Value { get; set; }

        [XmlAnyAttribute]
        public BaseObjectInfoValidity Validity{
            get { return validity; }
            set { SetPropertyValue("Validity", ref validity, value); }
        }
        #endregion
    }
}