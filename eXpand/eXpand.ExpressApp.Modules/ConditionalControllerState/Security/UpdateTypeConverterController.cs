using System;
using System.Linq.Expressions;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.ConditionalControllerState.Security {
    public class UpdateTypeConverterController :
        UpdateTypeConverterController<ControllerStateRulePermission, ControllerTypeConverter> {
        protected override Expression<Func<ControllerStateRulePermission, object>> Expression() {
            return permission => permission.ControllerType;
        }
        }
}