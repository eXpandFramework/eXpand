using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Logic {
    public abstract class LogicRule : ILogicRule {

        protected LogicRule(ILogicRule logicRule) {
            ExecutionContextGroup=logicRule.ExecutionContextGroup;
            Description=logicRule.Description;
            FrameTemplateContext = logicRule.FrameTemplateContext;
            ID = logicRule.Id;
            Index = logicRule.Index;
            IsRootView = logicRule.IsRootView;
            Nesting = logicRule.Nesting;
            TypeInfo = logicRule.TypeInfo;
            View = logicRule.View;
            ViewType = logicRule.ViewType;
        }

        public string ID { get; set; }
        #region ILogicRule Members
        public string ExecutionContextGroup { get; set; }


        public FrameTemplateContext FrameTemplateContext { get; set; }

        public bool? IsRootView { get; set; }

        public ViewType ViewType { get; set; }

        public IModelView View { get; set; }

        public Nesting Nesting { get; set; }


        string IRule.Id {
            get { return ID; }
            set { ID = value; }
        }

        public string Description { get; set; }

        public ITypeInfo TypeInfo { get; set; }

        public int Index { get; set; }
        #endregion
    }
}