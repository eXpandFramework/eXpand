using System;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl {
    public class XpandDateRange : XpandCustomObject, IDateRange {
        public XpandDateRange(Session session)
            : base(session) {
        }
        private string _name;
        [RuleUniqueValue(null, DefaultContexts.Save)]
        [RuleRequiredField]
        public string Name {
            get {
                return _name;
            }
            set {
                SetPropertyValue("Name", ref _name, value);
            }
        }
        private DateTime _startPoint;
        [DisplayDateAndTime]
        public DateTime StartPoint {
            get {
                return _startPoint;
            }
            set {
                SetPropertyValue("StartPoint", ref _startPoint, value);
            }
        }
        private DateTime _endPoint;
        [DisplayDateAndTime]
        public DateTime EndPoint {
            get {
                return _endPoint;
            }
            set {
                SetPropertyValue("EndPoint", ref _endPoint, value);
            }
        }
    }
}