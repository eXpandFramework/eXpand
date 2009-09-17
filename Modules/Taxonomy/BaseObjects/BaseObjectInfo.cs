using System;
using System.Xml.Serialization;
using DevExpress.Xpo;
using eXpand.ExpressApp.Taxonomy.BaseObjects;
using eXpand.Persistent.BaseImpl;

namespace eXpand.Persistent.TaxonomyImpl{
    [Serializable]
    public class BaseObjectInfo : PersistentKeyValuePair {
        private TaxonomyBaseObject owner;
        public BaseObjectInfo(){}

        public BaseObjectInfo(Session session) : base(session) {}

        [Association("ObjectInfos")]
        [XmlIgnore]
        public TaxonomyBaseObject Owner{
            get { return owner; }
            set { SetPropertyValue("Owner", ref owner, value); }
        }
    }
}