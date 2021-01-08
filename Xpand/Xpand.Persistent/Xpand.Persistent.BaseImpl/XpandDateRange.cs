using System;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Attributes.Custom;
using Xpand.Persistent.Base.General;
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
            get => _name;
            set => SetPropertyValue("Name", ref _name, value);
        }
        private DateTime _startPoint;
        [DisplayDateAndTime]
        public DateTime StartPoint {
            get => _startPoint;
            set => SetPropertyValue("StartPoint", ref _startPoint, value);
        }
        private DateTime _endPoint;
        [DisplayDateAndTime]
        public DateTime EndPoint {
            get => _endPoint;
            set => SetPropertyValue("EndPoint", ref _endPoint, value);
        }
    }
}