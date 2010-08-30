using System;
using System.Linq.Expressions;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Security {
    public abstract class UpdateDecoratorTypeTypeConverterController<TReferenceConverter> : UpdateTypeConverterController<AdditionalViewControlsPermission, TReferenceConverter> where TReferenceConverter : ReferenceConverter {
        protected override Expression<Func<AdditionalViewControlsPermission, object>> Expression()
        {
            return permission => permission.DecoratorType;
        }        
    }
}