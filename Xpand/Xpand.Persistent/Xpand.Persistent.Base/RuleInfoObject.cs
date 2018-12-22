using System;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.Persistent.Base{
    public enum RuleScheduleType{
        Purging
    }
    public class RuleInfoObject:XpandCustomObject{
        RuleScheduleType _ruleScheduleType;
        string _ruleId;
        DateTime _executed;

        public RuleInfoObject(Session session) : base(session){
        }

        public RuleScheduleType RuleScheduleType{
            get => _ruleScheduleType;
            set => SetPropertyValue(nameof(RuleScheduleType), ref _ruleScheduleType, value);
        }

        public string RuleId{
            get => _ruleId;
            set => SetPropertyValue(nameof(RuleId), ref _ruleId, value);
        }

        public DateTime Executed{
            get => _executed;
            set => SetPropertyValue(nameof(Executed), ref _executed, value);
        }
    }
}
