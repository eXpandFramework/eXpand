using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace Xpand.Persistent.Base.Logic.Model {
    [RuleCriteria("IContextLogicRule", DefaultContexts.Save, "(Not IsNullOrEmpty(ExecutionContextGroup)) OR (Not IsNullOrEmpty(ActionExecutionContextGroup))", SkipNullOrEmptyValues = false, CustomMessageTemplate = "At least one of ExecutionContextGroup, ActionExecutionContextGroup should not be null")]
    public interface IContextLogicRule : ILogicRule {
        [Category("Logic.Behavior")]
        [DataSourceProperty("FrameTemplateContexts")]
        string FrameTemplateContextGroup { get; set; }

        [Category("Logic.Data")]
        [DataSourceProperty("ExecutionContexts")]
        string ExecutionContextGroup { get; set; }

        [Category("Logic.Data")]
        [DataSourceProperty("ActionExecutionContexts")]
        string ActionExecutionContextGroup { get; set; }

        [Category("Logic.Behavior")]
        [DataSourceProperty("ViewContexts")]
        string ViewContextGroup { get; set; }
    }
}