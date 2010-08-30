using System;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Security;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Security {
    public class DecoratorTypeConverter:ReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(AdditionalViewControlsProviderDecorator);
        }
    }
}