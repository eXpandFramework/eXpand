using System.ComponentModel;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.Persistent.Base.Taxonomies;
using eXpand.Xpo;

namespace eXpand.Persistent.BaseImpl.Taxonomies{
    [DefaultClassOptions]
    public sealed class ValueTerm : BaseTerm, IValueTerm{
        private CustomTaxonomy _taxonomy;
        private ValueTerm parentValueTerm;
        private StructuralTerm structuralTerm;
        public ValueTerm(Session session) : base(session) {}

        [Association]
        public StructuralTerm StructuralTerm{
            get { return structuralTerm; }
            set { SetPropertyValue("StructuralTerm", ref structuralTerm, value); }
        }

        [Association, Aggregated]
        public XPCollection<ValueTerm> ValueTerms{
            get { return GetCollection<ValueTerm>("ValueTerms"); }
        }

        [Association]
        public ValueTerm ParentValueTerm{
            get { return parentValueTerm; }
            set { SetPropertyValue("ParentBaseTerm", ref parentValueTerm, value); }
        }

        [Association]
        public CustomTaxonomy Taxonomy{
            get { return _taxonomy; }
            set { SetPropertyValue("Taxonomy", ref _taxonomy, value); }
        }

        [Association, Aggregated]
        public XPCollection<ValueTermAssignment> ValueTermAssignments {
            get { return GetCollection<ValueTermAssignment>("ValueTermAssignments"); }
        }
        #region IValueTerm Members
        [Browsable(false)]
        public override IBaseTaxonomy BaseTaxonomy{
            get { return Taxonomy; }
            set { Taxonomy = (CustomTaxonomy) value; }
        }

        [Browsable(false)]
        public override ITreeNode Parent{
            get { return ParentValueTerm; }
            set { ParentValueTerm = (ValueTerm) value; }
        }

        [Browsable(false)]
        public override IBindingList Children{
            get { return ValueTerms; }
        }

        [Browsable(false)]
        IStructuralTerm IValueTerm.StructuralTerm{
            get { return structuralTerm; }
            set { structuralTerm = (StructuralTerm) value; }
        }
        #endregion
        public static ValueTerm CreateValueTermFromStructuralTerm(StructuralTerm structuralTerm, ValueTerm parentValueTerm){
            var term = new ValueTerm(structuralTerm.Session){
                                                                ParentValueTerm = parentValueTerm,
                                                                BaseTaxonomy = structuralTerm.BaseTaxonomy,
                                                                StructuralTerm = structuralTerm
                                                            };

            foreach (StructuralTerm enumerable in structuralTerm.StructuralTerms.Where(sterm => sterm.TypeOfObject == null)){
                ValueTerm childTerm = CreateValueTermFromStructuralTerm(enumerable, term);
                childTerm.Key = enumerable.Key;
                childTerm.Name = enumerable.Name;
                term.ValueTerms.Add(childTerm);
            }
            return term;
        }
    }
}