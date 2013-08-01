using System;
using System.Linq.Expressions;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ConditionalControllerState.Security.Improved {
    public class UpdateTypeConverterController : UpdateTypeConverterController<ControllerStateRulePermission, ControllerTypeConverter> {
        protected override Expression<Func<ControllerStateRulePermission, object>> Expression() {
            return permission => permission.ControllerType;
        }
    }
}