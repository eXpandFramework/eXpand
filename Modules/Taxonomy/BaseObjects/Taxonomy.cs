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
        private string pathSeparator;

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
        
        public Term GetTerm(string termPath, string caption){
            using (NestedUnitOfWork unitOfWork = Session.BeginNestedUnitOfWork()){
                Term term = AddTerm(unitOfWork, termPath, caption);
                unitOfWork.CommitChanges();
                return term;
            }
        }

        //public Term GetTerm(string termPath, string caption){
        //    Term term;
        //    using (NestedUnitOfWork unitOfWork = Session.BeginNestedUnitOfWork()){
        //        term = getTerm(unitOfWork, termPath);
        //        if (term== null)
        //        {
        //            string last = GetPathSegments(termPath).Last();
        //            term = new Term(unitOfWork){Key = last,Caption = caption,Taxonomy = (Taxonomy) unitOfWork.GetObject(this)};
        //            string substring = termPath.Substring(0,termPath.LastIndexOf("/"));
        //            AddTerm(unitOfWork, substring, caption, term,0);
        //            unitOfWork.CommitChanges();
        //        }
        //    }
        //    return term;
        //}

        //private void AddTerm(NestedUnitOfWork work, string termPath, string caption, Term term1,int index){
        //    Term term2 = getTerm(work, termPath);
        //    if (term2== null)
        //    {
                
        //        if (termPath.IndexOf("/")>-1)
        //        {
        //            index++;
        //            string[] segments = GetPathSegments(termPath);
                    
        //            var term = new Term(work) { Caption = caption, Taxonomy = (Taxonomy)work.GetObject(this), Key = segments[segments.Count()-index] };
        //            term1.ParentTerm = term;
                    
        //            string substring = termPath.Substring(0, termPath.LastIndexOf("/"));
        //            AddTerm(work, substring, caption, term, index);
                        
                    
        //        }
        //    }
        //}


        protected Term AddTerm(NestedUnitOfWork unitOfWork, string termPath, string name)
        {
            var term = getTerm(unitOfWork, termPath);
            if (term == null)
            {
                string[] pathSegments = GetPathSegments(termPath);

                if (pathSegments.Length == 0) {
                    throw new UserFriendlyException(new ArgumentException(string.Format("Invalid path: {0}", termPath)));
                }
                if (pathSegments.Length == 1)`
                {
                    return null;
                }
                term = new Term(unitOfWork){
                                               Key = pathSegments.Last(),
                                               Caption =
                                                   string.IsNullOrEmpty(name) ? "(" + pathSegments.Last() + ")" : name,
                                               Taxonomy = (Taxonomy) unitOfWork.GetObject(this)
                                           };
                term.ParentTerm = AddTerm(unitOfWork, termPath.Replace(string.Format("/{0}", pathSegments.Last()), string.Empty), pathSegments.Last());
                if (term.Parent == null) term.UpdateFullPath(false);
            }
            return term;
        }

        private Term getTerm(NestedUnitOfWork unitOfWork, string termPath){
            return unitOfWork.FindObject<Term>(t=>t.FullPath==termPath);
        }

        public string[] GetPathSegments(string termPath) {
            return termPath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetPathSegments(string termPath, Taxonomy taxonomy) {
            return taxonomy.GetPathSegments(termPath);
        }
    }
}