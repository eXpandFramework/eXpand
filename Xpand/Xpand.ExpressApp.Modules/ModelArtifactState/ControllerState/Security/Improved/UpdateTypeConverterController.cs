using System;
using System.Linq.Expressions;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Security.Improved {
    public class UpdateTypeConverterController : UpdateTypeConverterController<ModelArtifactState.ControllerState.Security.Improved.ControllerStateRulePermission, ControllerTypeConverter> {
        protected override Expression<Func<ModelArtifactState.ControllerState.Security.Improved.ControllerStateRulePermission, object>> Expression() {
            return permission => permission.ControllerType;
        }
    }
}