using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Logic {

    [RuleCriteria("testrule1", DefaultContexts.Save, "(Not IsNullOrEmpty(ExecutionContextGroup)) OR (Not IsNullOrEmpty(ActionExecutionContextGroup))", SkipNullOrEmptyValues = false, CustomMessageTemplate = "At least one of ExecutionContextGroup, ActionExecutionContextGroup should not be null")]
    public interface ILogicRule : IRule {
        [Category("Logic.Behavior")]
        FrameTemplateContext FrameTemplateContext { get; set; }
        [Category("Logic.Behavior")]
        [DataSourceProperty("FrameTemplateContexts")]
        string FrameTemplateContextGroup { get; set; }

        [Category("Logic.Behavior")]
        bool? IsRootView { get; set; }

        [Category("Logic.Behavior")]
        [Description("Specifies the View type in which the current rule is in effect.")]
        [RefreshProperties(RefreshProperties.All)]
        ViewType ViewType { get; set; }

        [Category("Logic.Behavior")]
        [Description("Specifies the View type in which the current rule is in effect.")]
        [DataSourceProperty("Views")]
        IModelView View { get; set; }

        [Category("Logic.Behavior")]
        [Description("Specifies the Nesting type in which the current rule is in effect.")]
        Nesting Nesting { get; set; }

        [Category("Logic.Data")]
        [DataSourceProperty("ExecutionContexts")]
        string ExecutionContextGroup { get; set; }

        [Category("Logic.Data")]
        [DataSourceProperty("ActionExecutionContexts")]
        string ActionExecutionContextGroup { get; set; }

        [Category("Logic.Behavior")]
        [DataSourceProperty("ViewContexts")]
        string ViewContextGroup { get; set; }

        [Category("Logic.Behavior")]
        ViewEditMode? ViewEditMode { get; set; }

        [Browsable(false)]
        ITypeInfo TypeInfo { get; set; }
    }
}