using System;
using System.Linq.Expressions;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security {
    public abstract class UpdateControlTypeTypeConverterController<TReferenceConverter> : UpdateTypeConverterController<AdditionalViewControlsPermission, TReferenceConverter> where TReferenceConverter : XpandReferenceConverter {
        protected override Expression<Func<AdditionalViewControlsPermission, object>> Expression()
        {
            return permission => permission.ControlType;
        }        
    }
}