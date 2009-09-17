using System;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using eXpand.ExpressApp.Attributes;
using eXpand.Persistent.BaseImpl;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{

    [DefaultProperty("Name")]
    [Serializable]
    [DefaultClassOptions]
    public class Term : eXpandLiteObject, ITreeNode {
        [Persistent, Size(SizeAttribute.Unlimited)]
        protected string fullPath;
        private string key;
        private AssociationXmlSerializationHelper pairsSerializationHelper;
        private Taxonomy taxonomy;
        public Term(Session session) : base(session) { }
        public Term() { }

        [PersistentAlias("fullPath")]
        [Indexed(Unique = true, Name = "pathIndex")]
        public string FullPath {
            get { return fullPath; }
        }

        [Indexed]
        [XmlAttribute]
        public string Key {
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        [Association("TaxonomyTerms")]
        [XmlIgnore]
        public Taxonomy Taxonomy {
            get { return taxonomy; }
            set { SetPropertyValue("Taxonomy", ref taxonomy, value); }
        }

        [Association("TermAssignmentKeys")]
        [XmlIgnore]
        public XPCollection<TermAssignment> AssignmentKeys {
            get { return GetCollection<TermAssignment>("AssignmentKeys"); }
        }

        [Association("TermAssignmentValues")]
        [XmlIgnore]
        public XPCollection<TermAssignment> AssignmentValues {
            get { return GetCollection<TermAssignment>("AssignmentValues"); }
        }

        [ProvidedAssociation("Terms")]
        [Association("TermKeyValuePairs", typeof(PersistentKeyValuePair)), Aggregated]
        [XmlIgnore]
        public XPCollection KeyValuePairs {
            get { return GetCollection("KeyValuePairs"); }
        }

        [XmlArray("TermValuePairs")]
        [XmlArrayItem(typeof(PersistentKeyValuePair))]
        [Browsable(false)]
        public AssociationXmlSerializationHelper PairsSerializationHelper {
            get {
                if (pairsSerializationHelper == null)
                    pairsSerializationHelper = new AssociationXmlSerializationHelper(KeyValuePairs);
                return pairsSerializationHelper;
            }
        }
        #region Category Implementation
        private Term parentTerm;

        [Association("TermStructure"), Aggregated]
        [XmlIgnore]
        public XPCollection<Term> Terms {
            get { return GetCollection<Term>("Terms"); }
        }

        [Association("TermStructure")]
        [XmlIgnore]
        public Term ParentTerm {
            get { return parentTerm; }
            set { SetPropertyValue("ParentTerm", ref parentTerm, value); }
        }
        #endregion

        private string caption;
        public string Caption {
            get {
                return caption;
            }
            set {
                SetPropertyValue("Caption", ref caption, value);
            }
        }
        #region ITreeNode Members
        [XmlAttribute]
        public string Name {
            get { return caption; }
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
        protected override void OnChanged(string propertyName, object oldValue, object newValue){
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "Caption"){
                if (string.IsNullOrEmpty(Key)){
                    Key = caption.ToLower();
                }
            }
            if (propertyName == "Key" && !string.IsNullOrEmpty(Key)) {
                UpdateAssignmentsKey();
            }
            if (propertyName == "ParentTerm")
            {
                Taxonomy = parentTerm.Taxonomy;
            }
            if ((propertyName == "Key" || propertyName == "ParentTerm")
                && (!string.IsNullOrEmpty(Key)
                    && (parentTerm != null || taxonomy != null))){
                UpdateFullPath(true);
            }
        }

        public void UpdateAssignmentsKey(){
            foreach (TermAssignment assignment in AssignmentKeys){
                assignment.Key = key;
            }
        }

        public virtual void UpdateFullPath(bool updateChildren) {
            fullPath = string.Format("{0}{1}{2}"
                                     , parentTerm == null ? taxonomy.Key : parentTerm.FullPath
                                     , "/"
                                     , key);
            
            OnChanged("fullPath");
            foreach (var assignment in AssignmentKeys) {
                assignment.Value = fullPath;
            }

            if (updateChildren){
                foreach (var term in Terms){
                    term.UpdateFullPath(true);
                }
            }
        }
    }
}