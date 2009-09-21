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

    public abstract class TermBase : eXpandLiteObject, ITreeNode{
        private readonly string caption;
        [Persistent, Size(SizeAttribute.Unlimited)] protected string fullPath;
        private string key;
        private AssociationXmlSerializationHelper keyValuePairsSerializationHelper;

        [Persistent] protected int level;
        [XmlAttribute] private string name;
        private TermBase parentTerm;
        private Taxonomy taxonomy;

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

        [Association(Associations.TaxonomyTerms)]
        [XmlIgnore]
        public Taxonomy Taxonomy{
            get { return taxonomy; }
            set { SetPropertyValue("Taxonomy", ref taxonomy, value); }
        }

        [ProvidedAssociation(Associations.TermKeyValuePairs)]
        [Association(Associations.TermKeyValuePairs, typeof(PersistentKeyValuePair)), Aggregated]
        [XmlIgnore]
        public XPCollection KeyValuePairs {
            get { return GetCollection("KeyValuePairs"); }
        }

        [XmlArray("TermValuePairs")]
        [XmlArrayItem(typeof(PersistentKeyValuePair))]
        [Browsable(false)]
        public AssociationXmlSerializationHelper KeyValuePairsSerializationHelper {
            get {
                if (keyValuePairsSerializationHelper == null)
                    keyValuePairsSerializationHelper = new AssociationXmlSerializationHelper(KeyValuePairs);
                return keyValuePairsSerializationHelper;
            }
        }

        #region Tree structure and categorization implementation

        [Association(Associations.TermTerms), Aggregated]
        [XmlIgnore]
        public XPCollection<TermBase> Terms {
            get { return GetCollection<TermBase>("Terms"); }
        }

        [Association(Associations.TermTerms)]
        [XmlIgnore]
        public TermBase ParentTerm {
            get { return parentTerm; }
            set { SetPropertyValue("ParentTerm", ref parentTerm, value); }
        }

        #region ITreeNode Members
        [XmlAttribute]
        public string Name {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        [PersistentAlias("level")]
        public int Level {
            get { return level; }
        }

        [XmlIgnore]
        [NonPersistent]
        [Browsable(false)]
        public ITreeNode Parent {
            get { return ParentTerm; }
        }

        [XmlIgnore]
        [NonPersistent]
        [Browsable(false)]
        public IBindingList Children {
            get { return Terms; }
        }
        #endregion
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

            level = (ParentTerm == null ? 0 : ParentTerm.Level + 1);
            if (parentTerm != null) Taxonomy = parentTerm.Taxonomy;

            UpdateFullPath(true);
        }

        public virtual void UpdateFullPath(bool updateChildren){
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
}