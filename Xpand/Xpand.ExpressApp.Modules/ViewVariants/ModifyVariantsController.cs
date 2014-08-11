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
using DevExpress.Persistent.Validation;
using Xpand.Persistent.Base.General;
using Fasterflect;
using Xpand.Persistent.Base.Xpo.MetaData;

namespace Xpand.ExpressApp.ViewVariants {
    public interface IModelApplicationViewVariants:IModelApplication{
        IModelXpandViewVariants ViewVariants { get; }
    }

    public interface IModelXpandViewVariants:IModelNode{
        [Browsable(false)]
        string Storage { get; set; }
    }

    public interface IModelClassViewClonable {
        [Description("Determines if the clone action will be shown for the view")]
        [Category(XpandViewVariantsModule.ViewVariantsModelCategory)]
        bool IsViewClonable { get; set; }
        [Category(XpandViewVariantsModule.ViewVariantsModelCategory)]
        bool ClonedViewResetEnabled { get; set; }
        [Category(XpandViewVariantsModule.ViewVariantsModelCategory)]
        [DefaultValue(true)]
        bool DeleteViewOnRename { get; set; }
        [Category(XpandViewVariantsModule.ViewVariantsModelCategory)]
        [DefaultValue(true)]
        bool DeleteViewOnDelete { get; set; }
    }

    [ModelInterfaceImplementor(typeof(IModelClassViewClonable), "ModelClass")]
    public interface IModelListViewViewClonable : IModelClassViewClonable {
    }

    public class ModifyVariantsController : ViewController<ListView>, IModelExtender {
        public const string ClonedViewsWithReset = "ClonedViewsWithReset";
        public const string IsClonable = "IsViewClonable";
        public const string Modify = "Modify";
        public const string Clone1 = "Clone";
        public const string DefaultVariantId = "XpandDefaultVariantId";
        public const string Delete = "Delete";
        
        readonly SingleChoiceAction _modifyVariantsAction;
        ChangeVariantController _changeVariantController;
        IModelListView _rootListView;
        VariantInfo _currentVariantInfo;
        bool _isViewClonable;
        ViewVariantsModule _viewVariantsModule;

        public ModifyVariantsController() {
            _modifyVariantsAction = new SingleChoiceAction(this, "ModifyViewVariants", PredefinedCategory.View){Caption = "Modify Variants"};
            
            _modifyVariantsAction.Execute += ModifyVariantsActionOnExecute;
            _modifyVariantsAction.ShowItemsOnClick = true;
            _modifyVariantsAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            _modifyVariantsAction.Items.Add(new ChoiceActionItem(Clone1, Clone1));
            _modifyVariantsAction.Items.Add(new ChoiceActionItem(Modify, Modify));
            _modifyVariantsAction.Items.Add(new ChoiceActionItem(Delete, Delete));
            TargetViewNesting = Nesting.Any;
        }

        void ModifyVariantsActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e) {
            View.SaveModel();
            if (ReferenceEquals(e.SelectedChoiceActionItem.Data, Clone1)) {
                CreateViewVariant(e);
            } else if (ReferenceEquals(e.SelectedChoiceActionItem.Data, Modify)) {
                ModifyViewVariant(e);
            } else if (ReferenceEquals(e.SelectedChoiceActionItem.Data, Delete)){
                DeleteViewVariant(e);
            }
        }

