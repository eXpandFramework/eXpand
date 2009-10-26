using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;

namespace eXpand.Persistent.TaxonomyImpl{
    [Serializable]
    public abstract class BaseObject : XPCustomObject{
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), NonCloneable]
        [XmlAttribute]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else
        [Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)] private Guid oid = Guid.Empty;

        [PersistentAlias("oid"), Browsable(false)]
        [XmlAttribute]
        public Guid Oid{
            get { return oid; }
        }
#endif

        public void SetId(Guid id){
            if (Session.IsNewObject(this) && oid == Guid.Empty){
                oid = id;
            }
        }

        private bool isDefaultPropertyAttributeInit;
        private XPMemberInfo defaultPropertyMemberInfo;

        protected override void OnSaving(){
            base.OnSaving();
            if (!(Session is NestedUnitOfWork) && Session.IsNewObject(this) && oid == Guid.Empty){
                oid = XpoDefault.NewGuid();
            }
        }

        //[Action]
        //public void Taxonomize(){
        //    IQueryable<TaxonomyRule> queryable = new XPQuery<TaxonomyRule>(Session).Where(rule => rule.TypeOfObject == GetType().AssemblyQualifiedName);
        //    foreach (var rule in queryable){
        //        taxonomize(rule);   
        //    }

        //}

        //public string taxonomize(TaxonomyRule taxonomyRule)
        //{
        //    var collection = new XPCollection<Term>(taxonomyRule.Session, taxonomyRule.TaxonomyQuery.ParseCriteria());
        //    if (collection.Count>0){
        //        foreach (var xpCollection in collection){
        //            string path = xpCollection.FullPath + "/" + GetMemberValue(taxonomyRule.PropertyName);
        //            ObjectInfos.Add(new BaseInfo(Session) { Key = taxonomyRule.Taxonomy.GetTerm<Term>(path, String.Empty)});
        //        }
        //    }
        //    else{
        //        Term term = taxonomyRule.Taxonomy.GetTerm<Term>(
        //            string.Format("{0}/{1}", taxonomyRule.Taxonomy.Key,
        //                          GetMemberValue(
        //                              taxonomyRule.PropertyName)),
        //            String.Empty);
        //        ObjectInfos.Add(new BaseInfo(Session){
        //                                                       Key =(Term) Session.GetObject(term)
        //                                                   });
        //    }
        //    Session.CommitTransaction();
        //    return null;
        //}

        protected BaseObject(Session session) : base(session) {}
        protected BaseObject() {}

        public override string ToString(){
            if (!isDefaultPropertyAttributeInit){
                var attrib = ClassInfo.FindAttributeInfo(typeof (DefaultPropertyAttribute)) as DefaultPropertyAttribute;
                if (attrib != null){
                    defaultPropertyMemberInfo = ClassInfo.FindMember(attrib.Name);
                }
                isDefaultPropertyAttributeInit = true;
            }
            if (defaultPropertyMemberInfo != null){
                object obj = defaultPropertyMemberInfo.GetValue(this);
                if (obj != null){
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

        [Association(Associations.BaseObjectsBaseInfos)]
        [XmlIgnore]
        public XPCollection<BaseInfo> Infos {
            get { return GetCollection<BaseInfo>("Infos"); }
        }

        private AssociationXmlSerializationHelper infosSerializationHelper;

        [XmlArray("Infos")]
        [Browsable(false)]
        public virtual AssociationXmlSerializationHelper InfosSerializationHelper{
            get{
                if (infosSerializationHelper == null)
                    infosSerializationHelper = new AssociationXmlSerializationHelper(Infos);
                return infosSerializationHelper;
            }
        }

        public string SerializeToString() {
            Type[] types = Infos.Select(info => info.ClassInfo.ClassType).Distinct().ToArray();
            string serializedInstance = Foxhound.Xpo.Xml.Serialization.XpoXmlSerializer.SerializeInstance(this, types);
            return serializedInstance;
        }

        public XmlDocument SerializeToXmlDocument() {
            var doc = new XmlDocument();
            doc.LoadXml(SerializeToString());
            return doc;
        }

        private string key;
        public string Key {
            get {
                return key;
            }
            set {
                SetPropertyValue("Key", ref key, value);
            }
        }

        public IEnumerable<TInfo> GetInfosOfType<TInfo>() where TInfo:BaseInfo{
            return Infos.OfType<TInfo>();    
        }
    }
}