using System;
using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [DefaultClassOptions]
    [Serializable]
    public class StructuralTerm : Term {
        public StructuralTerm(Session session) : base(session){
        }
        [Association(Associations.TaxonomyStructureTerms)]
        public XPCollection<Taxonomy> Taxonomys
        {
            get
            {
                return GetCollection<Taxonomy>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
        public StructuralTerm(){
        }

        private string _typeOfObject;
        [Size(SizeAttribute.Unlimited)]
        public string TypeOfObject
        {
            get
            {
                return _typeOfObject;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _typeOfObject, value);
            }
        }
    }
}