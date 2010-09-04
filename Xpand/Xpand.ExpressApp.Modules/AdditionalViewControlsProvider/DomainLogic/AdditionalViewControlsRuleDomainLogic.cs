using System;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.DomainLogic {
    [DomainLogic(typeof(IAdditionalViewControlsRule))]
    public class AdditionalViewControlsRuleDomainLogic
    {
        public static Type Get_ControlType(IAdditionalViewControlsRule additionalViewControlsRule)
        {
            TypeDecorator decorator = ModelAdditionalViewControlsRuleDomainLogic.GetTypeDecorator(additionalViewControlsRule.Position);
            return decorator != null ? decorator.DefaultType : typeof(NotAvaliableInThisPlatform);
        }

        public static Type Get_DecoratorType(IAdditionalViewControlsRule additionalViewControlsRule)
        {
            ITypeInfo decoratorType =
                ModelAdditionalViewControlsRuleDomainLogic.GetDecorators().Where(info => info.Type.GetCustomAttributes(typeof(TypeDecorator), true).OfType<TypeDecorator>().Where(
                    decorator => decorator.IsDefaultDecorator).Count() > 0).SingleOrDefault();
            return decoratorType != null ? decoratorType.Type : typeof(NotAvaliableInThisPlatform);
        }
    
    }
}