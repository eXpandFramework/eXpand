using System;
using System.Linq.Expressions;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Security {
    public abstract class UpdateControlTypeTypeConverterController<TReferenceConverter> : UpdateTypeConverterController<AdditionalViewControlsPermission, TReferenceConverter> where TReferenceConverter : ReferenceConverter {
        protected override Expression<Func<AdditionalViewControlsPermission, object>> Expression()
        {
            return permission => permission.ControlType;
        }        
    }
}