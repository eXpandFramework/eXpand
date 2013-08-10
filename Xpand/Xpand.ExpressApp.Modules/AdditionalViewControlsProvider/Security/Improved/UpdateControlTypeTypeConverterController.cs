using System;
using System.Linq.Expressions;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Security.Improved {
    public abstract class UpdateControlTypeTypeConverterController<TReferenceConverter> : UpdateTypeConverterController<AdditionalViewControlsOperationPermissionData, TReferenceConverter> where TReferenceConverter : XpandReferenceConverter {
        protected override Expression<Func<AdditionalViewControlsOperationPermissionData, object>> Expression() {
            return permission => permission.ControlType;
        }
    }
}