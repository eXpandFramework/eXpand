using System;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.Persistent.TaxonomyImpl{
    [Serializable]
    [DefaultProperty("Value")]
    public class BaseInfo : eXpandCustomObject{
        private string _value;
        private string key;
        private BaseInfoValidity validity;
        

        public BaseInfo(Session session) : base(session) {}

        [Association(Associations.BaseObjectsBaseInfos)]
        [XmlIgnore]
        public XPCollection<BaseObject> BaseObjects {
            get { return GetCollection<BaseObject>("BaseObjects"); }
        }

        [XmlAttribute]
        public string Key {
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        public string Value {
            get { return _value; }
            set { SetPropertyValue("Value", ref _value, value); }
        }

        [XmlAttribute]
        public BaseInfoValidity Validity {
            get { return validity; }
            set { SetPropertyValue("Validity", ref validity, value); }
        }

        //public static TObject FindInfoOwner<TObject, TInfo>(Session session, string key, string value) where TObject:BaseObject where TInfo:BaseInfo {
        //    return (TObject) FindInfo<TInfo>(session, key, value).Owner;
        //}

        //public static TInfo FindInfo<TInfo>(Session session, string key, string value) where TInfo : BaseInfo {
        //    return new XPQuery<TInfo>(session).Where(info => info.Key == key && info.Value == value).SingleOrDefault();
        //}
    }
}