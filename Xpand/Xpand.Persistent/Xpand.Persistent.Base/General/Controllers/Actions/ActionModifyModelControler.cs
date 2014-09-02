using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.Controllers.Actions {
    public interface IModifyModelActionUser {
         
    }
    [ModelAbstractClass]
    public interface IModelChoiceActionItemModifyModel : IModelChoiceActionItem {
        IModelModifyModelNodePaths ModelNodePaths { get; }
    }

    public class ModelActiosNodesUpdater : ModelNodesGeneratorUpdater<ModelActionsNodesGenerator> {
        public const string LockViewModel = "Lock View Model";
        public override void UpdateNode(ModelNode node){
            var modelAction = ((IModelActions) node)[ActionModifyModelControler.ModifyModelActionName];
            if (modelAction != null && modelAction.ChoiceActionItems != null){
                var lockViewModel = modelAction.ChoiceActionItems.AddNode<IModelChoiceActionItem>(LockViewModel);
                var modelNodePaths = ((IModelChoiceActionItemModifyModel) lockViewModel).ModelNodePaths;
                modelNodePaths.NewCaption = "Unlock Model";
                var allObjectViews = modelNodePaths.AddNode<IModelModifyModelNodePath>("All Object Views");
                var handleModelSaving = allObjectViews.Attributes.AddNode<IModelModelNodeAttribute>("HandleModelSaving");
                handleModelSaving.Value = "True";
            }
        }
    }

    public class ModifyModelNodePathsNodeGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
            
        }
    }

    [ModelNodesGenerator(typeof(ModifyModelNodePathsNodeGenerator))]
    public interface IModelModifyModelNodePaths : IModelList<IModelModifyModelNodePath>,IModelNode {
        [Required]
        string NewCaption { get; set; }
    }

    [DomainLogic(typeof(IModelModifyModelNodePaths))]
    public class ModelModifyModelNodePathsDomainLogic {
        public static string Get_NewCaption(IModelModifyModelNodePaths nodePaths){
            return nodePaths.GetParent<IModelChoiceActionItem>().Caption;
        }
    }

    public interface IModelModifyModelNodePath:IModelNode{
        IModelModeNodeAttributes Attributes { get; }
        [Required]
        [DefaultValue("Views")]
        [RefreshProperties(RefreshProperties.All)]
        string ModelNodePath { get; set; }

        [DefaultValue("(AsObjectView Is Not Null)")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + AssemblyInfo.VSuffix, typeof(UITypeEditor))]
        [CriteriaOptions("EvaluatorTypeInfo")]
        string Criteria { get; set; }
    }
    
    public interface IModelModeNodeAttributes : IModelList<IModelModelNodeAttribute>, IModelNode {

    }

    public interface IModelModelNodeAttribute : IModelNode {
        [Required]
        string Value { get; set; }
    }

    public class ActionModifyModelControler:WindowController,IModelExtender {
        public const string ModifyModelActionName = "ModifyModel";
        private List<SingleChoiceAction> _singleChoiceActions;
        private readonly SingleChoiceAction _modifyModelAction;

        public ActionModifyModelControler(){
            _modifyModelAction = new SingleChoiceAction(this,ModifyModelActionName,PredefinedCategory.View){
                ItemType = SingleChoiceActionItemType.ItemIsOperation,
                DefaultItemMode = DefaultItemMode.LastExecutedItem
            };
            _modifyModelAction.Execute+=ModifyModelActionOnExecute;
        }

        public SingleChoiceAction ModifyModelAction{
            get { return _modifyModelAction; }
        }

        private void ModifyModelActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){

        }

        protected override void OnActivated(){
            base.OnActivated();
            var modelChoiceActionItems = Application.Model.ActionDesign.Actions.Where(action 
                => action.ChoiceActionItems!=null&&action.ChoiceActionItems.Any()).SelectMany(action => action.ChoiceActionItems);
            var items = GetModelChoiceActionItemModifyModels(modelChoiceActionItems);
            _singleChoiceActions = Frame.Actions<SingleChoiceAction>(items).ToList();
            foreach (var singleChoiceAction in _singleChoiceActions){
                singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
            }    
        }

        private IEnumerable<IModelChoiceActionItemModifyModel> GetModelChoiceActionItemModifyModels(IEnumerable<IModelChoiceActionItem> modelChoiceActionItems){
            var actionItemModifyModels = modelChoiceActionItems.Cast<IModelChoiceActionItemModifyModel>();
            return actionItemModifyModels.Where(model => model.ModelNodePaths.Any());
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            foreach (var singleChoiceAction in _singleChoiceActions){
                singleChoiceAction.Execute-=SingleChoiceActionOnExecute;
            }
        }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            var modelBaseChoiceActionItem = e.SelectedChoiceActionItem.Model as IModelChoiceActionItemModifyModel;
            if (modelBaseChoiceActionItem != null){
                var modelNodePaths = modelBaseChoiceActionItem.ModelNodePaths;
                var modelNodeAttributes = modelNodePaths.SelectMany(path => path.Attributes);
                if (modelNodeAttributes.Any()) {
                    var oldCaption = e.SelectedChoiceActionItem.Caption;
                    e.SelectedChoiceActionItem.Caption = modelNodePaths.NewCaption;
                    var modelItem = modelBaseChoiceActionItem;
                    foreach (var modelNodePath in modelItem.ModelNodePaths) {
                        var modelNodes = GetModelNodes(modelNodePath);
                        foreach (var nodeAttribute in modelNodePath.Attributes) {
                            var propertyName = nodeAttribute.Id();
                            var oldValue = OldValue(propertyName, modelNodes, nodeAttribute.Value);
                            foreach (var modelNode in modelNodes){
                                modelNode.SetChangedValue(propertyName, nodeAttribute.Value);
                            }
                            nodeAttribute.Value = oldValue;    
                        }
                    }
                    modelNodePaths.NewCaption = oldCaption;
                }
            }
        }

        private string OldValue(string propertyName, IEnumerable<IModelNode> modelNodes, string value){
            var firstOrDefault = modelNodes.Select(node =>{
                var modelValueInfo = node.GetModelValueInfo(propertyName).Item1;
                var changedValue = modelValueInfo.ChangedValue(value, modelValueInfo.PropertyType);
                return node.GetValue(propertyName) != changedValue ? node : null;
            }).FirstOrDefault();
            return firstOrDefault != null ? firstOrDefault.GetValue(propertyName) + "" : null;
        }

        private IEnumerable<IModelNode> GetModelNodes(IModelModifyModelNodePath modelNodePath) {
            var modelNodes = ((IEnumerable)Application.Model.FindNodeByPath(modelNodePath.ModelNodePath)).Cast<IModelNode>();
            return modelNodes.GetNodes(modelNodePath.Criteria);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelChoiceActionItem, IModelChoiceActionItemModifyModel>();
        }
    }
}
