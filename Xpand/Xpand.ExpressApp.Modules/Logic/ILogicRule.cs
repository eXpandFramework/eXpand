using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Logic {


    public interface ILogicRule : IRule {
        [Category("Behavior")]
        FrameTemplateContext FrameTemplateContext { get; set; }
        [Category("Behavior")]
        [DataSourceProperty("FrameTemplateContexts")]
        string FrameTemplateContextGroup { get; set; }

        [Category("Behavior")]
        bool? IsRootView { get; set; }

        [Category("Behavior")]
        [Description("Specifies the View type in which the current rule is in effect.")]
        [RefreshProperties(RefreshProperties.All)]
        ViewType ViewType { get; set; }

        [Category("Behavior")]
        [Description("Specifies the View type in which the current rule is in effect.")]
        [DataSourceProperty("Views")]
        IModelView View { get; set; }

        [Category("Behavior")]
        [Description("Specifies the Nesting type in which the current rule is in effect.")]
        Nesting Nesting { get; set; }

        [Category("Data")]
        [DataSourceProperty("ExecutionContexts")]
        string ExecutionContextGroup { get; set; }

        [Category("Data")]
        [DataSourceProperty("ActionExecutionContexts")]
        string ActionExecutionContextGroup { get; set; }

        [Category("Behavior")]
        [DataSourceProperty("ViewContexts")]
        string ViewContextGroup { get; set; }

        [Category("Behavior")]
        ViewEditMode? ViewEditMode { get; set; }

        [Browsable(false)]
        ITypeInfo TypeInfo { get; set; }
    }
}