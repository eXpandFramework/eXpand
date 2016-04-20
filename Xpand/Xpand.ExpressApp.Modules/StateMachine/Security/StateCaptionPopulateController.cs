using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.StateMachine;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.StateMachine.Security{
    public class StateCaptionPopulateController:PopulateController<IStateMachineTransitionPermission>{
        protected override string GetPredefinedValues(IModelMember wrapper){
            var stateMachineTransitionPermission = ((IStateMachineTransitionPermission)View.CurrentObject);
            return string.Join(";", GetStateCaptions(stateMachineTransitionPermission.StateMachineName));
        }

        IEnumerable<string> GetStateCaptions(string stateMachineName) {
            return ObjectSpace.QueryObjects<IState>(state => state.StateMachine.Name == stateMachineName).Select(
                state => state.Caption);
        }

        protected override IEnumerable<string> RefreshingProperties(){
            yield return ((IStateMachineTransitionPermission) View.CurrentObject).GetPropertyName(permission => permission.StateMachineName);
        }

        protected override Expression<Func<IStateMachineTransitionPermission, object>> GetPropertyName(){
            return permission => permission.StateCaption;
        }
    }
}