using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

namespace Xpand.Persistent.Base.General {
    public static class ActionExtenions {
        public static void DoExecute(this ActionBase actionBase) {
            var simpleAction = actionBase as SimpleAction;
            if (simpleAction != null) {
                simpleAction.DoExecute();
                return;
            }
            var singleChoiceAction = actionBase as SingleChoiceAction;
            if (singleChoiceAction != null) {
                singleChoiceAction.DoExecute(singleChoiceAction.SelectedItem);
                return;
            }

            var popupWindowShowAction = actionBase as PopupWindowShowAction;
            if (popupWindowShowAction != null) {
                popupWindowShowAction.DoExecute((Window)popupWindowShowAction.Controller.Frame);
                return;
            }

            var parametrizedAction = actionBase as ParametrizedAction;
            if (parametrizedAction != null)
                parametrizedAction.DoExecute(parametrizedAction.Value);
        }
    }
}