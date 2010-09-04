using System;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Security {
    public class DecoratorTypeConverter:XpandReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(AdditionalViewControlsProviderDecorator);
        }
    }
}