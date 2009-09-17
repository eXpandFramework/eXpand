using System;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.TaxonomyImpl;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    public abstract class TaxonomyBaseObject : XPCustomObject {
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), NonCloneable]
        [XmlAttribute]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else
        [Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)]
        private Guid oid = Guid.Empty;
        [PersistentAlias("oid"), Browsable(false)]
        [XmlAttribute]
        public Guid Oid {
            get { return oid; }
        }
#endif
        private bool isDefaultPropertyAttributeInit;
        private XPMemberInfo defaultPropertyMemberInfo;
        protected override void OnSaving() {
            base.OnSaving();
            if(!(Session is NestedUnitOfWork) && Session.IsNewObject(this)) {
                oid = XpoDefault.NewGuid();
            }
        }

        public void Taxonomize(TaxonomyRule taxonomyRule)
        {
            
        }
        protected TaxonomyBaseObject(Session session) : base(session) { }
        protected TaxonomyBaseObject(){ }
        
        public override string ToString() {
            if(!isDefaultPropertyAttributeInit) {
                var attrib = ClassInfo.FindAttributeInfo(typeof(DefaultPropertyAttribute)) as DefaultPropertyAttribute;
                if(attrib != null) {
                    defaultPropertyMemberInfo = ClassInfo.FindMember(attrib.Name);
                }
                isDefaultPropertyAttributeInit = true;
            }
            if(defaultPropertyMemberInfo != null) {
                object obj = defaultPropertyMemberInfo.GetValue(this);
                if(obj != null) {
                    return obj.ToString();
                }
            }
            return base.ToString();
        }

        private string caption;
        
        [XmlAttribute]
        public string Caption{
            get { return caption; }
            set { SetPropertyValue("Caption", ref caption, value); }
        }

        
        [Association("ObjectInfos")]
        [XmlIgnore]
        public XPCollection<BaseObjectInfo> ObjectInfos{
            get { return GetCollection<BaseObjectInfo>("ObjectInfos"); }
        }

        AssociationXmlSerializationHelper objectInfosSerializationHelper;
        [XmlArray("ObjectInfos")]
        [XmlArrayItem(typeof(BaseObjectInfo))]
        [Browsable(false)]
        public AssociationXmlSerializationHelper ObjectInfosSerializationHelper
        {
            get
            {
                if (objectInfosSerializationHelper == null)
                    objectInfosSerializationHelper = new AssociationXmlSerializationHelper(ObjectInfos);
                return objectInfosSerializationHelper;
            }
        }

    }
}