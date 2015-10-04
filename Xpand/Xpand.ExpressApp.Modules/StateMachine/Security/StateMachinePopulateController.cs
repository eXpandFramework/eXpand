using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.StateMachine.Xpo;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.StateMachine.Security {
    public class StateMachinePopulateController : PopulateController<IStateMachineTransitionPermission> {

        protected override string GetPredefinedValues(IModelMember wrapper) {
            IList<XpoStateMachine> xpoStateMachines = ObjectSpace.GetObjects<XpoStateMachine>(null);
            return xpoStateMachines.Select(machine => machine.Name).AggregateWith(";");
        }

        protected override Expression<Func<IStateMachineTransitionPermission, object>> GetPropertyName() {
            return permission => permission.StateMachineName;
        }
    }
}