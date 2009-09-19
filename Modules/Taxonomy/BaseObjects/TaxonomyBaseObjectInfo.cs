using System;
using System.Reflection;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using eXpand.Persistent.TaxonomyImpl;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    [DefaultClassOptions]
    public class TaxonomyBaseObjectInfo : eXpandLiteObject, ICategorizedItem, IPersistentProperty{
        private Term key;
        private TaxonomyBaseObject owner;
        private PersistentKeyValuePairStatus status;
        public TaxonomyBaseObjectInfo() {}

        public TaxonomyBaseObjectInfo(Session session) : base(session) {}

        private StructuralTerm structuralTerm;
        [Association(Associations.StructuralTermObjectInfos)]
        [XmlIgnore]
        public StructuralTerm StructuralTerm
        {
            get
            {
                return structuralTerm;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref structuralTerm, value);
            }
        }

        [Association("ObjectInfoTerms")]
        [XmlIgnore]
        public Term Key{
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        [Association("ObjectInfos")]
        [XmlIgnore]
        public TaxonomyBaseObject Owner{
            get { return owner; }
            set { SetPropertyValue("Owner", ref owner, value); }
        }

        [XmlIgnore]
        public Term Category{
            get { return key; }
            set { key = value; }
        }
        #region ICategorizedItem Members
        [XmlIgnore]
        ITreeNode ICategorizedItem.Category{
            get { return key; }
            set { key = (Term) value; }
        }
        #endregion
        #region Implementation of IPersistentProperty
        //ToDO: Implement it with persisten alias ????

        string IPersistentProperty.Key{
            get { return Key != null ? Key.Key : null; }
            set { throw new NotImplementedException(); }
        }

        string IPersistentProperty.Value{
            get { return Key.FullPath; }
            set { throw new NotImplementedException(); }
        }
        #endregion
        #region IPersistentProperty Members
        [XmlAttribute]
        public PersistentKeyValuePairStatus Status{
            get { return status; }
            set { SetPropertyValue("Status", ref status, value); }
        }
        #endregion
    }
}