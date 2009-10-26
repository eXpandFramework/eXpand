using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;
using eXpand.Xpo;


namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    public class TaxonomyQuery : eXpandLiteObject {
        private string criteriaOnProperties;
        private string criteriaOnTerms;
        private string desrciption;
        private string name;
        private Taxonomy taxonomy;
        public TaxonomyQuery(Session session) : base(session) {}
        public TaxonomyQuery() {}
        
        [Association(Associations.TaxonomyQueryTaxonomyTypeRules)]
        [XmlIgnore]
        public XPCollection<TaxonomyRule> TaxonomyTypeRules
        {
            get
            {
                return GetCollection<TaxonomyRule>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Type CriteriaOnTermsType{
            get { return typeof (Term); }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Type CriteriaOnPropertiesType{
            get { return typeof (PersistentKeyValuePair); }
        }

        [CriteriaObjectTypeMember("CriteriaOnTermsType")]
        public string CriteriaOnTerms{
            get { return criteriaOnTerms; }
            set { SetPropertyValue("CriteriaOnTerms", ref criteriaOnTerms, value); }
        }

        [CriteriaObjectTypeMember("CriteriaOnPropertiesType")]
        public string CriteriaOnProperties{
            get { return criteriaOnProperties; }
            set { SetPropertyValue("CriteriaOnProperties", ref criteriaOnProperties, value); }
        }

        [XmlAttribute]
        public string Name{
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Desrciption{
            get { return desrciption; }
            set { SetPropertyValue("Desrciption", ref desrciption, value); }
        }

        [Association("TaxonomyQueries")]
        [XmlIgnore]
        public Taxonomy Taxonomy{
            get { return taxonomy; }
            set { SetPropertyValue("Taxonomy", ref taxonomy, value); }
        }

        public CriteriaOperator ParseCriteria(params object[] args){
            return new GroupOperator(GroupOperatorType.And, new[]{
                                                                     new BinaryOperator("Taxonomy.Term", Taxonomy.Key),
                                                                     CriteriaOperator.Parse(PrepareCriteriaString(CriteriaOnTerms, args)),
                                                                     new ContainsOperator("KeyValuePairs", CriteriaOperator.Parse(PrepareCriteriaString(CriteriaOnProperties, args)))
                                                                 }

                );
        }

        private string PrepareCriteriaString(string criteriaString, object[] args){
            //string preparedString = 
            if (args.Length>0)
                return string.Format(criteriaString, args);
            return criteriaString;
        }
    }
}