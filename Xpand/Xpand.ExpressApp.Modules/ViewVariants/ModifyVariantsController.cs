using System;
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.Persistent.Base;
using System.Linq;

namespace Xpand.ExpressApp.ViewVariants {
    public interface IModelClassViewClonable {
        [Description("Determines if the clone action will be shown for the view")]
        [Category("eXpand.ViewVariants")]
        bool IsViewClonable { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassViewClonable), "ModelClass")]
    public interface IModelListViewViewClonable : IModelClassViewClonable {
    }

    public class ModifyVariantsController : ViewController<ListView>, IModelExtender {
        private const string IsClonable = "IsViewClonable";
        private const string Rename = "Rename";
        private const string Clone1 = "Clone";
        private const string Delete = "Delete";
        
        readonly SingleChoiceAction _viewVariantsChoiceAction;
        ChangeVariantMainWindowController _changeVariantMainWindowController;
        ChangeVariantController _changeVariantController;
        IModelListView _rootListView;
        VariantInfo _currentVariantInfo;
        bool _isViewClonable;


        public ModifyVariantsController() {
            _viewVariantsChoiceAction = new SingleChoiceAction(this, "ModifyViewVariants", PredefinedCategory.View);
            _viewVariantsChoiceAction.Execute += ViewVariantsChoiceActionOnExecute;
            _viewVariantsChoiceAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            _viewVariantsChoiceAction.Items.Add(new ChoiceActionItem(Clone1, Clone1));
            _viewVariantsChoiceAction.Items.Add(new ChoiceActionItem(Rename, Rename));
            _viewVariantsChoiceAction.Items.Add(new ChoiceActionItem(Delete, Delete));
            TargetViewNesting = Nesting.Root;
        }

        void ViewVariantsChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            View.SaveModel();
            if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, Clone1)) {
                CloneView(singleChoiceActionExecuteEventArgs);
            } else if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, Rename)) {
                RenameView();
            } else if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, Delete)) {
                DeleteView();
            }
        }

        void DeleteView() {
            DeleteViewCore(_rootListView.Id);
        }

        void CloneView(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            var objectSpace = Application.CreateObjectSpace(typeof(ViewCloner));
            DetailView detailView = Application.CreateDetailView(objectSpace, objectSpace.CreateObject<ViewCloner>());
            detailView.ViewEditMode = ViewEditMode.Edit;
            detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.XpandViewVariants, "CreateViewCaption");
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = detailView;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            dialogController.Accepting += (o, args) => CloneView((ViewCloner)((DialogController)o).Frame.View.CurrentObject);
            singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }

        void RenameView() {
            var objectSpace = Application.CreateObjectSpace(typeof(ViewCloner));
            var viewCloner = objectSpace.CreateObject<ViewCloner>();
            viewCloner.Caption = _changeVariantController.ChangeVariantAction.SelectedItem.Caption;
            var detailView = Application.CreateDetailView(objectSpace, viewCloner);
            detailView.ViewEditMode = ViewEditMode.Edit;
            detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.XpandViewVariants, "RenameViewToolTip");
            var parameters = new ShowViewParameters(detailView) { TargetWindow = TargetWindow.NewModalWindow };
            var controller = new DialogController();
            controller.AcceptAction.Execute += RenameViewActionOnExecute;
            parameters.Controllers.Add(controller);
            Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(null, null));
        }

        private void RenameViewActionOnExecute(object sender, SimpleActionExecuteEventArgs args) {
            var viewCloner = ((ViewCloner)args.CurrentObject);
            var modelVariants = ((IModelViewVariants)_rootListView).Variants;
            CreateVariantNode(viewCloner.Caption, modelVariants);
            DeleteViewCore(viewCloner.Caption);
        }

        void DeleteViewCore(string redirectToViewId) {
            var rootModelVariant = ((IModelViewVariants)_rootListView).Variants[_currentVariantInfo.Id];
            rootModelVariant.Remove();
            Application.Model.Views[_currentVariantInfo.ViewID].Remove();
            _changeVariantController.RefreshVariantsAction();
            _changeVariantMainWindowController.ChangeCurrentViewToVariant(Frame, redirectToViewId);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_isViewClonable)
                _changeVariantController.ChangeVariantAction.SelectedItemChanged -= ChangeVariantActionOnSelectedItemChanged;
        }

        void ChangeVariantActionOnSelectedItemChanged(object sender, EventArgs eventArgs) {
            if (_changeVariantController.ChangeVariantAction.SelectedItem != null) {
                _rootListView = FindRootListView();
                _currentVariantInfo = (VariantInfo)_changeVariantController.ChangeVariantAction.SelectedItem.Data;
                var items = _viewVariantsChoiceAction.Items;
                if (_currentVariantInfo.Caption == ChangeVariantController.DefaultVariantId) {
                    var choiceActionItems = items.Where(item => !ReferenceEquals(item.Data, Clone1)).ToList();
                    foreach (var choiceActionItem in choiceActionItems) {
                        choiceActionItem.Enabled[ChangeVariantController.DefaultVariantId] = false;
                    }
                }
                else {
                    foreach (var item in items) {
                        item.Enabled[ChangeVariantController.DefaultVariantId] = true;
                    }
                }
            }
        }

        public void CloneView(ViewCloner viewCloner) {
            var modelVariants = ((IModelViewVariants)_rootListView ).Variants;
            _changeVariantMainWindowController.GenerateDefaultVariant = modelVariants.Count == 0;
            var modelVariant = CreateVariantNode(viewCloner.Caption, modelVariants);
            _changeVariantController.RefreshVariantsAction();
            _changeVariantMainWindowController.GenerateDefaultVariant = true;
            _changeVariantMainWindowController.ChangeCurrentViewToVariant(Frame, modelVariant.View.Id);
        }

        protected override void OnViewControllersActivated() {
            base.OnViewControllersActivated();
            _isViewClonable = ((IModelListViewViewClonable) View.Model).IsViewClonable;
            _viewVariantsChoiceAction.Active[IsClonable] = _isViewClonable ;
            if (_isViewClonable) {
                _changeVariantMainWindowController = Frame.GetController<ChangeVariantMainWindowController>();
                _changeVariantController = Frame.GetController<ChangeVariantController>();
                _rootListView = FindRootListView();
                _changeVariantController.ChangeVariantAction.SelectedItemChanged += ChangeVariantActionOnSelectedItemChanged;
                _changeVariantMainWindowController.GenerateDefaultVariant = true;
                _changeVariantController.RefreshVariantsAction();
                _changeVariantMainWindowController.GenerateDefaultVariant = false;
                if (_changeVariantController.ChangeVariantAction.Items.Count == 0) {
                    foreach (var item in _viewVariantsChoiceAction.Items.Where(item => !ReferenceEquals(item.Data, Clone1))) {
                        item.Enabled[ChangeVariantController.DefaultVariantId] = false;
                    }
                }
            }
        }


        IModelListView FindRootListView() {
            EventHandler<CustomGetVariantsEventArgs>[] handlers = { null };
            IModelListView modelListView = null;
            handlers[0] = (sender, args) => {
                modelListView = (IModelListView) Application.Model.Views[args.ViewId];
                _changeVariantMainWindowController.CustomGetVariants -= handlers[0];
            };
            _changeVariantMainWindowController.CustomGetVariants += handlers[0];
            _changeVariantMainWindowController.GetVariants(View);
            Debug.Assert(modelListView != null, "modelView != null");
            return modelListView;
        }

        IModelVariant CreateVariantNode(string id, IModelVariants modelVariants) {
            var newVariantNode = modelVariants.AddNode<IModelVariant>();
            modelVariants.Current = newVariantNode;
            newVariantNode.Caption = id;
            newVariantNode.Id = id;
            newVariantNode.View = ((IModelListView)((ModelNode)View.Model).Clone(id));
            return newVariantNode;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewViewClonable>();
            extenders.Add<IModelClass, IModelClassViewClonable>();
        }

    }
}