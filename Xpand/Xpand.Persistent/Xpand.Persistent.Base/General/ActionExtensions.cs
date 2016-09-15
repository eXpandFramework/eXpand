using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    public static class ActionExtensions {
        public static void ActivateKey(this ActionBase action, string key) {
            action.Active.Changed += (sender, args) =>{
                if (action.Active.Contains(key) && !action.Active[key]) {
                    action.Active.BeginUpdate();
                    action.Active.SetItemValue(key, true);
                    action.Active.EndUpdate();
                }
            };
        }

        public static IModelAction Model(this ActionBase actionBase){
            return CaptionHelper.ApplicationModel.ActionDesign.Actions[actionBase.Id];
        }

        public static IModelBaseChoiceActionItem Model(this ChoiceActionItem choiceActionItem){
            var modelAction = ((SingleChoiceAction) choiceActionItem.GetPropertyValue("Owner")).Model;
            var nodePath = choiceActionItem.GetIdPath();
            return (IModelBaseChoiceActionItem) modelAction.ChoiceActionItems.FindNodeByPath(nodePath);
        }

        public static bool DoExecute(this ActionBase actionBase) {
            if (!actionBase.Active||!actionBase.Enabled)
                return false;
            var simpleAction = actionBase as SimpleAction;
            simpleAction?.DoExecute();
            var singleChoiceAction = actionBase as SingleChoiceAction;
            singleChoiceAction?.DoExecute(singleChoiceAction.SelectedItem);

            var popupWindowShowAction = actionBase as PopupWindowShowAction;
            popupWindowShowAction?.DoExecute((Window)popupWindowShowAction.Controller.Frame);

            var parametrizedAction = actionBase as ParametrizedAction;
            parametrizedAction?.DoExecute(parametrizedAction.Value);
            return true;
        }
    }
}