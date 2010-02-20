using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic
{
    public abstract class LogicRule : ILogicRule 
    {
        readonly ILogicRule _logicRule;

        protected LogicRule(ILogicRule logicRule){
            _logicRule = logicRule;
        }

        public string ID {
            get { return _logicRule.ID; }
            set { _logicRule.ID = value; }
        }

        public ViewType ViewType {
            get { return _logicRule.ViewType; }
            set { _logicRule.ViewType = value; }
        }

        public Nesting Nesting {
            get { return _logicRule.Nesting; }
            set { _logicRule.Nesting = value; }
        }

        

        public string Description {
            get { return _logicRule.Description; }
            set { _logicRule.Description = value; }
        }

        public ITypeInfo TypeInfo {
            get { return _logicRule.TypeInfo; }
            set { _logicRule.TypeInfo = value; }
        }

        public string ViewId {
            get { return _logicRule.ViewId; }
            set { _logicRule.ViewId = value; }
        }
    }
}


