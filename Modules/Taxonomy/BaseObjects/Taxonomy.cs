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
        public XPCollection<Term> Terms {
            get { return GetCollection<Term>("Terms"); }
        }

        [Association(Associations.TaxonomyStructure), Aggregated]
        public XPCollection<StructuralTerm> Structure {
            get {
                return GetCollection<StructuralTerm>("Structure");
            }
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

        public Term AddTerm(Session unitOfWork, string termPath, string termName){
            return AddTerm<Term>(unitOfWork, StructurePath(termPath), termName);
        }

        public StructuralTerm AddStructure(Session unitOfWork, string termPath, string termName, Type[] types){
            var term = AddTerm<StructuralTerm>(unitOfWork, StructurePath(termPath), termName);
            term.UpdateTypes(types);
            return term;
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
                string parentPath = termPath.Substring(0, termPath.LastIndexOf("/"));
                term.ParentTerm = AddTerm<TTerm>(unitOfWork, parentPath, string.Empty);
                ((ITerm) term).Taxonomy = (Taxonomy) unitOfWork.GetObject(this);
                if (((ITerm)term).Taxonomy == null) {
                    ((ITerm)term).Taxonomy = this;
                }
                term.UpdateFullPath(false);
            }
            return term;
        }

        private string StructurePath(string termPath){
            return string.Format("{0}{1}"
                                 , key
                                 , !termPath.StartsWith("/")
                                       ? string.Format("/{0}", termPath)
                                       : termPath);
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