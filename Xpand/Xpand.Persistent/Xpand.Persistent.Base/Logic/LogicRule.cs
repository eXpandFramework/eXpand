using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.Logic {
    public abstract class LogicRule : ILogicRule {
        protected LogicRule(ILogicRule logicRule) {
            var rule = ((ILogicRule) this);
            rule.ExecutionContextGroup = logicRule.ExecutionContextGroup;
            rule.FrameTemplateContextGroup = logicRule.FrameTemplateContextGroup;
            Description = logicRule.Description;
            FrameTemplateContext = logicRule.FrameTemplateContext;
            ID = logicRule.Id;
            Index = logicRule.Index;
            IsRootView = logicRule.IsRootView;
            Nesting = logicRule.Nesting;
            TypeInfo = logicRule.TypeInfo;
            View = logicRule.View;
            ViewType = logicRule.ViewType;
            ViewEditMode = logicRule.ViewEditMode;
            ViewContextGroup = logicRule.ViewContextGroup;
            
            NormalCriteria=logicRule.NormalCriteria;
            EmptyCriteria=logicRule.EmptyCriteria;
        }

        public string ID { get; set; }
        #region ILogicRule Members
        string ILogicRule.ExecutionContextGroup { get; set; }
        public string ActionExecutionContextGroup { get; set; }

        public string ViewContextGroup { get; set; }
        public ViewEditMode? ViewEditMode { get; set; }


        public string NormalCriteria { get; set; }

        public string EmptyCriteria { get; set; }

        public FrameTemplateContext FrameTemplateContext { get; set; }
        public string FrameTemplateContextGroup { get; set; }

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

        public int? Index { get; set; }

        public ExecutionContext ExecutionContext { get; set; }
        #endregion
        public override string ToString() {
            return !string.IsNullOrEmpty(ID) ? ID : base.ToString();
        }
    }
}