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
        

        private Taxonomy structureTaxonomy;
        [Association(Associations.TaxonomyStructureTerms)]
        public Taxonomy StructureTaxonomy
        {
            get
            {
                return structureTaxonomy;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref structureTaxonomy, value);
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

        public void UpdateTypes(Type[] types){
            updateType(this, types, 0);
        }

        private void updateType(StructuralTerm structuralTerm, Type[] types, int index){
            if (index<types.Length){
                structuralTerm.TypeOfObject = types[index].AssemblyQualifiedName;
                structuralTerm.Save();
                index++;
                if (structuralTerm.ParentTerm != null) updateType((StructuralTerm) structuralTerm.ParentTerm,types,index);
            }
        }

        [Association(Associations.StructuralTermObjectInfos)]
        public XPCollection<TaxonomyBaseObjectInfo> ObjectInfos
        {
            get
            {
                return GetCollection<TaxonomyBaseObjectInfo>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
    }
}