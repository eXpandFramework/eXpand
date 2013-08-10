using System;
using System.Linq.Expressions;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security {
    public abstract class UpdateDecoratorTypeTypeConverterController<TReferenceConverter> : UpdateTypeConverterController<AdditionalViewControlsPermission, TReferenceConverter> where TReferenceConverter : XpandReferenceConverter {
        protected override Expression<Func<AdditionalViewControlsPermission, object>> Expression() {
            return permission => permission.DecoratorType;
        }
    }
}