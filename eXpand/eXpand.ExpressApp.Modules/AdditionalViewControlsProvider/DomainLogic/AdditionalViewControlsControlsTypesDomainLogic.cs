using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.DomainLogic {
    public abstract class AdditionalViewControlsControlsTypesDomainLogic<TControl,TDecorator> where TDecorator:AdditionalViewControlsProviderDecorator{
        bool gettingControlTypeValue;
        bool gettingDecoratorTypeValue;

        public void BeforeGet(object logicRule, string propertyName) {
            if (propertyName == "ControlType") {
                if (!gettingControlTypeValue) {
                    gettingControlTypeValue = true;
                    var rule = ((IAdditionalViewControlsRule) logicRule);
                    if (rule.ControlType == null)
                        rule.ControlType = typeof(TControl);
                    gettingControlTypeValue = false;
                }
            }
            if (propertyName == "DecoratorType") {
                if (!gettingDecoratorTypeValue) {
                    gettingDecoratorTypeValue = true;
                    var rule = ((IAdditionalViewControlsRule) logicRule);
                    if (rule.DecoratorType == null)
                        rule.DecoratorType = typeof(TDecorator);
                    gettingDecoratorTypeValue = false;
                }
            }
        }

        
    }
}