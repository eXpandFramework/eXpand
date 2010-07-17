using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;
using eXpand.ExpressApp.Logic.TypeConverters;

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
        public string View { get; set; }
        [TypeConverter(typeof(StringToModelViewConverter))]
        IModelView ILogicRule.View { get; set; }
        public Nesting Nesting { get; set; }
        public string ExecutionContextGroup { get; set; }

        

        ITypeInfo ILogicRule.TypeInfo { get; set; }
        #endregion
    }
}