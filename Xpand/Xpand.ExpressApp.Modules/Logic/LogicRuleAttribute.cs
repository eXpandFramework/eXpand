using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.TypeConverters;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class LogicRuleAttribute : Attribute, ILogicRule {
        protected LogicRuleAttribute(string id) {
            Id = id;
        }
        #region ILogicRule Members
        public string Id { get; set; }
        public string Description { get; set; }
        public int? Index { get; set; }

        public FrameTemplateContext FrameTemplateContext { get; set; }


        public ViewType ViewType { get; set; }
        public ViewEditMode? ViewEditMode { get; set; }
        public string View { get; set; }
        [TypeConverter(typeof(StringToModelViewConverter))]
        IModelView ILogicRule.View { get; set; }
        public Nesting Nesting { get; set; }
        public string ExecutionContextGroup { get; set; }
        public string ActionExecutionContextGroup { get; set; }
        public string ViewContextGroup { get; set; }
        public string FrameTemplateContextGroup { get; set; }


        public bool? IsRootView { get; set; }


        ITypeInfo ILogicRule.TypeInfo { get; set; }
        #endregion
    }
}