using System;
using System.Reflection;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    public class StructuralTerm : TermBase, ITerm {
        private Type typeOfObject;
        public StructuralTerm(Session session) : base(session) {}

        

        [NonPersistent]
        [XmlIgnore]
        public Type TypeOfObject{
            get {
                if (!IsLoading && !IsSaving){
                    return savedType != null ? ReflectionHelper.GetType(savedType) : null;
                }
                return null;
            }
            set { savedType = value == null ? null : value.FullName; }
        }

        private string savedType;
        [Size(SizeAttribute.Unlimited)]
        [MemberDesignTimeVisibility(false)]
        public string SavedType {
            get {
                return savedType;
            }
            set {
                SetPropertyValue("SavedType", ref savedType, value);
            }
        }

        [Association(Associations.StructuralTermDerivedTerms)]
        [XmlIgnore]
        public XPCollection<Term> DerivedTerms{
            get { return GetCollection<Term>("DerivedTerms"); }
        }

        [Association(Associations.TaxonomyStructure)]
        [XmlIgnore][RuleRequiredField(null,DefaultContexts.Save)]
        public Taxonomy Taxonomy {
            get { return taxonomy; }
            set { SetPropertyValue("Taxonomy", ref taxonomy, value); }
        }

        public void UpdateTypes(Type[] types){
            UpdateType(this, types, 0);
        }

        private static void UpdateType(StructuralTerm structuralTerm, Type[] types, int index){
            if (index < types.Length){
                structuralTerm.TypeOfObject = types[index];
                index++;
                if (structuralTerm.ParentTerm != null) UpdateType((StructuralTerm) structuralTerm.ParentTerm, types, index);
            }
        }
    }
}