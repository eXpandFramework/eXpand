using System;
using System.Collections;
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
            IModelDetailView modelDetailView = GetModelView(shortcut);
            if ((modelDetailView != null)) {
                object objectKey = GetObjectKey(shortcut);
                var objectSpace = _application.CreateObjectSpace();
                shortcutEventArgs.Handled = true;
                object obj = GetObject(modelDetailView, objectKey, objectSpace);
                _detailView = _application.CreateDetailView(objectSpace, modelDetailView, true,obj);
                shortcutEventArgs.View = _detailView;
                if (objectKey== null)
                    _application.ViewShown+=ApplicationOnViewShown;
            }
        }

        object GetObjectKey(ViewShortcut shortcut) {
            object objectKey = null;
            if (shortcut.ObjectKey.StartsWith("@")) {
                objectKey =
                    ParametersFactory.CreateParameter(shortcut.ObjectKey.Substring(1)).CurrentValue;
            }
            return objectKey;
        }

        object GetObject(IModelDetailView modelView, object objectKey, ObjectSpace objectSpace) {
            Type type = modelView.ModelClass.TypeInfo.Type;
            object obj;
            if (typeof(IXPSimpleObject).IsAssignableFrom(type)){
                if (objectKey!= null)
                    obj=objectSpace.GetObjectByKey(type,objectKey);
                else
                    obj = objectSpace.FindObject(type, null) ?? objectSpace.CreateObject(type);
            }
            else{
                obj = Activator.CreateInstance(type);
            }
            return obj;
        }

        IModelDetailView GetModelView(ViewShortcut shortcut) {
            return _application.Model.Views.OfType<IModelDetailView>().Where(v => v.Id == shortcut.ViewId).FirstOrDefault();
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