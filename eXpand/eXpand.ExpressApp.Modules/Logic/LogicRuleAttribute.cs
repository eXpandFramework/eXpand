using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.Logic {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class LogicRuleAttribute : Attribute, ILogicRule {
        protected LogicRuleAttribute(string id) {
            Id = id;
            ExecutionContextGroup = LogicDefaultGroupContextNodeUpdater.Default;
        }
        #region ILogicRule Members
        public string Id { get; set; }
        public string Description { get; set; }
        public int Index { get; set; }
        public ViewType ViewType { get; set; }
        public string ViewId { get; set; }
        public Nesting Nesting { get; set; }
        public string ExecutionContextGroup { get; set; }

        

        ITypeInfo ILogicRule.TypeInfo { get; set; }
        #endregion
    }
}