        void DeleteViewVariant(SingleChoiceActionExecuteEventArgs e) {
            ShowViewVariantView(e, controller => DeleteViewVariantCore(((IModelListViewViewClonable)_rootListView).DeleteViewOnDelete),
                                detailView => {
                                    detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.ViewVariantsModelCategory, "DeleteView");
                                    var viewVariant = ((ViewVariant)detailView.CurrentObject);
                                    viewVariant.ShowCaption = false;
                                });
        }

        void ModifyViewVariant(SingleChoiceActionExecuteEventArgs e) {
            ShowViewVariantView(e, controller => ModifyViewVariantCore(controller.Frame),
                                detailView => {
                                    detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.ViewVariantsModelCategory, "ModiyView");
                                    var viewVariant = ((ViewVariant)detailView.CurrentObject);
                                    viewVariant.VariantCaption =_changeVariantController.ChangeVariantAction.SelectedItem.Caption;
                                });
        }

        void CreateViewVariant(SingleChoiceActionExecuteEventArgs e) {
            ShowViewVariantView(e, controller => CreateViewVariantCore(controller.Frame),
                                detailView => detailView.Caption =CaptionHelper.GetLocalizedText(XpandViewVariantsModule.ViewVariantsModelCategory,"CreateView"));
        }

        private IModelListView CloneView(string id, string viewCaption) {
            var modelApplicationBase = (ModelApplicationBase)Application.Model;
            var clonedViewResetEnabled = ((IModelListViewViewClonable)View.Model).ClonedViewResetEnabled;
            if (clonedViewResetEnabled) {
                var modelListView = (IModelListView)(((ModelNode)View.Model).Clone(id));
                modelListView.Caption = viewCaption;
                SynchronizeModel(Frame, id);
                var viewXml = modelListView.Xml();
                var clonedViewsStorage = ((IModelApplicationViewVariants)Application.Model).ViewVariants.Storage ?? viewXml;
                var clonedViewStorageLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
                clonedViewStorageLayer.Id = id;
                new ModelXmlReader().ReadFromString(clonedViewStorageLayer,"",clonedViewsStorage);
                new ModelXmlReader().ReadFromString(clonedViewStorageLayer, "", string.Format("<Application><Views>{0}</Views></Application>", viewXml));                    
                ((IModelApplicationViewVariants)Application.Model).ViewVariants.Storage = clonedViewStorageLayer.Xml;
                modelListView.Remove();
                modelApplicationBase.AddLayerBeforeLast(clonedViewStorageLayer);
                return (IModelListView) Application.Model.Views[id];
            }
            return (IModelListView)(((ModelNode)View.Model).Clone(id));
        }

        void DeleteViewVariantCore(bool deleteView) {
            var variantsInfo = _viewVariantsModule.FrameVariantsEngine.GetVariants(View);
            var rootModelVariant = ((IModelViewVariants)_rootListView).Variants[_currentVariantInfo.Id];
            rootModelVariant.Remove();
            if (deleteView)
                Application.Model.Views[_currentVariantInfo.ViewID].Remove();
            var variantInfos = variantsInfo.Items.Where(info => info.Id != _currentVariantInfo.Id).ToArray();
            VariantsInfo infos=null;
            if (variantInfos.Length>1)
                infos = new VariantsInfo(variantsInfo.RootViewId, DefaultVariantId, variantInfos);
            ChangeToVariant(infos);
            UpdateActionState();
        }

        void ShowViewVariantView(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs,Action<DialogController> dialogControllerAction,Action<DetailView> detailViewAction) {
            var objectSpace = Application.CreateObjectSpace(typeof (ViewVariant));
            DetailView detailView = Application.CreateDetailView(objectSpace, objectSpace.CreateObject<ViewVariant>());
            detailView.ViewEditMode = ViewEditMode.Edit;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = detailView;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            detailViewAction.Invoke(detailView);
            var dialogController = new DialogController();
            dialogController.Accepting += (o, args) => {
                var controller = ((DialogController) o);
                Validator.RuleSet.Validate(controller.Frame.View.ObjectSpace,controller.Frame.View.CurrentObject,ContextIdentifier.Save);
                dialogControllerAction.Invoke(controller);
            };
            singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }

        private void ModifyViewVariantCore(Frame frame) {
            var viewVariant = (ViewVariant)frame.View.CurrentObject;
            var modelVariants = ((IModelViewVariants)_rootListView).Variants;
            if (_currentVariantInfo.Id == DefaultVariantId) {
                modelVariants[DefaultVariantId].Caption = viewVariant.VariantCaption;
                ChangeToVariant();
            } else if (_currentVariantInfo.Id==null) {
                var modelVariant = modelVariants[DefaultVariantId];
                if (modelVariant==null)
                    CreateVariantNode(DefaultVariantId, modelVariants, _rootListView, viewVariant.VariantCaption);
                else {
                    modelVariant.Caption = viewVariant.VariantCaption;
                    ChangeToVariant();
                }
            }
            else {
                var modelVariant = modelVariants[_currentVariantInfo.Id];
                modelVariant.Caption = viewVariant.VariantCaption;
                SynchronizeModel(frame, modelVariant.View.Id);
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
                var items = _modifyVariantsAction.Items;
                if (_currentVariantInfo.Id == DefaultVariantId){
                    UpdateActionState();
                }
                else {
                    foreach (var item in items) {
                        item.Enabled[DefaultVariantId] = true;
                    }
                }
            }
        }

        private  void UpdateActionState(){
            var choiceActionItems = _modifyVariantsAction.Items.Where(item => new[] { Delete, Modify }.Any(s => ReferenceEquals(s, item.Data))).ToList();
            foreach (var choiceActionItem in choiceActionItems){
                choiceActionItem.Enabled[DefaultVariantId] = false;
            }
        }

        public void CreateViewVariantCore(Frame frame) {
            var viewVariant = (ViewVariant)frame.View.CurrentObject;
            var modelVariants = ((IModelViewVariants)_rootListView ).Variants;
            if (modelVariants.Count == 0) {
                CreateVariantNode(DefaultVariantId, modelVariants,_rootListView,_rootListView.Caption);
            }
            CreateVariantNode(viewVariant.VariantCaption, ((IViewVariant)viewVariant).ViewCaption, modelVariants);
            ChangeToVariant();
        }

        private void SynchronizeModel(Frame frame, string viewId){
            var viewVariant = (ViewVariant)frame.View.CurrentObject;
            var modelMemberInfoController = frame.GetController<XpandModelMemberInfoController>();
            modelMemberInfoController.SynchronizeModel(Application.Model.Views[viewId], viewVariant);
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
            _modifyVariantsAction.Active[IsClonable] = _isViewClonable ;
            if (_isViewClonable) {
                _changeVariantController = Frame.GetController<ChangeVariantController>();
                _rootListView = FindRootListView();
                _changeVariantController.ChangeVariantAction.SelectedItemChanged += ChangeVariantActionOnSelectedItemChanged;
                if (_changeVariantController.ChangeVariantAction.Items.Count == 0) {
                    UpdateActionState();
                }
            }
        }

        IModelListView FindRootListView() {
            if (_viewVariantsModule.FrameVariantsEngine != null){
                var variantsInfo = _viewVariantsModule.FrameVariantsEngine.GetVariants(View);
                return variantsInfo == null ? View.Model : (IModelListView) Application.Model.Views[variantsInfo.RootViewId];
            }
            return null;
        }

        void CreateVariantNode(string id, IModelVariants modelVariants, IModelListView modelListView, string caption){
            var newVariantNode = modelVariants.AddNode<IModelVariant>();
            modelVariants.Current = newVariantNode;
            newVariantNode.Caption = caption;
            newVariantNode.Id = id;
            newVariantNode.View = modelListView;
        }

        void CreateVariantNode(string variantCaption, string viewCaption, IModelVariants modelVariants) {
            var id = GetId(viewCaption);
            var modelView = ((IModelListView) Application.Model.Views[id]);
            var modelListView = modelView ?? CloneView(id, viewCaption);
            CreateVariantNode(variantCaption, modelVariants, modelListView, variantCaption);
        }

        private  string GetId(string caption){
            return View.Model.ModelClass.TypeInfo.Name +"_ListView_"+caption.Replace(" ","");
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewViewClonable>();
            extenders.Add<IModelClass, IModelClassViewClonable>();
            extenders.Add<IModelApplication, IModelApplicationViewVariants>();
        }
    }
}
