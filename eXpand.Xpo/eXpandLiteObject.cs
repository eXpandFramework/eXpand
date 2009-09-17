using System;
using System.Xml.Serialization;
using DevExpress.Xpo;

namespace eXpand.Xpo{
    [Serializable]
    public abstract class eXpandLiteObject : XPCustomObject{
        private Guid oid;

        protected eXpandLiteObject() {}
        protected eXpandLiteObject(Session session) : base(session) {}

        [Key(AutoGenerate = true)]
        [XmlAttribute]
        public Guid Oid {
            get {
                return oid;
            }
            set {
                SetPropertyValue("Oid", ref oid, value);
            }
        }
    }
}