using System;
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Editors {

    public abstract class ReleaseSequencePopupWindowHelper {
        XafApplication _application;
        XPObjectSpace _objectSpace;
        readonly PopupWindowShowAction showObjectAction = new PopupWindowShowAction(null, "ShowReleasedSequences", "");
        private ListView _listView;
        object _sequenceReleasedObjectKey;
        ISupportSequenceObject _supportSequenceObject;
        public event EventHandler ViewShowing;

        protected void OnViewShowing(EventArgs e) {
            EventHandler handler = ViewShowing;
            if (handler != null) handler(this, e);
        }


        void ShowObjectActionOnExecute(object sender, PopupWindowShowActionExecuteEventArgs popupWindowShowActionExecuteEventArgs) {
            Assign(popupWindowShowActionExecuteEventArgs, _supportSequenceObject);
        }

        public void Assign(PopupWindowShowActionExecuteEventArgs popupWindowShowActionExecuteEventArgs, ISupportSequenceObject sequenceObject) {
            var sequenceReleasedObject = ((ISequenceReleasedObject)popupWindowShowActionExecuteEventArgs.PopupWindow.View.CurrentObject);
            _sequenceReleasedObjectKey = popupWindowShowActionExecuteEventArgs.PopupWindow.View.ObjectSpace.GetKeyValue(popupWindowShowActionExecuteEventArgs.PopupWindow.View.CurrentObject);
            var sequence = sequenceReleasedObject.Sequence;
            sequenceObject.Sequence = sequence;
        }

        public PopupWindowShowAction ShowObjectAction {
            get { return showObjectAction; }
        }
        private void DialogController_Cancelling(object sender, EventArgs e) {
            var controller = ((DialogController)sender);
            controller.CanCloseWindow = true;
            _objectSpace.Committing -= ObjectSpaceOnCommitting;
        }

        private void showObjectAction_CustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
            args.DialogController.Cancelling += DialogController_Cancelling;
            _objectSpace.Committing += ObjectSpaceOnCommitting;
            _listView = CreateListView(_application, _objectSpace);
            args.View = _listView;
        }

        public ListView CreateListView(XafApplication application, IObjectSpace objectSpace, ISupportSequenceObject supportSequenceObject) {
            var nestedObjectSpace = (XPNestedObjectSpace)objectSpace.CreateNestedObjectSpace();
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<ISequenceReleasedObject>();
            var collectionSource = application.CreateCollectionSource(nestedObjectSpace, objectType, application.FindListViewId(objectType));
            collectionSource.Criteria["ShowReleasedSequences"] = CriteriaOperator.Parse("TypeName=?", supportSequenceObject.Prefix + supportSequenceObject.ClassInfo.FullName);
            return application.CreateListView(nestedObjectSpace, objectType, true);
        }

        ListView CreateListView(XafApplication application, IObjectSpace objectSpace) {
            return CreateListView(application, objectSpace, _supportSequenceObject);
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs e) {
            Capture(_objectSpace);
        }

        public void Capture(XPObjectSpace objectSpace) {
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<ISequenceReleasedObject>();
            var objectByKey = objectSpace.GetObjectByKey(objectType, _sequenceReleasedObjectKey);
            if (objectByKey != null)
                objectSpace.Delete(objectByKey);
        }

        public void ShowObject() {
            OnViewShowing(EventArgs.Empty);
            try {
                showObjectAction.Application = _application;
                showObjectAction.IsModal = true;
                showObjectAction.CustomizePopupWindowParams += showObjectAction_CustomizePopupWindowParams;
                showObjectAction.Execute += ShowObjectActionOnExecute;
                ShowObjectCore();
            } finally {
                showObjectAction.Dispose();
            }
        }

        protected abstract void ShowObjectCore();

        public void Init(IObjectSpace objectSpace, ISupportSequenceObject editingObject, XafApplication application) {
            _objectSpace = (XPObjectSpace)objectSpace;
            _supportSequenceObject = editingObject;
            _application = application;
        }
    }
}