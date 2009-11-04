using System.ComponentModel;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.Base.Taxonomies{
    [DefaultProperty("Name"), NonPersistent]
    public abstract class BaseTerm : eXpandCustomObject, IBaseTerm{
        [Persistent("FullPath"), Size(SizeAttribute.Unlimited)] private string fullPath;
        private string indexer;
        private string key;
        [Persistent("Level")] private int level;
        private string name;

        protected BaseTerm(Session session) : base(session) {}

        [Indexed]
        public string Key{
            get { return key; }
            set { SetPropertyValue("BaseTerm", ref key, value); }
        }
        #region IBaseTerm Members
        [PersistentAlias("fullPath")]
        [Indexed(Unique = true, Name = "pathIndex")]
        public string FullPath{
            get { return fullPath; }
        }

        [Persistent]
        public string RelativePath{
            get{
                if (!IsLoading && !string.IsNullOrEmpty(fullPath)){
                    return fullPath.Substring(fullPath.IndexOf("/"));
                }
                return null;
            }
        }

        public string Indexer{
            get { return indexer; }
            set { SetPropertyValue("Indexer", ref indexer, value); }
        }

        public virtual void EvaluateTermPropertyValues(bool recursive){
            if (!IsDeleted){

                level = (ParentTerm == null ? 0 : ParentTerm.Level + 1);
                if (ParentTerm != null) BaseTaxonomy = ParentTerm.BaseTaxonomy;
                fullPath = string.Format("{0}{1}{2}"
                                         , ParentTerm == null ? BaseTaxonomy.Key : ParentTerm.FullPath
                                         , "/"
                                         , key);
                OnChanged("fullPath");

                if (string.IsNullOrEmpty(name)){
                    Name = key;
                }

                if (recursive){
                    foreach (IBaseTerm term in Children){
                        term.EvaluateTermPropertyValues(true);
                    }
                }
            }
        }

        [Browsable(false)]
        public IBaseTerm ParentTerm{
            get { return (IBaseTerm) Parent; }
            set { Parent = value; }
        }

        [Browsable(false)]
        public abstract IBaseTaxonomy BaseTaxonomy { get; set; }

        [PersistentAlias("level")]
        public int Level{
            get { return level; }
        }

        public string Name{
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        [Browsable(false)]
        public abstract ITreeNode Parent { get; set; }

        [Browsable(false)]
        public abstract IBindingList Children { get; }
        #endregion
        protected override void OnChanged(string propertyName, object oldValue, object newValue){
            base.OnChanged(propertyName, oldValue, newValue);
            if (!IsDeleted){
                if (propertyName == "Name"){
                    if (string.IsNullOrEmpty(Key)){
                        Key = name.ToLower();
                    }
                }
            }
        }

        protected override void OnSaving(){
            EvaluateTermPropertyValues(false);
            base.OnSaving();
        }
    }
}