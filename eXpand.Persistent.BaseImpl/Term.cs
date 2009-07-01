using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using System.Drawing;
using eXpand.Persistent.Base.Interfaces;

namespace eXpand.Persistent.BaseImpl
{
    [DefaultProperty("Name")]
    [FriendlyKeyProperty("Name")]
    [DefaultClassOptions]
    public class Term : Category, IIndexedObject, ITaggedObject{
        private string termValue;
        [Persistent, Size(SizeAttribute.Unlimited)] 
        protected string fullPath;
        private string indexer;
        private string key;
        private object tag;
        private Taxonomy taxonomy;
        public Term(Session session) : base(session) {}
        
        [Association("Term-TermInfos"), Aggregated]
        public XPCollection<TermInfo> TermInfos {
            get {
                return GetCollection<TermInfo>("TermInfos");
            }
        }

        [Association("Taxonomy-Terms"), Aggregated]
        public Taxonomy Taxonomy {
            get { return taxonomy; }
            set { SetPropertyValue("Taxonomy", ref taxonomy, value); }
        }

        [PersistentAlias("fullPath")]
        public string FullPath{
            get { return fullPath; }
        }

        public string TermValue{
            get { return termValue; }
            set { SetPropertyValue("TermValue", ref termValue, value); }
        }
        #region Category Implementation
        private Term parentTerm;

        [NonPersistent]
        protected override ITreeNode Parent{
            get { return ParentTerm; }
        }

        [NonPersistent]
        protected override IBindingList Children{
            get { return Terms; }
        }

        [Association("Term-Children"), Aggregated]
        public XPCollection<Term> Terms{
            get { return GetCollection<Term>("Terms"); }
        }

        [Association("Term-Children"), Aggregated]
        public Term ParentTerm{
            get { return parentTerm; }
            set { SetPropertyValue("ParentTerm", ref parentTerm, value); }
        }
        #endregion
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
        #region ITaggedObject Members
        public object Tag{
            get { return tag; }
            set { SetPropertyValue("Tag", ref tag, value); }
        }
        #endregion
        
        protected override void OnChanged(string propertyName, object oldValue, object newValue){
            base.OnChanged(propertyName, oldValue, newValue);
            if (!IsLoading && !IsSaving) {
                if (propertyName == "ParentTerm") {
                    if (parentTerm != null && taxonomy == null) {
                        Taxonomy = ParentTerm.Taxonomy;
                    }
                }
                if (propertyName == "Taxonomy") {
                    HandleTaxonomyChange();
                }
            }
        }

        private Color color;
        

        public Color Color {
            get {
                return color;
            }
            set {
                SetPropertyValue("Color", ref color, value);
            }
        }

        protected virtual void HandleTaxonomyChange() {
            UpdateFullPath();
            //UpdatePathSegments();
        }

        //private string[] pathSegments;
        
        //public string[] PathSegments {
        //    get {
        //        return pathSegments ?? new string[] { };
        //    }
        //}
        
        //protected virtual void UpdatePathSegments() {
        //    if (taxonomy != null && !string.IsNullOrEmpty(fullPath) && !string.IsNullOrEmpty(taxonomy.PathSeparator)) {
        //        pathSegments = taxonomy.FullPathToSegments(fullPath);
        //    }
        //}

        protected virtual void UpdateFullPath() {
            if (taxonomy != null) {
                fullPath = string.Format("{0}{1}{2}"
                                         , parentTerm != null
                                               ? parentTerm.FullPath
                                               : taxonomy.Name
                                         , taxonomy.PathSeparator
                                         , Name);
            }
        }
    }
}