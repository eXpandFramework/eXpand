using System;
using System.Reflection;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    public class StructuralTerm : TermBase{
        private string typeOfObject;
        public StructuralTerm(Session session) : base(session) {}

        public StructuralTerm() {}

        [Size(SizeAttribute.Unlimited)]
        public string TypeOfObject{
            get { return typeOfObject; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref typeOfObject, value); }
        }

        [Association(Associations.StructuralTermDerivedTerms)]
        public XPCollection<Term> DerivedTerms{
            get { return GetCollection<Term>("DerivedTerms"); }
        }

        public void UpdateTypes(Type[] types){
            UpdateType(this, types, 0);
        }

        private static void UpdateType(StructuralTerm structuralTerm, Type[] types, int index){
            if (index < types.Length){
                structuralTerm.TypeOfObject = types[index].AssemblyQualifiedName;
                index++;
                if (structuralTerm.ParentTerm != null) UpdateType((StructuralTerm) structuralTerm.ParentTerm, types, index);
            }
        }
    }
}