using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using System.Linq;
using DevExpress.ExpressApp.Model;

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

            var detailViewModel = _application.Model.Views.OfType<IModelDetailView>().Where(v => v.Id == shortcut.ViewId).FirstOrDefault();
            if (string.IsNullOrEmpty(shortcut.ObjectKey) && (detailViewModel != null) && 
                !detailViewModel.ModelClass.TypeInfo.IsPersistent) 
            {
                var objectSpace = _application.CreateObjectSpace();
                shortcutEventArgs.Handled = true;
                shortcutEventArgs.View = _application.CreateDetailView(
                    objectSpace,
                    objectSpace.CreateObject(detailViewModel.ModelClass.TypeInfo.Type));
            }
        }
    }
}