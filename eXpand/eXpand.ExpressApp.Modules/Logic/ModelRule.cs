using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.Logic
{
    public abstract class ModelRule : IModelRule
    {
        readonly IModelRule _modelRule;

        protected ModelRule(IModelRule modelRule) {
            _modelRule = modelRule;
        }

        public string ID {
            get { return _modelRule.ID; }
            set { _modelRule.ID = value; }
        }

        public ViewType ViewType {
            get { return _modelRule.ViewType; }
            set { _modelRule.ViewType = value; }
        }

        public Nesting Nesting {
            get { return _modelRule.Nesting; }
            set { _modelRule.Nesting = value; }
        }

        public string NormalCriteria {
            get { return _modelRule.NormalCriteria; }
            set { _modelRule.NormalCriteria = value; }
        }

        public string EmptyCriteria {
            get { return _modelRule.EmptyCriteria; }
            set { _modelRule.EmptyCriteria = value; }
        }

        public string Description {
            get { return _modelRule.Description; }
            set { _modelRule.Description = value; }
        }

        public ITypeInfo TypeInfo {
            get { return _modelRule.TypeInfo; }
            set { _modelRule.TypeInfo = value; }
        }

        public string ViewId {
            get { return _modelRule.ViewId; }
            set { _modelRule.ViewId = value; }
        }
    }
}


