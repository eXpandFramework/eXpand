using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [DefaultClassOptions]
    [DefaultProperty("Name")]
    [Serializable]
    public class Term : TermBase, ITerm {
        public Term(Session session) : base(session) {}
        public Term() {}

        [XmlIgnore]
        [Association(Associations.TermTaxonomizedInfos)]
        public XPCollection<TaxonomizedInfo> Infos {
            get { return GetCollection<TaxonomizedInfo>("Infos"); }
        }

        private StructuralTerm structuralTerm;
        protected Taxonomy parentTaxonomy;

        [Association(Associations.StructuralTermDerivedTerms)]
        public StructuralTerm StructuralTerm {
            get {
                return structuralTerm;
            }
            set {
                SetPropertyValue("StructuralTerm", ref structuralTerm, value);
            }
        }

        [Association(Associations.TaxonomyTerms)]
        [XmlIgnore]
        public Taxonomy Taxonomy {
            get { return taxonomy; }
            set { SetPropertyValue("Taxonomy", ref taxonomy, value); }
        }

        public static Term CreateTermFromStructure(StructuralTerm structuralTerm, Term parentTerm) {
            var term = new Term(structuralTerm.Session) {
                ParentTerm = parentTerm,
                Taxonomy = (Taxonomy)structuralTerm.Session.GetObject(structuralTerm.Taxonomy),
                StructuralTerm = structuralTerm
            };

            foreach (var enumerable in structuralTerm.Terms.Cast<StructuralTerm>().Where(sterm => sterm.TypeOfObject == null)) {
                var childTerm = CreateTermFromStructure(enumerable, term);
                childTerm.Key = enumerable.Key;
                childTerm.Name = enumerable.Name;
            }
            return term;
        }
    }
}