using System;
using System.Collections;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Model;


namespace Xpand.ExpressApp {
    public class ViewShortCutProccesor {
        readonly XafApplication _application;
        DetailView _detailView;

        public ViewShortCutProccesor(XafApplication application) {
            _application = application;

        }

        public void Proccess(CustomProcessShortcutEventArgs shortcutEventArgs) {
            if (shortcutEventArgs.Handled) return;
            var shortcut = shortcutEventArgs.Shortcut;
            IModelDetailView modelDetailView = GetModelDetailView(shortcut);
            if ((modelDetailView != null && IsEnable(modelDetailView))) {
                if (CanCreate(modelDetailView.ModelClass.TypeInfo)) {
                    shortcutEventArgs.Handled = true;
                    var objectSpace = _application.CreateObjectSpace();
                    object obj = GetObject(shortcut, modelDetailView, objectSpace);
                    _detailView = _application.CreateDetailView(objectSpace, modelDetailView, true, obj);
                    shortcutEventArgs.View = _detailView;
                }
            }
        }

        bool CanCreate(ITypeInfo typeInfo) {
            if (!(typeInfo.IsPersistent)) {
                return (typeInfo.Type.GetConstructor(Type.EmptyTypes) != null) || (typeInfo.Type.GetConstructor(new[] { typeof(Session) }) != null);
            }
            return true;
        }

        bool IsEnable(IModelDetailView modelDetailView) {
            return ((IModelDetailViewProccessViewShortcuts)modelDetailView).ViewShortcutProccesor;
        }

        object GetObject(ViewShortcut shortcut, IModelDetailView modelDetailView, IObjectSpace objectSpace) {
            object objectKey = GetObjectKey(shortcut, modelDetailView.ModelClass.TypeInfo.Type, objectSpace);
            return GetObjectCore(modelDetailView, objectKey, objectSpace);
        }

        object GetObjectKey(ViewShortcut shortcut, Type type, IObjectSpace objectSpace) {
            var objectKey = GetObjectKey(objectSpace, type, shortcut);
            if (objectKey != null)
                return objectKey;
            return shortcut.ObjectKey.StartsWith("@")
                            ? ParametersFactory.CreateParameter(shortcut.ObjectKey.Substring(1)).CurrentValue
                            : CriteriaWrapper.ParseCriteriaWithReadOnlyParameters(shortcut.ObjectKey, type);
        }

        object GetObjectKey(IObjectSpace objectSpace, Type type, ViewShortcut shortcut) {
            object objectKey = null;
            if (string.IsNullOrEmpty(shortcut.ObjectKey))
                return objectKey;

            try {
                objectKey = objectSpace.GetObjectKey(type, shortcut.ObjectKey);
            } catch {
            }
            return objectKey;
        }

        protected virtual object GetObjectCore(IModelDetailView modelView, object objectKey, IObjectSpace objectSpace) {
            Type type = modelView.ModelClass.TypeInfo.Type;
            object obj;

            if (XafTypesInfo.CastTypeToTypeInfo(type).IsPersistent) {
                if (objectKey != null && !(objectKey is CriteriaOperator))
                    obj = objectSpace.GetObjectByKey(type, objectKey);
                else {
                    obj = objectSpace.FindObject(type, (CriteriaOperator)objectKey) ?? objectSpace.CreateObject(type);
                    if (!(objectSpace.IsNewObject(obj)))
                        ((ISupportAfterViewShown)_application).AfterViewShown += OnAfterViewShown;
                }
            } else {
                obj = (type.GetConstructor(new[] { typeof(Session) })!=null)  ? objectSpace.CreateObject(type) : Activator.CreateInstance(type);
            }
            return obj;
        }

        void OnAfterViewShown(object sender, ViewShownEventArgs e) {
            if (_detailView == null) return;
            IObjectSpace objectSpace = _application.ObjectSpaceProvider.CreateObjectSpace();
            IList objects = objectSpace.GetObjects(_detailView.ObjectTypeInfo.Type);
            var standaloneOrderProvider = new StandaloneOrderProvider(objectSpace, objects);
            var orderProviderSource = new OrderProviderSource { OrderProvider = standaloneOrderProvider };
            e.TargetFrame.GetController<RecordsNavigationController>().OrderProviderSource = orderProviderSource;
            ((ISupportAfterViewShown) _application).AfterViewShown-=OnAfterViewShown;
            _detailView = null;
        }

        IModelDetailView GetModelDetailView(ViewShortcut shortcut) {
            return _application.Model.Views.OfType<IModelDetailView>().Where(v => v.Id == shortcut.ViewId).FirstOrDefault();
        }


    }
}