using System;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.NodeWrappers {
    public interface IAdditionalViewControlsRule:IModelRule {
        string Message { get; set; }
        string MessagePropertyName { get; set; }
        Type DecoratorType { get; set; }
        Type ControlType { get; set; }
        AdditionalViewControlsProviderPosition AdditionalViewControlsProviderPosition { get; set; }
        bool UseSameIfFound { get; set; }
    }
}