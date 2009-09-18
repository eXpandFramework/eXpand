using System;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [DefaultClassOptions]
    [Serializable]
    public class TermAssignment : BaseObjectInfo {
        private Term valueTerm;
        
        public TermAssignment(Session session) : base(session) {}


        [Association("TermAssignmentValues")]
        [XmlIgnore]
        public Term ValueTerm{
            get { return valueTerm; }
            set { SetPropertyValue("ValueTerm", ref valueTerm, value); }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (valueTerm != null){
                Value = valueTerm.FullPath;
            }
        }
    }
}