using System;
using System.ComponentModel;
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
using Xpand.Persistent.Base.General;
using Fasterflect;

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
        public const string IsClonable = "IsViewClonable";
        public const string Rename = "Rename";
        public const string Clone1 = "Clone";
        public const string DefaultVariantId = "XpandDefaultVariantId";
        public const string Delete = "Delete";
        
        readonly SingleChoiceAction _viewVariantsChoiceAction;
        ChangeVariantController _changeVariantController;
        IModelListView _rootListView;
        VariantInfo _currentVariantInfo;
        bool _isViewClonable;
        ViewVariantsModule _viewVariantsModule;

        public ModifyVariantsController() {
            _viewVariantsChoiceAction = new SingleChoiceAction(this, "ModifyViewVariants", PredefinedCategory.View){Caption = "View Variant"};
            _viewVariantsChoiceAction.Execute += ViewVariantsChoiceActionOnExecute;
            _viewVariantsChoiceAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            _viewVariantsChoiceAction.Items.Add(new ChoiceActionItem(Clone1, Clone1));
            _viewVariantsChoiceAction.Items.Add(new ChoiceActionItem(Rename, Rename));
            _viewVariantsChoiceAction.Items.Add(new ChoiceActionItem(Delete, Delete));
            TargetViewNesting = Nesting.All;
        }

        void ViewVariantsChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e) {
            View.SaveModel();
            if (ReferenceEquals(e.SelectedChoiceActionItem.Data, Clone1)) {
                CreateViewVariant(e);
            } else if (ReferenceEquals(e.SelectedChoiceActionItem.Data, Rename)) {
                RenameViewVariant(e);
            } else if (ReferenceEquals(e.SelectedChoiceActionItem.Data, Delete)) {
                DeleteViewVariant(e);
            }
        }

        void DeleteViewVariant(SingleChoiceActionExecuteEventArgs e) {
            ShowViewVariantView(e, controller => DeleteViewVariantCore((ViewVariant) (controller).Frame.View.CurrentObject),
                                detailView => {
                                    detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.XpandViewVariants, "DeleteView");
                                    var viewVariant = ((ViewVariant) detailView.CurrentObject);
                                    viewVariant.CanDeleteView = true;
                                    viewVariant.ShowCaption = false;
                                });
        }

        void RenameViewVariant(SingleChoiceActionExecuteEventArgs e) {
            ShowViewVariantView(e, controller => RenameViewVariantCore((ViewVariant) (controller).Frame.View.CurrentObject),
                                detailView => {
                                    detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.XpandViewVariants, "RenameView");
                                    ((ViewVariant)detailView.CurrentObject).Caption = GetCaption();
                                    ((ViewVariant)detailView.CurrentObject).CanDeleteView = _currentVariantInfo.Id != DefaultVariantId && _currentVariantInfo.Id!=null;
                                });
        }

        void CreateViewVariant(SingleChoiceActionExecuteEventArgs e) {
            ShowViewVariantView(e, controller => CreateViewVariantCore((ViewVariant) (controller).Frame.View.CurrentObject),
                                detailView => detailView.Caption =CaptionHelper.GetLocalizedText(XpandViewVariantsModule.XpandViewVariants,"CreateView"));
        }

        void DeleteViewVariantCore(ViewVariant viewVariant) {
            var variantsInfo = _viewVariantsModule.FrameVariantsEngine.GetVariants(View);
            var rootModelVariant = ((IModelViewVariants)_rootListView).Variants[_currentVariantInfo.Id];
            rootModelVariant.Remove();
            if (viewVariant.DeleteView)
                Application.Model.Views[_currentVariantInfo.ViewID].Remove();
            var variantInfos = variantsInfo.Items.Where(info => info.Id != _currentVariantInfo.Id).ToArray();
            VariantsInfo infos=null;
            if (variantInfos.Length>1)
                infos = new VariantsInfo(variantsInfo.RootViewId, DefaultVariantId, variantInfos);
            ChangeToVariant(infos);
        }


        void ShowViewVariantView(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs,Action<DialogController> dialogControllerAction,Action<DetailView> detailViewAction) {
            var objectSpace = Application.CreateObjectSpace(typeof (ViewVariant));
            DetailView detailView = Application.CreateDetailView(objectSpace, objectSpace.CreateObject<ViewVariant>());
            detailView.ViewEditMode = ViewEditMode.Edit;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = detailView;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            detailViewAction.Invoke(detailView);
            var dialogController = new DialogController();
            dialogController.Accepting +=(o, args) =>dialogControllerAction.Invoke((DialogController) o) ;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }

        string GetCaption() {
            var selectedItem = _changeVariantController.ChangeVariantAction.SelectedItem;
            if (selectedItem != null)
                return selectedItem.Caption;
            var modelVariant = ((IModelViewVariants) _rootListView).Variants[DefaultVariantId];
            return modelVariant != null ? modelVariant.Caption : _rootListView.Caption;
        }

        private void RenameViewVariantCore(ViewVariant viewVariant) {
            var modelVariants = ((IModelViewVariants)_rootListView).Variants;
            if (_currentVariantInfo.Id == DefaultVariantId) {
                modelVariants[DefaultVariantId].Caption = viewVariant.Caption;
                ChangeToVariant();
            } else if (_currentVariantInfo.Id==null) {
                var modelVariant = modelVariants[DefaultVariantId];
                if (modelVariant==null)
                    CreateVariantNode(DefaultVariantId, modelVariants, _rootListView, viewVariant.Caption);
                else {
                    modelVariant.Caption = viewVariant.Caption;
                    ChangeToVariant();
                }
            }
            else {
                var modelVariant = modelVariants[_currentVariantInfo.Id];
                modelVariant.Caption = viewVariant.Caption;
                if (viewVariant.DeleteView) {
                    DeleteViewVariantCore(viewVariant);
                    CreateViewVariantCore(viewVariant);
                } else {
                    ChangeToVariant();
                }
            }
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
                if (_currentVariantInfo.Id == DefaultVariantId) {
                    var choiceActionItems = items.Where(item => ReferenceEquals(item.Data, Delete)).ToList();
                    foreach (var choiceActionItem in choiceActionItems) {
                        choiceActionItem.Enabled[DefaultVariantId] = false;
                    }
                }
                else {
                    foreach (var item in items) {
                        item.Enabled[DefaultVariantId] = true;
                    }
                }
            }
        }

        public void CreateViewVariantCore(ViewVariant viewVariant) {
            var modelVariants = ((IModelViewVariants)_rootListView ).Variants;
            
            if (modelVariants.Count == 0) {
                CreateVariantNode(DefaultVariantId, modelVariants,_rootListView,_rootListView.Caption);
            }
            CreateVariantNode(viewVariant.Caption, modelVariants);
            ChangeToVariant();
        }

        void ChangeToVariant() {
            ChangeToVariant(_viewVariantsModule.FrameVariantsEngine.GetVariants(View));
        }

        void ChangeToVariant(VariantsInfo variantsInfo) {
            var currentFrameViewVariantsManager = _changeVariantController.CurrentFrameViewVariantsManager;
            currentFrameViewVariantsManager.SetFieldValue("viewVariants", variantsInfo);
            _changeVariantController.RefreshChangeVariantAction();
            if (variantsInfo!=null)
                _changeVariantController.CurrentFrameViewVariantsManager.ChangeToVariant(_currentVariantInfo);
        }

        protected override void OnViewControllersActivated() {
            base.OnViewControllersActivated();
            _viewVariantsModule = Application.FindModule<ViewVariantsModule>();
            _isViewClonable = ((IModelListViewViewClonable) View.Model).IsViewClonable;
            _viewVariantsChoiceAction.Active[IsClonable] = _isViewClonable ;
            if (_isViewClonable) {
                _changeVariantController = Frame.GetController<ChangeVariantController>();
                _rootListView = FindRootListView();
                _changeVariantController.ChangeVariantAction.SelectedItemChanged += ChangeVariantActionOnSelectedItemChanged;
                if (_changeVariantController.ChangeVariantAction.Items.Count == 0) {
                    foreach (var item in _viewVariantsChoiceAction.Items.Where(item => ReferenceEquals(item.Data, Delete))) {
                        item.Enabled[DefaultVariantId] = false;
                    }
                }
            }
        }

        IModelListView FindRootListView() {
            var variantsInfo = _viewVariantsModule.FrameVariantsEngine.GetVariants(View);
            return variantsInfo == null ? View.Model : (IModelListView) Application.Model.Views[variantsInfo.RootViewId];
        }

        void CreateVariantNode(string id, IModelVariants modelVariants, IModelListView modelListView, string caption) {
            var newVariantNode = modelVariants.AddNode<IModelVariant>();
            modelVariants.Current = newVariantNode;
            newVariantNode.Caption = caption;
            newVariantNode.Id = id;
            newVariantNode.View = modelListView;
        }

        void CreateVariantNode(string caption, IModelVariants modelVariants) {
            var modelView = ((IModelListView) Application.Model.Views[caption]);
            var modelListView = modelView??((IModelListView)((ModelNode)View.Model).Clone(caption));
            modelListView.Caption = caption;
            CreateVariantNode(caption, modelVariants, modelListView, caption);
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewViewClonable>();
            extenders.Add<IModelClass, IModelClassViewClonable>();
        }

    }
}
