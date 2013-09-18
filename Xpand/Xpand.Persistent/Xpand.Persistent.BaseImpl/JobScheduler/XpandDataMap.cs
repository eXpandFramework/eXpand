using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    [NonPersistent]
    public abstract class XpandDataMap : XpandCustomObject, IDataMap {
        string _typeName;
        string _name;

        protected XpandDataMap(Session session)
            : base(session) {
        }

        [Index(-1)]
        [RuleRequiredField]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Browsable(false)]
        public string TypeName {
            get { return _typeName; }
            set { _typeName = value; }
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            _typeName = GetType().FullName;
        }
    }

    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "TypeName,Name")]
    public abstract class XpandJobDetailDataMap : XpandDataMap {
        protected XpandJobDetailDataMap(Session session)
            : base(session) {
        }
    }
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "TypeName,Name")]
    public abstract class XpandJobDataMap : XpandDataMap {
        protected XpandJobDataMap(Session session)
            : base(session) {
        }
    }
}