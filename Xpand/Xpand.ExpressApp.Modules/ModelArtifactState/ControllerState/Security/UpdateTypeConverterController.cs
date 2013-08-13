using System;
using System.Linq.Expressions;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ModelArtifactState.ControllerState.Security {
    public class UpdateTypeConverterController :
        UpdateTypeConverterController<ControllerStateRulePermission, ControllerTypeConverter> {
        protected override Expression<Func<ControllerStateRulePermission, object>> Expression() {
            return permission => permission.ControllerType;
        }
    }
}