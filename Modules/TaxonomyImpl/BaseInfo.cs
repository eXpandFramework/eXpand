using System;
using System.Linq;
using System.Xml.Serialization;
using DevExpress.Xpo;
using eXpand.Xpo;
using System.Drawing;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Globalization;

namespace eXpand.Persistent.TaxonomyImpl{
    [Serializable]
    public abstract class BaseInfo : eXpandLiteObject {
        private BaseObject owner;
        private InfoValidity validity;
        public BaseInfo() {}

        // ReSharper disable InconsistentNaming
        private string _value;
        // ReSharper restore InconsistentNaming
        private string key;

        protected BaseInfo(Session session) : base(session) {}

        [Association(Associations.BaseObjectsBaseInfos)]
        [XmlIgnore]
        public XPCollection<BaseObject> BaseObjects {
            get {
                return GetCollection<BaseObject>("BaseObjects");
            }
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
        public InfoValidity Validity {
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