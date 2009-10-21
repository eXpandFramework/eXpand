using System;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using eXpand.ExpressApp.Attributes;
using eXpand.Persistent.TaxonomyImpl;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    public abstract class TermBase : eXpandLiteObject, ITreeNode {
        [Persistent, Size(SizeAttribute.Unlimited)] protected string fullPath;
        private string key;

        [Persistent] protected int level;
        [XmlAttribute] private string name;
        private TermBase parentTerm;
        private AssociationXmlSerializationHelper persistentPropertiesSerializationHelper;
        protected Taxonomy taxonomy;

        protected TermBase(Session session) : base(session) {}
        protected TermBase() {}

        [PersistentAlias("fullPath")]
        [Indexed(Unique = true, Name = "pathIndex")]
        public string FullPath{
            get { return fullPath; }
        }

        [Persistent]
        [XmlAttribute]
        public string RelativePath{
            get{
                if (!IsLoading && !string.IsNullOrEmpty(fullPath)){
                    return fullPath.Substring(fullPath.IndexOf("/"));
                }
                return null;
            }
        }

        [Indexed]
        [XmlAttribute]
        public string Key{
            get { return key; }
            set { SetPropertyValue("Term", ref key, value); }
        }

        [ProvidedAssociation(Associations.TermAdditionalValues)]
        [Association(Associations.TermAdditionalValues, typeof(BaseInfo)), Aggregated]
        [XmlIgnore]
        public XPCollection AdditionalValues {
            get { return GetCollection("AdditionalValues"); }
        }

        [XmlArray("TermValuePairs")]
        [XmlArrayItem(typeof(BaseInfo))]
        [Browsable(false)]
        public AssociationXmlSerializationHelper PersistentPropertiesSerializationHelper {
            get {
                if (persistentPropertiesSerializationHelper == null)
                    persistentPropertiesSerializationHelper = new AssociationXmlSerializationHelper(AdditionalValues);
                return persistentPropertiesSerializationHelper;
            }
        }

        #region Tree structure and categorization implementation
        [Association(Associations.TermTerms), Aggregated]
        [XmlIgnore]
        public XPCollection<TermBase> Terms{
            get { return GetCollection<TermBase>("Terms"); }
        }

        [Association(Associations.TermTerms)]
        [XmlIgnore]
        public TermBase ParentTerm{
            get { return parentTerm; }
            set { SetPropertyValue("ParentTerm", ref parentTerm, value); }
        }

        [PersistentAlias("level")]
        public int Level{
            get { return level; }
        }

        [XmlAttribute]
        public string Name{
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        [XmlIgnore]
        [NonPersistent]
        [Browsable(false)]
        public ITreeNode Parent{
            get { return ParentTerm; }
        }

        [XmlIgnore]
        [NonPersistent]
        [Browsable(false)]
        public IBindingList Children{
            get { return Terms; }
        }
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
            base.OnSaving();
            UpdateFullPath(true);
        }

        public virtual void UpdateFullPath (bool updateChildren){
            if (!IsDeleted){
                level = (ParentTerm == null ? 0 : ParentTerm.Level + 1);
                if (parentTerm != null) taxonomy = parentTerm.taxonomy;

                fullPath = string.Format("{0}{1}{2}"
                                         , parentTerm == null ? taxonomy.Key : parentTerm.FullPath
                                         , "/"
                                         , key);

                OnChanged("fullPath");
                if (updateChildren){
                    foreach (TermBase term in Terms){
                        term.UpdateFullPath(true);
                    }
                }
            }
        }

        private string indexer;
        public string Indexer {
            get {
                return indexer;
            }
            set {
                SetPropertyValue("Indexer", ref indexer, value);
            }
        }
    }
}