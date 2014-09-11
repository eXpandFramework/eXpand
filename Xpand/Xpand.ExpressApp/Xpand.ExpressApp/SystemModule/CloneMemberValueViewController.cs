using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using System.Linq;
using DevExpress.ExpressApp.Xpo;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelMemberCloneValue : IModelNode {
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool CloneValue { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberCloneValue), "ModelMember")]
    public interface IModelCommonMemberViewItemCloneValue : IModelMemberCloneValue {

    }

    public class CloneMemberValueViewController : ViewController<ObjectView>,IModelExtender {
        object _previousObject;
        NewObjectViewController _newObjectViewController;
        ModificationsController _modificationsController;
        

        protected override void OnActivated() {
            base.OnActivated();
            _previousObject = null;
            if (Enabled()) {
                _newObjectViewController = Frame.GetController<NewObjectViewController>();
                _newObjectViewController.ObjectCreating+=NewObjectViewControllerOnObjectCreating;
                _newObjectViewController.ObjectCreated += newObjectViewController_ObjectCreated;
                _modificationsController = Frame.GetController<ModificationsController>();
                _modificationsController.SaveAndNewAction.Executing += SaveAndNewAction_Executing;
            }
        }

        void NewObjectViewControllerOnObjectCreating(object sender, ObjectCreatingEventArgs objectCreatingEventArgs) {
            if (_previousObject == null&&(objectCreatingEventArgs.ShowDetailView&&View is DetailView))
                _previousObject = View.CurrentObject;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (Enabled()) {
                _previousObject = null;
                _newObjectViewController.ObjectCreating += NewObjectViewControllerOnObjectCreating;
                _newObjectViewController.ObjectCreated += newObjectViewController_ObjectCreated;
                _modificationsController.SaveAndNewAction.Executing -= SaveAndNewAction_Executing;
            }
        }

        private void newObjectViewController_ObjectCreated(Object sender, ObjectCreatedEventArgs e) {
            if (_previousObject != null) {
                MapMemberValues(_previousObject, e.CreatedObject);
            }
        }

        private IEnumerable<IModelCommonMemberViewItemCloneValue> CommonMemberViewItems() {
            var objectStates = View.Model is IModelListView 
                ? ((ListView) View).Model.Columns.Cast<IModelCommonMemberViewItemCloneValue>() 
                : ((IModelDetailView) View.Model).Items.OfType<IModelCommonMemberViewItemCloneValue>();
            return objectStates.Where(state => state.CloneValue);
        }

        bool Enabled() {
            return CommonMemberViewItems().Any();
        }

        void MapMemberValues(object previousObject, object currentObject) {
            var memberInfos = CommonMemberViewItems().Cast<IModelMemberViewItem>().Select(item => item.ModelMember.MemberInfo);
            foreach (var member in memberInfos) {
                var value = member.GetValue(previousObject);
                if (member.MemberTypeInfo.IsPersistent)
                    value = XPObjectSpace.FindObjectSpaceByObject(currentObject).GetObject(value);
                
                member.SetValue(currentObject, value);
            }
        }

        void SaveAndNewAction_Executing(object sender, CancelEventArgs e) {
            _previousObject = View.CurrentObject;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember,IModelMemberCloneValue>();
            extenders.Add<IModelPropertyEditor, IModelCommonMemberViewItemCloneValue>();
            extenders.Add<IModelColumn, IModelCommonMemberViewItemCloneValue>();
        }
    }
}
