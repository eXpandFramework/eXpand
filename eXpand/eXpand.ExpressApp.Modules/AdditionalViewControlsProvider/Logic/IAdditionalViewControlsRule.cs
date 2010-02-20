using System;
using eXpand.ExpressApp.Logic.Conditional;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public interface IAdditionalViewControlsRule:IConditionalLogicRule {
        string Message { get; set; }
        string MessagePropertyName { get; set; }
        Type DecoratorType { get; set; }
        Type ControlType { get; set; }
        AdditionalViewControlsProviderPosition AdditionalViewControlsProviderPosition { get; set; }
        bool UseSameIfFound { get; set; }
    }
}