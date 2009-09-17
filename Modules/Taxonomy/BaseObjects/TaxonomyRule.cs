using System.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{

    [DefaultClassOptions]
    public class TaxonomyRule : eXpandLiteObject{
        private Taxonomy taxonomy;

        public TaxonomyRule(Session session) : base(session){}

        [Association(Associations.TaxonomyTaxonomyCreateObjectRules)]
        public Taxonomy Taxonomy
        {
            get
            {
                return taxonomy;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref taxonomy, value);
            }
        }




  
        private string _typeOfObject;
        
        
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

        private TaxonomyQuery taxonomyQuery;
        [Association(Associations.TaxonomyQueryTaxonomyTypeRules)]
        public TaxonomyQuery TaxonomyQuery
        {
            get
            {
                return taxonomyQuery;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref taxonomyQuery, value);
            }
        }

        private string propertyName;
        //TODO: Implement a MemberInfo-like-type property editor
        public string PropertyName
        {
            get
            {
                return propertyName;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref propertyName, value);
            }
        }
    }
}