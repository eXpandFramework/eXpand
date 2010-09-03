using System;
using System.Collections;
using DevExpress.Data.Filtering;
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
                shortcutEventArgs.Handled = true;
                var objectSpace = _application.CreateObjectSpace();
                object obj = GetObject(shortcut, modelDetailView, objectSpace);
                _detailView = _application.CreateDetailView(objectSpace, modelDetailView, true,obj);
                shortcutEventArgs.View = _detailView;
                    
            }
        }

        object GetObject(ViewShortcut shortcut, IModelDetailView modelDetailView, ObjectSpace objectSpace) {
            object objectKey = GetObjectKey(shortcut,modelDetailView.ModelClass.TypeInfo.Type,objectSpace);
            return GetObjectCore(modelDetailView, objectKey, objectSpace);
        }

        object GetObjectKey(ViewShortcut shortcut, Type type, ObjectSpace objectSpace) {
            var objectKey = GetObjectKey(objectSpace, type, shortcut);
            if (objectKey!= null)
                return objectKey;
            return shortcut.ObjectKey.StartsWith("@")
                            ? ParametersFactory.CreateParameter(shortcut.ObjectKey.Substring(1)).CurrentValue
                            : CriteriaWrapper.ParseCriteriaWithReadOnlyParameters(shortcut.ObjectKey,type);
        }

        object GetObjectKey(ObjectSpace objectSpace, Type type, ViewShortcut shortcut) {
            object objectKey= null;
            try {
                objectKey = objectSpace.GetObjectKey(type,shortcut.ObjectKey);
            }
            catch {
            }
            return objectKey;
        }

        object GetObjectCore(IModelDetailView modelView, object objectKey, ObjectSpace objectSpace) {
            Type type = modelView.ModelClass.TypeInfo.Type;
            object obj;

            if (typeof(IXPSimpleObject).IsAssignableFrom(type)){
                if (objectKey != null && !(objectKey is CriteriaOperator))
                    obj=objectSpace.GetObjectByKey(type,objectKey);
                else {
                    obj = objectSpace.FindObject(type, (CriteriaOperator) objectKey) ?? objectSpace.CreateObject(type);
                    if (!(objectSpace.IsNewObject(obj)))
                        _application.ViewShown += ApplicationOnViewShown;
                }
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