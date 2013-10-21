using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Model;
using Xpand.Utils.Linq;
using Xpand.Utils.Helpers;
using Fasterflect;


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
            var modelDetailView = GetModelDetailView(shortcut);
            if ((modelDetailView != null && IsEnable(modelDetailView))) {
                if (CanCreate(modelDetailView.ModelClass.TypeInfo)) {
                    shortcutEventArgs.Handled = true;
                    var objectSpace = _application.CreateObjectSpace(modelDetailView.ModelClass.TypeInfo.Type);
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
            var modelDetailViewProccessViewShortcuts = modelDetailView as IModelDetailViewProccessViewShortcuts;
            return modelDetailViewProccessViewShortcuts != null && (modelDetailViewProccessViewShortcuts).ViewShortcutProccesor;
        }

        object GetObject(ViewShortcut shortcut, IModelDetailView modelDetailView, IObjectSpace objectSpace) {
            object objectKey = GetObjectKey(shortcut, modelDetailView.ModelClass.TypeInfo.Type, objectSpace);
            return GetObjectCore(modelDetailView, objectKey, objectSpace);
        }

        object GetObjectKey(ViewShortcut shortcut, Type type, IObjectSpace objectSpace) {
            return !string.IsNullOrEmpty(shortcut.ObjectKey) && shortcut.ObjectKey.StartsWith("@")
                       ? CriteriaWrapper.ParseCriteriaWithReadOnlyParameters(shortcut.ObjectKey, type)
                       : GetObjectKey(objectSpace, type, shortcut);
        }

        object GetObjectKey(IObjectSpace objectSpace, Type type, ViewShortcut shortcut) {
            var objectKeyString = shortcut.ObjectKey ;
            if (shortcut.ContainsKey("Criteria"))
                objectKeyString = shortcut["Criteria"];
            return string.IsNullOrEmpty(objectKeyString) ? null : GetObjectKeyCore(objectSpace, type, objectKeyString);
        }

        object GetObjectKeyCore(IObjectSpace objectSpace, Type type, string objectKeyString) {
            object objectKey = null;
            var criteriaOperator = CriteriaOperator.TryParse(objectKeyString);
            if (criteriaOperator != null)
                return criteriaOperator;
            try {
                objectKey = objectSpace.GetObjectKey(type, objectKeyString);
            }
            catch {
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
                    if (!(objectSpace.IsNewObject(obj))) {
                        _application.ViewShown +=ApplicationOnViewShown;
                    }
                }
            } else {
                obj = (type.GetConstructor(new[] { typeof(Session) }) != null) ? objectSpace.CreateObject(type) : type.CreateInstance();
            }
            return obj;
        }

        void ApplicationOnViewShown(object sender, ViewShownEventArgs e) {
            if (_detailView == null) return;
            IObjectSpace objectSpace = _application.ObjectSpaceProvider.CreateObjectSpace();
            IList objects = objectSpace.GetObjects(_detailView.ObjectTypeInfo.Type);
            var standaloneOrderProvider = new StandaloneOrderProvider(objectSpace, objects);
            var orderProviderSource = new OrderProviderSource { OrderProvider = standaloneOrderProvider };
            e.TargetFrame.GetController<RecordsNavigationController>().OrderProviderSource = orderProviderSource;
            _application.ViewShown -= ApplicationOnViewShown;
            _detailView = null;
        }

        IModelDetailView GetModelDetailView(ViewShortcut shortcut) {
            return _application.Model.Views.OfType<IModelDetailView>().FirstOrDefault(v => v.Id == shortcut.ViewId);
        }
    }

    public class ViewShortcutProccesorControler : WindowController {
        readonly Dictionary<IModelNavigationItem,string> _items=new Dictionary<IModelNavigationItem, string>();

        public ViewShortcutProccesorControler() {
            TargetWindowType=WindowType.Main;
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            var showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            showNavigationItemController.CustomInitializeItems += ShowNavigationItemControllerOnCustomInitializeItems;
            showNavigationItemController.ItemsInitialized += ShowNavigationItemControllerOnItemsInitialized;
//            showNavigationItemController.Activated+=ShowNavigationItemControllerOnActivated;
        }



        void ShowNavigationItemControllerOnCustomInitializeItems(object sender, HandledEventArgs handledEventArgs) {
            ((ShowNavigationItemController)sender).CustomInitializeItems-= ShowNavigationItemControllerOnCustomInitializeItems;
            handledEventArgs.Handled = false;
            var items = ((IModelApplicationNavigationItems) Application.Model).NavigationItems.Items.GetItems<IModelNavigationItem>(item => item.Items).Where(CannotConvertCriteriaValueToObjectKeyType);
            foreach (var modelNavigationItem in items) {
                _items.Add(modelNavigationItem, modelNavigationItem.ObjectKey);
                modelNavigationItem.ObjectKey = null;
            }
        }

        bool CannotConvertCriteriaValueToObjectKeyType(IModelNavigationItem modelNavigationItem) {
            return !string.IsNullOrEmpty(modelNavigationItem.ObjectKey) && (((modelNavigationItem.View != null &&
                   modelNavigationItem.View.AsObjectView != null && !modelNavigationItem.ObjectKey.CanChange(
                       modelNavigationItem.View.AsObjectView.ModelClass.TypeInfo.KeyMember.MemberType)))&&!modelNavigationItem.ObjectKey.StartsWith("@"));
        }

        void ShowNavigationItemControllerOnItemsInitialized(object sender, EventArgs eventArgs) {
            var showNavigationItemController = ((ShowNavigationItemController) sender);
            var choiceActionItems = showNavigationItemController.ShowNavigationItemAction.Items.GetItems<ChoiceActionItem>(item => item.Items).Where(item => _items.ContainsKey((IModelNavigationItem) item.Model));
            showNavigationItemController.ItemsInitialized -= ShowNavigationItemControllerOnItemsInitialized;
            foreach (var item in _items) {
                item.Key.ObjectKey = item.Value;
                var choiceActionItem = choiceActionItems.First(actionItem => actionItem.Model == item.Key);
                var viewShortcut = ((ViewShortcut) choiceActionItem.Data);
                viewShortcut.Add("Criteria",item.Value);
//                choiceActionItem.Data = new ViewShortcut(viewShortcut.ObjectClass, viewShortcut.ObjectKey, viewShortcut.ViewId, viewShortcut.ScrollPosition, item.Value);
//                viewShortcut.ObjectKey = item.Value;
            }
        }

    }

//    public class ViewShortcut : DevExpress.ExpressApp.ViewShortcut {
//        readonly string _objectKeyEx;
//
//        public ViewShortcut(Type objectClass, string objectKey, string viewId, Point scrollPosition,string objectKeyEx) : base(objectClass, objectKey, viewId, scrollPosition) {
//            _objectKeyEx = objectKeyEx;
//        }
//
//        public string ObjectKeyEx {
//            get { return _objectKeyEx; }
//        }
//    }
}