using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.Base.Taxonomies;

namespace eXpand.Persistent.BaseImpl.Taxonomies {
    public class CustomTaxonomy : BaseTaxonomy {
        public CustomTaxonomy(Session session) : base(session) {}

        [Association, Aggregated]
        public XPCollection<ValueTerm> TermsCollection {
            get { return GetCollection<ValueTerm>("TermsCollection"); }
        }

        [Association, Aggregated]
        public XPCollection<StructuralTerm> StructuralTermsCollection {
            get { return GetCollection<StructuralTerm>("StructuralTermsCollection"); }
        }

        #region Overrides of BaseTaxonomy
        [Browsable(false)]
        public override IList<IValueTerm> Terms{
            get { return new ListConverter<IValueTerm, ValueTerm>(TermsCollection); }
        }

        [Browsable(false)]
        public override IList<IStructuralTerm> Structure{
            get { return new ListConverter<IStructuralTerm, StructuralTerm>(StructuralTermsCollection); }
        }

        public override IStructuralTerm AddStructuralTerm(string termPath, string termName, Type[] types){
            return TaxonomyManager.AddStructuralTerm<StructuralTerm>(this, Session, termPath, termName, types);
        }

        public override IValueTerm AddValueTerm(string termPath, string termName){
            return TaxonomyManager.AddValueTerm<ValueTerm>(this, Session, termPath, termName);
        }

        public override IValueTerm AddValueTerm(string termPath, string termName, IStructuralTerm structuralTerm) {
            return TaxonomyManager.AddValueTerm<ValueTerm>(this, Session, termPath, termName, structuralTerm);
        }
        #endregion
    }
}