using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    [DefaultClassOptions]
    public class Taxonomy : XPLiteObject{
        //public const string DefaultStructuralRootNodeKey = "Structure";
        //public const string DefaultBrowsableViewRootNodeKey = "BrowsableView";
        private string description;
        private bool forceReevaluationOfPaths;
        private string group;
        private string key;
        private string name;
        //private AssociationXmlSerializationHelper valueTreesSerializationHelper;

        public Taxonomy(Session session) : base(session) {}
        public Taxonomy() {}

        [Key]
        [XmlAttribute]
        public string Key{
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        [XmlAttribute]
        public string Name{
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
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

        [Association(Associations.TaxonomyTerms), Aggregated]
        [XmlIgnore]
        [Browsable(false)]
        public XPCollection<TermBase> Terms{
            get { return GetCollection<TermBase>("Terms"); }
        }
        #region Queries and Rules
        //[Association(Associations.TaxonomyTaxonomyCreateObjectRules)]
        //[XmlIgnore]
        //public XPCollection<TaxonomyRule> TaxonomyRules
        //{
        //    get
        //    {
        //        return GetCollection<TaxonomyRule>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
        //    }
        //}

        //AssociationXmlSerializationHelper queriesSerializationHelper;
        //[XmlArray("TaxonomyQueries")]
        //[XmlArrayItem(typeof(TaxonomyQuery))]
        //[Browsable(false)]
        //public AssociationXmlSerializationHelper QueriesSerializationHelper {
        //    get {
        //        if (queriesSerializationHelper == null)
        //            queriesSerializationHelper = new AssociationXmlSerializationHelper(Queries);
        //        return queriesSerializationHelper;
        //    }
        //}

        //[Association("TaxonomyQueries"), Aggregated]
        //[XmlIgnore]
        //public XPCollection<TaxonomyQuery> Queries {
        //    get {
        //        return GetCollection<TaxonomyQuery>("Queries");
        //    }
        //}
        #endregion
        //[XmlIgnore]
        //public XPCollection<StructuralTerm> Structure {
        //    get{
        //        if (!IsLoading && !IsSaving){
        //            return new XPCollection<StructuralTerm>(Session, Terms.OfType<StructuralTerm>());
        //        }
        //        return null;
        //    }
        //}

        //[XmlIgnore]
        //public XPCollection<Term> ValueTrees{
        //    get{
        //        if (!IsLoading && !IsSaving) {
        //            return new XPCollection<Term>(Session, Terms.OfType<Term>());
        //        }
        //        return null;
        //    }
        //}

        //[XmlArray("ValueTrees")]
        //[XmlArrayItem(typeof (Term))]
        //[Browsable(false)]
        //public AssociationXmlSerializationHelper ValueTreesSerializationHelper{
        //    get{
        //        if (valueTreesSerializationHelper == null)
        //            valueTreesSerializationHelper = new AssociationXmlSerializationHelper(ValueTrees);
        //        return valueTreesSerializationHelper;
        //    }
        //}

        //[XmlArray("Structure")]
        //[XmlArrayItem(typeof(StructuralTerm))]
        //[Browsable(false)]
        //public AssociationXmlSerializationHelper StructureSerializationHelper {
        //    get {
        //        if (structureSerializationHelper == null)
        //            structureSerializationHelper = new AssociationXmlSerializationHelper(Structure);
        //        return structureSerializationHelper;
        //    }
        //}

        protected override void OnChanged(string propertyName, object oldValue, object newValue){
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "Key" && !string.IsNullOrEmpty(key) & !IsLoading){
                forceReevaluationOfPaths = true;
            }
        }

        protected override void OnSaving(){
            if (forceReevaluationOfPaths){
                ReevaluatePaths();
            }
        }

        public TTerm GetTerm<TTerm>(string termPath, string termName) where TTerm : TermBase{
            termPath = termPath.StartsWith("/") ? string.Format("{0}{1}", key, termPath) : termPath;
            var term = AddTerm<TTerm>(Session, termPath, termName);
            return term; 
        }

        public TTerm GetTerm<TTerm>(string termPath) where TTerm : TermBase{
            return GetTerm<TTerm>(termPath);
        }

        //public StructuralTerm GetStructure(string termPath, Type[] types) {
        //    
        //    var term = AddTerm<StructuralTerm>(Session, string.Format("{0}/{1}{2}", key, DefaultStructuralRootNodeKey, termPath));
        //    term.UpdateTypes(types);
        //    return term;
        //}

        //public Term GetBrowsableTerm(string termPath) {
        //    return GetBrowsableTerm(termPath, string.Empty);
        //}

        //public Term GetBrowsableTerm(string termPath, string termName) {
        //    termPath = !termPath.StartsWith("/") ? string.Format("/{0}", termPath) : termPath;
        //    var term = AddTerm<Term>(Session, string.Format("{0}/{1}{2}", key, DefaultBrowsableViewRootNodeKey, termPath), termName);
        //    return term;
        //}

        protected virtual TTerm AddTerm<TTerm>(Session unitOfWork, string termPath) where TTerm : TermBase{
            return AddTerm<TTerm>(unitOfWork, termPath, string.Empty);
        }

        protected virtual TTerm AddTerm<TTerm>(Session unitOfWork, string termPath, string termName) where TTerm : TermBase{
            var term = GetTerm<TTerm>(unitOfWork, termPath);
            if (term == null){
                string[] pathSegments = GetPathSegments(termPath);

                if (pathSegments.Length == 0){
                    throw new UserFriendlyException(new ArgumentException(string.Format("Invalid path: {0}", termPath)));
                }
                if (pathSegments.Length == 1){
                    return null;
                }

                term = (TTerm) Activator.CreateInstance(typeof (TTerm), unitOfWork);
                term.Key = pathSegments.Last();
                term.Name = string.IsNullOrEmpty(termName) ? pathSegments.Last() : termName;

                string format = string.Format("/{0}", pathSegments.Last());
                term.ParentTerm = AddTerm<TTerm>(unitOfWork, termPath.Replace(format, string.Empty), string.Empty);
                term.Taxonomy = (Taxonomy) unitOfWork.GetObject(this);
                if (term.Taxonomy == null){
                    term.Taxonomy = this;
                }
                term.UpdateFullPath(false);
            }
            return term;
        }

        public static Taxonomy GetTaxonomy(Session unitOfWork, string taxonomyKey){
            return unitOfWork.FindObject<Taxonomy>(PersistentCriteriaEvaluationBehavior.InTransaction, x => x.Key == taxonomyKey);
        }

        private static TTerm GetTerm<TTerm>(Session unitOfWork, string termPath) where TTerm : TermBase{
            return unitOfWork.FindObject<TTerm>(PersistentCriteriaEvaluationBehavior.InTransaction, t => t.FullPath == termPath);
        }

        public string[] GetPathSegments(string termPath){
            return termPath.Split(new[]{"/"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] GetPathSegments(string termPath, Taxonomy taxonomy){
            return taxonomy.GetPathSegments(termPath);
        }
        #region Actions 
        private string asXml;
        ///private AssociationXmlSerializationHelper structureSerializationHelper;

        [Size(SizeAttribute.Unlimited)]
        [XmlIgnore]
        public string AsXml{
            get { return asXml; }
            set { SetPropertyValue("AsXml", ref asXml, value); }
        }

        [Action]
        public void ReevaluatePaths(){
            foreach (TermBase child in Terms){
                child.UpdateFullPath(true);
            }
            forceReevaluationOfPaths = false;
        }

        [Action]
        public void RefreshXml(){
            var xs = new XmlSerializer(typeof (Taxonomy));
            TextWriter textWriter = new StringWriter();
            xs.Serialize(textWriter, this);
            asXml = textWriter.ToString();
        }
        #endregion
    }
}