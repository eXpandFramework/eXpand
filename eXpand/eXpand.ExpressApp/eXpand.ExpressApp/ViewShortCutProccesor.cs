using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using System.Linq;

namespace eXpand.ExpressApp {
    public class ViewShortCutProccesor {
        readonly XafApplication _application;

        public ViewShortCutProccesor(XafApplication application) {
            _application = application;
            
        }

        public void Proccess(CustomProcessShortcutEventArgs shortcutEventArgs) {
            var shortcut = shortcutEventArgs.Shortcut;
            if (shortcut.ObjectKey.StartsWith("@"))
                shortcut.ObjectKey =
                    ParametersFactory.CreateParameter(shortcut.ObjectKey.Substring(1)).CurrentValue.ToString();

            var baseViewInfoNodeWrapper = new ApplicationNodeWrapper(_application.Model).Views.FindViewById(shortcut.ViewId);
            if (string.IsNullOrEmpty(shortcut.ObjectKey) && (baseViewInfoNodeWrapper is DetailViewInfoNodeWrapper)) {
                var objectSpace = _application.CreateObjectSpace();
                shortcutEventArgs.Handled = true;
                shortcutEventArgs.View = _application.CreateDetailView(objectSpace,
                                                                       objectSpace.CreateObject(
                                                                           baseViewInfoNodeWrapper.BusinessObjectType));
            }
        }
    }
}