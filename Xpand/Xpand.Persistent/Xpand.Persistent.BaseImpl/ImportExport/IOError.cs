using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.Persistent.BaseImpl.ImportExport {
    [DefaultClassOptions]
    [NavigationItem("ImportExport")]
    [DefaultProperty("Reason")]
    public class IOError : XPBaseObject, IIOError {
        public IOError(Session session)
            : base(session) {
        }
        private int _oid;
        [Key(true)]
        public int Oid {
            get {
                return _oid;
            }
            set {
                SetPropertyValue("Oid", ref _oid, value);
            }
        }
        private FailReason _reason;
        public FailReason Reason {
            get {
                return _reason;
            }
            set {
                SetPropertyValue("Type", ref _reason, value);
            }
        }
        private string _elementXml;
        [Size(SizeAttribute.Unlimited)]
        public string ElementXml {
            get {
                return _elementXml;
            }
            set {
                SetPropertyValue("Xml", ref _elementXml, value);
            }
        }
        private string _innerXml;
        [Size(SizeAttribute.Unlimited)]
        public string InnerXml {
            get {
                return _innerXml;
            }
            set {
                SetPropertyValue("InnerXml", ref _innerXml, value);
            }
        }
    }
}