using System;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.Persistent.BaseImpl;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable][DefaultClassOptions]
    public class BaseObjectInfo : PersistentKeyValuePair,ICategorizedItem {
        private TaxonomyBaseObject owner;
        public BaseObjectInfo(){}

        public BaseObjectInfo(Session session) : base(session) {}
        [Association("TermAssignmentKeys")]
        [XmlIgnore]
        public Term KeyTerm
        {
            get { return keyTerm; }
            set { SetPropertyValue("KeyTerm", ref keyTerm, value); }
        }
        private Term keyTerm;
        [Association("ObjectInfos")]
        [XmlIgnore]
        public TaxonomyBaseObject Owner {
            get { return owner; }
            set { SetPropertyValue("Owner", ref owner, value); }
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (keyTerm != null){
                Key = keyTerm.Key;
            }
        }

        
        public Term Category
        {
            get { return keyTerm; }
            set
            {
                keyTerm = value;
            }
        }
        ITreeNode ICategorizedItem.Category{
            get { return keyTerm; }
            set { keyTerm = (Term) value; }
        }
    }
}