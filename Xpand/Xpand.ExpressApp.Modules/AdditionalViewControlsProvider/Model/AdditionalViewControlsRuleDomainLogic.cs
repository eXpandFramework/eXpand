using System;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.Persistent.Base.AdditionalViewControls;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Model {
    [DomainLogic(typeof(IAdditionalViewControlsRule))]
    public class AdditionalViewControlsRuleDomainLogic {
        public static Type Get_ControlType(IAdditionalViewControlsRule additionalViewControlsRule) {
            TypeDecorator decorator = ModelAdditionalViewControlsRuleDomainLogic.GetTypeDecorator(additionalViewControlsRule.DecoratorType, additionalViewControlsRule.Position);
            return decorator != null ? decorator.DefaultType : typeof(NotAvaliableInThisPlatform);
        }

        public static Type Get_DecoratorType(IAdditionalViewControlsRule additionalViewControlsRule) {
            var decoratorType =
                ModelAdditionalViewControlsRuleDomainLogic.GetDecorators().FirstOrDefault(info => info.Type.GetCustomAttributes(typeof(TypeDecorator), true).OfType<TypeDecorator>().Count(decorator => decorator.IsDefaultDecorator) > 0);
            return decoratorType != null ? decoratorType.Type : typeof(NotAvaliableInThisPlatform);
        }
    }
    class NotAvaliableInThisPlatform {

    }
}
