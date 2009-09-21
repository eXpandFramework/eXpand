using System;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [DefaultClassOptions]
    [DefaultProperty("Name")]
    [Serializable]
    public class Term : TermBase {
        public Term(Session session) : base(session) {}
        public Term() {}

        [XmlIgnore]
        [Association(Associations.TermTermAssignments)]
        public XPCollection<TermAssignment> Assignments{
            get { return GetCollection<TermAssignment>("Assignments"); }
        }

        private StructuralTerm structuralTerm;
        [Association(Associations.StructuralTermDerivedTerms)]
        public StructuralTerm StructuralTerm {
            get {
                return structuralTerm;
            }
            set {
                SetPropertyValue("StructuralTerm", ref structuralTerm, value);
            }
        }
    }
}