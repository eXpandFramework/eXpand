using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;

namespace Xpand.Persistent.Base.Logic.Model {
    [ModelAbstractClass]

    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelLogicRule : IModelNode, ILogicModelClassRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity

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