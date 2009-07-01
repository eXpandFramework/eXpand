using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.Interfaces;


namespace eXpand.Persistent.BaseImpl
{
    [DefaultClassOptions]
    public class Taxonomy : BaseObject, INamedObject, ITaggedObject, IIndexedObject{
        private string group;
        private string indexer;
        private string key;

        private string name;
        private string pathSeparator;

        private object tag;
        public Taxonomy(Session session) : base(session) {}

        public override void AfterConstruction(){
            PathSeparator = ":";
        }

        [Association("Taxonomy-Terms"), Aggregated]
        public XPCollection<Term> Terms {
            get { return GetCollection<Term>("Terms"); }
        }

        public string PathSeparator{
            get { return pathSeparator; }
            set { SetPropertyValue("PathSeparator", ref pathSeparator, value); }
        }

        public string Group {
            get { return group; }
            set { SetPropertyValue("Group", ref group, value); }
        }

        #region IIndexedObject Members
        public string Indexer{
            get { return indexer; }
            set { SetPropertyValue("Indexer", ref indexer, value); }
        }

        public string Key{
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }
        #endregion
        #region INamedObject Members
        public string Name{
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }
        #endregion
        #region ITaggedObject Members
        [Browsable(false)]
        public object Tag{
            get { return tag; }
            set { SetPropertyValue("Tag", ref tag, value); }
        }
        #endregion

        private string itemSeparator;
        public string ItemSeparator {
            get {
                return itemSeparator;
            }
            set {
                SetPropertyValue("ItemSeparator", ref itemSeparator, value);
            }
        }

        private string description;
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get {
                return description;
            }
            set {
                SetPropertyValue("Description", ref description, value);
            }
        }


        private int fullPathStartLevel;
        public int FullPathStartLevel {
            get {
                return fullPathStartLevel;
            }
            set {
                SetPropertyValue("FullPathStartLevel", ref fullPathStartLevel, value);
            }
        }

        private bool includeNameInFullPath;
        public bool IncludeNameInFullPath {
            get {
                return includeNameInFullPath;
            }
            set {
                SetPropertyValue("IncludeNameInFullPath", ref includeNameInFullPath, value);
            }
        }

        public static Taxonomy FindByKey(Session session, string key){
            return new XPQuery<Taxonomy>(session).Where(thes => thes.Key == key).SingleOrDefault();
        }

        public static Term FindTermByKey(Taxonomy taxonomy, string termKey){
            return taxonomy.FindTermByKey(termKey);
        }
        public Term FindTermByKey(string termKey) {
            return Terms.Where(term => term.Key == termKey).SingleOrDefault();
        }
        
        public static Term FindTermByFullPath(Taxonomy taxonomy, string fullPath) {
            return taxonomy.FindTermByFullPath(fullPath);
        }
        public Term FindTermByFullPath(string fullPath) {
            return Terms.Where(term => term.FullPath == fullPath).SingleOrDefault();
        }
        
        public static Term FindTermByValue(Taxonomy taxonomy, string termValue) {
            return taxonomy.FindTermByValue(termValue);
        }
        private Term FindTermByValue(string termValue) {
            return Terms.Where(term => term.TermValue == termValue).SingleOrDefault();
        }
        
        //public Term FindDeeperExistingTermFromPath(string fullpath){
        //    Term term = FindTermByFullPath(fullpath);
        //    if(term == null && FullPathToSegments(fullpath).Length> 0){
        //        return FindDeeperExistingTermFromPath(GetFullPathUpToSegment(fullpath, FullPathToSegments(fullpath).Length - 1));
        //    }
        //    return term;
        //}
        
        //public string GetFullPathUpToSegment(string fullpath, int segmentIndex) {
        //    return FullPathFromSegments(
        //        FullPathToSegments(fullpath).Take(segmentIndex).ToArray());
        //}
        //public string FullPathFromSegments(string[] segments) {
        //    return segments.Length > 1
        //               ? string.Format("{0}{1}{2}"
        //                               , FullPathFromSegments(segments.Take(segments.Length - 1).ToArray())
        //                               , pathSeparator
        //                               , segments[0])
        //               : segments[0];
        //}
        
        //public static string[] FullPathToSegments(Taxonomy taxonomy, string fullPath) {
        //    return taxonomy.FullPathToSegments(fullPath);
        //}
        //public string[] FullPathToSegments(string fullPath) {
        //    return fullPath.Split(new[] { PathSeparator }, StringSplitOptions.None);
        //}

        public static Taxonomy EnsureTaxonomyExists(ObjectSpace objectSpace, string key, string name, string group) {
            Taxonomy taxonomy = new XPQuery<Taxonomy>(objectSpace.Session).Where(tax => tax.Group == group && tax.Key == key).SingleOrDefault();
            if (taxonomy == null) {
                taxonomy = objectSpace.CreateObject<Taxonomy>();
                taxonomy.Key = key;
                taxonomy.Name = name;
                taxonomy.Group = group;
                taxonomy.Save();
            }
            return taxonomy;
        }

    }
}