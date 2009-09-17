using System;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [DefaultClassOptions]
    [Serializable]
    public class TermAssignment : BaseObjectInfo {
        private Term valueTerm;
        private Term keyTerm;
        public TermAssignment(Session session) : base(session) {}

        [Association("TermAssignmentKeys")]
        [XmlIgnore]
        public Term KeyTerm {
            get { return keyTerm; }
            set { SetPropertyValue("KeyTerm", ref keyTerm, value); }
        }

        [Association("TermAssignmentValues")]
        [XmlIgnore]
        public Term ValueTerm{
            get { return valueTerm; }
            set { SetPropertyValue("ValueTerm", ref valueTerm, value); }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (keyTerm != null){
                Key = keyTerm.Key;
            }
            if (valueTerm != null){
                Value = valueTerm.FullPath;
            }
        }
    }
}