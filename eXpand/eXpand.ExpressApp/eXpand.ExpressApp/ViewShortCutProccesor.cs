using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;

namespace eXpand.ExpressApp {
    public class ViewShortCutProccesor {
        readonly XafApplication _application;
        DetailView _detailView;

        public ViewShortCutProccesor(XafApplication application) {
            _application = application;
            
        }

        public void Proccess(CustomProcessShortcutEventArgs shortcutEventArgs) {
            var shortcut = shortcutEventArgs.Shortcut;
            if (shortcut.ObjectKey.StartsWith("@"))
                shortcut.ObjectKey =
                    ParametersFactory.CreateParameter(shortcut.ObjectKey.Substring(1)).CurrentValue.ToString();

            var baseViewInfoNodeWrapper = _application.Model.Views.OfType<IModelDetailView>().Where(v => v.Id == shortcut.ViewId).FirstOrDefault();
            if (string.IsNullOrEmpty(shortcut.ObjectKey) && (baseViewInfoNodeWrapper != null)) {
                var objectSpace = _application.CreateObjectSpace();
                shortcutEventArgs.Handled = true;
                Type type = baseViewInfoNodeWrapper.ModelClass.TypeInfo.Type;
                object obj;
                if (typeof(IXPSimpleObject).IsAssignableFrom(type)){
                    obj = objectSpace.FindObject(type, null) ?? objectSpace.CreateObject(type);
                }
                else{
                    obj = Activator.CreateInstance(type);
                }
                _detailView = _application.CreateDetailView(objectSpace,obj);
                shortcutEventArgs.View = _detailView;
                _application.ViewShown+=ApplicationOnViewShown;
            }
        }

        void ApplicationOnViewShown(object sender, ViewShownEventArgs e) {
            if (_detailView == null) return;
            ObjectSpace objectSpace = _application.ObjectSpaceProvider.CreateObjectSpace();
            IList objects = objectSpace.GetObjects(_detailView.ObjectTypeInfo.Type);
            var standaloneOrderProvider = new StandaloneOrderProvider(objectSpace, objects);
            var orderProviderSource = new OrderProviderSource { OrderProvider = standaloneOrderProvider };
            e.TargetFrame.GetController<RecordsNavigationController>().OrderProviderSource=orderProviderSource;
            _application.ViewShown-=ApplicationOnViewShown;
            _detailView = null;
        }
    }
}