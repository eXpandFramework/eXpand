using System;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Security;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Security {
    public class DecoratorTypeConverter:XpandReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(AdditionalViewControlsProviderDecorator);
        }
    }
}