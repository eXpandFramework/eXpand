using System;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Security {
    public class DecoratorTypeConverter:ReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(AdditionalViewControlsProviderDecorator);
        }
    }
}