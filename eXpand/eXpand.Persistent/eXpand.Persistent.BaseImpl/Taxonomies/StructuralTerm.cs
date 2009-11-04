using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.Taxonomies;

namespace eXpand.Persistent.BaseImpl.Taxonomies{
    public sealed class StructuralTerm : BaseTerm, IStructuralTerm{
        private StructuralTerm parentStructuralTerm;
        private string savedType;
        private CustomTaxonomy taxonomy;
        public StructuralTerm(Session session) : base(session) {}

        [Association]
        public StructuralTerm ParentStructuralTerm{
            get { return parentStructuralTerm; }
            set { SetPropertyValue("ParentStructuralTerm", ref parentStructuralTerm, value); }
        }

        [Association]
        public XPCollection<StructuralTerm> StructuralTerms{
            get { return GetCollection<StructuralTerm>("StructuralTerms"); }
        }

        [Association]
        public XPCollection<ValueTerm> ValueTerms{
            get { return GetCollection<ValueTerm>("ValueTerms"); }
        }

        [Association]
        [RuleRequiredField(null, DefaultContexts.Save)]
        public CustomTaxonomy Taxonomy {
            get { return taxonomy; }
            set { SetPropertyValue("Taxonomy", ref taxonomy, value); }
        }

        [Browsable(false)]
        public override IBaseTaxonomy BaseTaxonomy{
            get { return Taxonomy; }
            set { Taxonomy = (CustomTaxonomy) value; }
        }

        [Browsable(false)]
        public override ITreeNode Parent{
            get { return ParentStructuralTerm; }
            set { ParentStructuralTerm = (StructuralTerm) value; }
        }

        [Browsable(false)]
        public override IBindingList Children{
            get { return StructuralTerms; }
        }
        #region IStructuralTerm Members
        [NonPersistent]
        public Type TypeOfObject{
            get{
                if (!IsLoading && !IsSaving){
                    return savedType != null ? ReflectionHelper.GetType(savedType) : null;
                }
                return null;
            }
            set { savedType = value == null ? null : value.FullName; }
        }

        [Size(SizeAttribute.Unlimited)]
        [MemberDesignTimeVisibility(false)]
        public string SavedType{
            get { return savedType; }
            set { SetPropertyValue("SavedType", ref savedType, value); }
        }

        IList<IValueTerm> IStructuralTerm.DerivedTerms {
            get { return new ListConverter<IValueTerm, ValueTerm>(ValueTerms); }
        }

        public void UpdateTypes(Type[] types){
            UpdateType(this, types, 0);
        }
        #endregion
        private static void UpdateType(StructuralTerm structuralTerm, Type[] types, int index){
            if (index < types.Length){
                structuralTerm.TypeOfObject = types[index];
                index++;
                if (structuralTerm.ParentTerm != null) UpdateType((StructuralTerm) structuralTerm.ParentTerm, types, index);
            }
        }
    }
}