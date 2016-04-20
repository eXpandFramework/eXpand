using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.StateMachine;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.StateMachine.Security {
    public class StateMachinePopulateController : PopulateController<IStateMachineTransitionPermission> {

        protected override string GetPredefinedValues(IModelMember wrapper) {
            var xpoStateMachines = ObjectSpace.QueryObjects<IStateMachine>();
            return string.Join(";", xpoStateMachines.Select(machine => machine.Name));
        }

        protected override Expression<Func<IStateMachineTransitionPermission, object>> GetPropertyName() {
            return permission => permission.StateMachineName;
        }
    }
}