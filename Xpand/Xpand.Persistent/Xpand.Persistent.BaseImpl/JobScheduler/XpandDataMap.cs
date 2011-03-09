using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    [NonPersistent]
    public abstract class XpandDataMap : XpandCustomObject, IDataMap {
        [Persistent("TypeName")] protected string TypeName;
        string _name;

        protected XpandDataMap(Session session)
            : base(session) {
        }

        [Index(-1)]
        [RuleRequiredField]
        public string Name {
            get {
                return _name;
            }
            set {
                SetPropertyValue("Name", ref _name, value);
            }
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            TypeName = GetType().FullName;
        }
    }

    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "TypeName,Name")]
    public abstract class XpandJobDetailDataMap : XpandDataMap {
        protected XpandJobDetailDataMap(Session session)
            : base(session) {
        }
    }
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "TypeName,Name")]
    public abstract class XpandJobDataMap:XpandDataMap {
        protected XpandJobDataMap(Session session) : base(session) {
        }
    }
}