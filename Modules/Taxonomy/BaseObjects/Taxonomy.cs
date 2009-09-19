using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    [DefaultClassOptions]
    public class Taxonomy : XPLiteObject, ITreeNode {
        private string description;
        private string group;
        private string key;
        private string _name;

        public Taxonomy(Session session) : base(session) {}
        [Association(Associations.TaxonomyTaxonomyCreateObjectRules)]
        [XmlIgnore]
        public XPCollection<TaxonomyRule> TaxonomyRules
        {
            get
            {
                return GetCollection<TaxonomyRule>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
        
        [Key]
        [XmlAttribute]
        public string Key{
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        [XmlAttribute]
        public string Group{
            get { return group; }
            set { SetPropertyValue("Group", ref group, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Description{
            get { return description; }
            set { SetPropertyValue("Description", ref description, value); }
        }

        [XmlAttribute]
        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Browsable(false)]
        [XmlIgnore]
        public ITreeNode Parent{
            get { return null; }
        }

        [Browsable(false)]
        [XmlIgnore]
        public IBindingList Children {
            get {
                return new XPCollection<Term>(Terms, new NullOperator("ParentTerm"));
            }
        }

        public Taxonomy() {}

        [Association("TaxonomyTerms"), Aggregated]
        [XmlIgnore]
        public XPCollection<Term> Terms {
            get {
                return GetCollection<Term>("Terms");
            }
        }

        AssociationXmlSerializationHelper termsSerializationHelper;
        [XmlArray("Terms")]
        [XmlArrayItem(typeof(Term))]
        [Browsable(false)]
        public AssociationXmlSerializationHelper TermsSerializationHelper {
            get {
                if (termsSerializationHelper == null)
                    termsSerializationHelper = new AssociationXmlSerializationHelper(Terms);
                return termsSerializationHelper;
            }
        }

        AssociationXmlSerializationHelper queriesSerializationHelper;
        [XmlArray("TaxonomyQueries")]
        [XmlArrayItem(typeof(TaxonomyQuery))]
        [Browsable(false)]
        public AssociationXmlSerializationHelper QueriesSerializationHelper {
            get {
                if (queriesSerializationHelper == null)
                    queriesSerializationHelper = new AssociationXmlSerializationHelper(Queries);
                return queriesSerializationHelper;
            }
        }

        [Association("TaxonomyQueries"), Aggregated]
        [XmlIgnore]
        public XPCollection<TaxonomyQuery> Queries {
            get {
                return GetCollection<TaxonomyQuery>("Queries");
            }
        }

        private string asXml;
        [Size(SizeAttribute.Unlimited)]
        [XmlIgnore]
        public string AsXml {
            get {
                return asXml;
            }
            set {
                SetPropertyValue("AsXml", ref asXml, value);
            }
        }
        
        [Action]
        public void RefreshXml(){
            var xs = new XmlSerializer(typeof(Taxonomy));
            TextWriter textWriter = new StringWriter();
            xs.Serialize(textWriter, this);
            asXml = textWriter.ToString();
            Save();
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName=="Key" && string.IsNullOrEmpty(key)){
                ReevaluatePaths();
            }
        }

        [Action]
        public void ReevaluatePaths(){
            foreach (var child in Children){
                ((Term) child).UpdateFullPath(true);
            }
            Save();
        }
        
        public TTerm GetTerm<TTerm>(string termPath, string caption) where TTerm:Term{
            var term = AddTerm<TTerm>(Session, termPath, caption);
            return term;
            
        }

        [Association(Associations.TaxonomyStructureTerms)]
        public XPCollection<StructuralTerm> StructureTerms
        {
            get
            {
                return GetCollection<StructuralTerm>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }

        protected virtual TTerm AddTerm<TTerm>(Session unitOfWork, string termPath, string name) where TTerm: Term
        {
            var term = getTerm<TTerm>(unitOfWork, termPath);
            if (term == null)
            {
                string[] pathSegments = GetPathSegments(termPath);

                if (pathSegments.Length == 0) {
                    throw new UserFriendlyException(new ArgumentException(string.Format("Invalid path: {0}", termPath)));
                }
                if (pathSegments.Length == 1)
                {
                    return null;
                }
                
                term = (TTerm) Activator.CreateInstance(typeof (TTerm), unitOfWork);
                term.Key = pathSegments.Last();
                term.Caption = string.IsNullOrEmpty(name) ? pathSegments.Last() : name;
                

                string format = string.Format("/{0}", pathSegments.Last());
                term.ParentTerm = AddTerm<TTerm>(unitOfWork, termPath.Replace(format, string.Empty), string.Empty);
                if (term.Parent == null){
                    term.Taxonomy = getTaxonomy(unitOfWork);
                    term.UpdateFullPath(false);
                }
            }
            return term;
        }

        private Taxonomy getTaxonomy(Session unitOfWork){
            return unitOfWork.FindObject<Taxonomy>(PersistentCriteriaEvaluationBehavior.InTransaction,x=>x.Key == Key);
        }

        private TTerm getTerm<TTerm>(Session unitOfWork, string termPath)where TTerm:Term{
            return unitOfWork.FindObject<TTerm>(PersistentCriteriaEvaluationBehavior.InTransaction, t=>t.FullPath==termPath);
        }

        public string[] GetPathSegments(string termPath) {
            return termPath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetPathSegments(string termPath, Taxonomy taxonomy) {
            return taxonomy.GetPathSegments(termPath);
        }
    }
}