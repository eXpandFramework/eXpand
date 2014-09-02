﻿using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.SystemModule.Actions {
    [ModelAbstractClass]
    public interface IModelActionState:IModelAction {
        [DefaultValue(true)]
        [Category("eXpand")]
        bool Active { get; set; }
    }
    public class GlobalActionStateController:Controller,IModelExtender {
        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            var modelActionStates = Application.Model.ActionDesign.Actions.Cast<IModelActionState>();
            foreach (var modelActionState in modelActionStates.Where(state => !state.Active)) {
                var controllerType = Application.TypesInfo.FindTypeInfo(modelActionState.Controller.Name).Type;
                var actionBase = Frame.GetController(controllerType).Actions[modelActionState.Id];
                actionBase.Active.BeginUpdate();
                actionBase.Active["ModelActiveAttribute"] = false;
                actionBase.Active.EndUpdate();
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelAction,IModelActionState>();
        }
    }
}
