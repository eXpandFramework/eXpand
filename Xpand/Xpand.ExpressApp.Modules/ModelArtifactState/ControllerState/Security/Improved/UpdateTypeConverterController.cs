using System;
using System.Linq.Expressions;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Security.Improved {
    public class UpdateTypeConverterController : UpdateTypeConverterController<ControllerStateOperationPermissionData, ControllerTypeConverter> {
        protected override Expression<Func<ControllerStateOperationPermissionData, object>> Expression() {
            return permission => permission.ControllerType;
        }
    }
}