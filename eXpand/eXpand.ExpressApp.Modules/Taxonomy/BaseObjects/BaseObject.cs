using System;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using System.Linq;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Taxonomy.BaseObjects{
    [Serializable]
    public abstract class BaseObject : XPCustomObject {
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
        [Action]
        public void Taxonomize(){
            IQueryable<TaxonomyRule> queryable = new XPQuery<TaxonomyRule>(Session).Where(rule => rule.TypeOfObject == GetType().AssemblyQualifiedName);
            foreach (var rule in queryable){
                taxonomize(rule);   
            }
            
        }

        public string taxonomize(TaxonomyRule taxonomyRule)
        {
            var collection = new XPCollection<Term>(taxonomyRule.Session, taxonomyRule.TaxonomyQuery.ParseCriteria());
            if (collection.Count>0){
                foreach (var xpCollection in collection){
                    string path = xpCollection.FullPath + "/" + GetMemberValue(taxonomyRule.PropertyName);
                    ObjectInfos.Add(new BaseObjectInfo(Session) { Key = taxonomyRule.Taxonomy.GetTerm<Term>(path, String.Empty)});
                }
            }
            else{
                Term term = taxonomyRule.Taxonomy.GetTerm<Term>(
                    string.Format("{0}/{1}", taxonomyRule.Taxonomy.Key,
                                  GetMemberValue(
                                      taxonomyRule.PropertyName)),
                    String.Empty);
                ObjectInfos.Add(new BaseObjectInfo(Session){
                                                               Key =(Term) Session.GetObject(term)
                                                           });
            }
            Session.CommitTransaction();
            return null;
        }

        protected BaseObject(Session session) : base(session) { }
        protected BaseObject(){ }
        
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
        [XmlArray("TaxonomyBaseObjectInfos")]
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