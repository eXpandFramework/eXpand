using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Logic.Model {
    [ModelAbstractClass]
    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelLogicRule : IModelNode, ILogicModelClassRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity
        [RuleFromBoolProperty("GroupContext", DefaultContexts.Save, CustomMessageTemplate = "At least one of ExecutionContextGroup, ActionExecutionContextGroup should not be null")]
        bool GroupContext { get; }
        [Browsable(false)]
        IEnumerable<IModelView> Views { get; }
        [Browsable(false)]
        IEnumerable<string> ActionExecutionContexts { get; }
        [Browsable(false)]
        IEnumerable<string> ExecutionContexts { get; }
        [Browsable(false)]
        IEnumerable<string> FrameTemplateContexts { get; }
        [Browsable(false)]
        IEnumerable<string> ViewContexts { get; }
    }
}