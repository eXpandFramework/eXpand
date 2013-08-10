using System;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Security {
    public class DecoratorTypeConverter:XpandReferenceConverter {
        protected override Type GetTypeInfo() {
            return typeof(AdditionalViewControlsProviderDecorator);
        }
    }
}