﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    public static class ActionExtensions {

        public static IModelBaseChoiceActionItem Model(this ChoiceActionItem choiceActionItem){
            var modelAction = ((SingleChoiceAction) choiceActionItem.GetPropertyValue("Owner")).Model;
            var nodePath = choiceActionItem.GetIdPath();
            return (IModelBaseChoiceActionItem) modelAction.ChoiceActionItems.FindNodeByPath(nodePath);
        }

        public static bool DoExecute(this ActionBase actionBase) {
            if (!actionBase.Active||!actionBase.Enabled)
                return false;
            var simpleAction = actionBase as SimpleAction;
            if (simpleAction != null) {
                simpleAction.DoExecute();
            }
            var singleChoiceAction = actionBase as SingleChoiceAction;
            if (singleChoiceAction != null) {
                singleChoiceAction.DoExecute(singleChoiceAction.SelectedItem);
            }

            var popupWindowShowAction = actionBase as PopupWindowShowAction;
            if (popupWindowShowAction != null) {
                popupWindowShowAction.DoExecute((Window)popupWindowShowAction.Controller.Frame);
            }

            var parametrizedAction = actionBase as ParametrizedAction;
            if (parametrizedAction != null)
                parametrizedAction.DoExecute(parametrizedAction.Value);
            return true;
        }
    }
}