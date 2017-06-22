using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Editors {

    public abstract class ReleaseSequencePopupWindowHelper {
        XafApplication _application;
        XPObjectSpace _objectSpace;
        readonly PopupWindowShowAction _showObjectAction = new PopupWindowShowAction(null, "ShowReleasedSequences", "");
        private ListView _listView;
        object _sequenceReleasedObjectKey;
        object _supportSequenceObject;
        public event EventHandler ViewShowing;

        protected void OnViewShowing(EventArgs e) {
            EventHandler handler = ViewShowing;
            handler?.Invoke(this, e);
        }

        public IModelPropertyEditor Model{ get; set; }

        void ShowObjectActionOnExecute(object sender, PopupWindowShowActionExecuteEventArgs popupWindowShowActionExecuteEventArgs) {
            Assign(popupWindowShowActionExecuteEventArgs, _supportSequenceObject);
        }

        public void Assign(PopupWindowShowActionExecuteEventArgs popupWindowShowActionExecuteEventArgs, object sequenceObject) {
            var sequenceReleasedObject = ((ISequenceReleasedObject)popupWindowShowActionExecuteEventArgs.PopupWindow.View.CurrentObject);
            _sequenceReleasedObjectKey = popupWindowShowActionExecuteEventArgs.PopupWindow.View.ObjectSpace.GetKeyValue(popupWindowShowActionExecuteEventArgs.PopupWindow.View.CurrentObject);
            var sequence = sequenceReleasedObject.Sequence;
            var supportSequenceObject = sequenceObject as ISupportSequenceObject;
            if (supportSequenceObject != null)
                supportSequenceObject.Sequence = sequence;
            else{
                Model.ModelMember.MemberInfo.SetValue(_supportSequenceObject,sequence);
            }
            _objectSpace.Delete(sequenceObject);
        }

        public PopupWindowShowAction ShowObjectAction => _showObjectAction;

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

        public ListView CreateListView(XafApplication application, IObjectSpace objectSpace, object supportSequenceObject) {
            var nestedObjectSpace = (XPNestedObjectSpace)objectSpace.CreateNestedObjectSpace();
            var objectType = XafTypesInfo.Instance.FindBussinessObjectType<ISequenceReleasedObject>();
            var collectionSource = application.CreateCollectionSource(nestedObjectSpace, objectType, application.FindListViewId(objectType));
            collectionSource.Criteria["ShowReleasedSequences"] = CriteriaOperator.Parse("TypeName=?", GetParameters(supportSequenceObject));
            return application.CreateListView(nestedObjectSpace, objectType, true);
        }

        private  string GetParameters(object sequenceObject){
            var supportSequenceObject = sequenceObject as ISupportSequenceObject;
            if (supportSequenceObject != null)
                return supportSequenceObject.Prefix + supportSequenceObject.GetType().GetTypeInfo().QueryXPClassInfo()
                           .FullName;
            var sequenceGeneratorAttributes = Model.ModelMember.MemberInfo.FindAttributes<SequenceGeneratorAttribute>().ToArray();
            if (!sequenceGeneratorAttributes.Any())
                throw new NotSupportedException($"Use {nameof(SequenceGeneratorAttribute)} to provide sequence name");
            return sequenceGeneratorAttributes.First().SequenceName;
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
                _showObjectAction.Application = _application;
                _showObjectAction.IsModal = true;
                _showObjectAction.CustomizePopupWindowParams += showObjectAction_CustomizePopupWindowParams;
                _showObjectAction.Execute += ShowObjectActionOnExecute;
                ShowObjectCore();
            } finally {
                _showObjectAction.Dispose();
            }
        }

        protected abstract void ShowObjectCore();

        public void Init(IObjectSpace objectSpace, object editingObject, XafApplication application) {
            _objectSpace = (XPObjectSpace)objectSpace;
            _supportSequenceObject = editingObject;
            _application = application;
        }
    }

}