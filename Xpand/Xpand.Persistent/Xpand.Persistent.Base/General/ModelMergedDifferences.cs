using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Fasterflect;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.Persistent.Base.General {
    public class MergedDifferencesUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            var modulesDifferences = ModuleDifferencesHelper.GetModuleDifferences(node);
            var modelViews = ((IModelViews)node);
            var newViews = AddNewViews(modelViews, modulesDifferences).ToList();
            var mergedDifferenceInfos = MergeDifferencesHelper.GetMergedDifferenceInfos(modelViews, modulesDifferences );
            CloneMergedView(mergedDifferenceInfos, modelViews, modulesDifferences);
            foreach (var newView in newViews) {
                newView.Remove();
            }
            AddDifferenceLayers(node, mergedDifferenceInfos, modulesDifferences);
        }

        IEnumerable<IModelObjectView> AddNewViews(IModelViews modelViews, IEnumerable<ModelApplicationBase> modulesDifferences) {
            var objectViews = modulesDifferences.Cast<IModelApplication>().SelectMany(application => 
                application.Views.OfType<IModelObjectView>()).Where(view => view.IsNewNode());
            foreach (IModelObjectView objectView in objectViews.Where(view => modelViews[view.Id]==null)) {
                ModelEditorHelper.AddCloneNode((ModelNode)modelViews, (ModelNode)objectView, objectView.Id);
                yield return modelViews[objectView.Id].AsObjectView;
            }
        }

        void AddDifferenceLayers(ModelNode node, IEnumerable<ModelMergedDifferenceInfo> mergedDifferenceses, List<ModelApplicationBase> modulesDifferences) {
            var master = (ModelApplicationBase) ((ModelApplicationBase) node.Application).Master;
            foreach (var mergedDifferenceInfo in mergedDifferenceses) {
                foreach (ModelApplicationBase modulesDifference in GetModulesDifferences(modulesDifferences, mergedDifferenceInfo.MergedViewId)) {
                    var modelApplicationBase = CreateInheritanceLayer(node, modulesDifference, mergedDifferenceInfo,master);
                    master.InsertLayer(master.LayersCount - 1, modelApplicationBase);
                }
            }
            master.CallMethod("EnsureNodes");
        }

        void CloneMergedView(IEnumerable<ModelMergedDifferenceInfo> mergedDifferenceses, IModelViews modelViews, List<ModelApplicationBase> modulesDifferences) {
            var modelObjectViews = mergedDifferenceses.GroupBy(info =>((IModelObjectView) info.TargetView));
            foreach (var modelObjectView in modelObjectViews) {
                var modelView = modelViews[modelObjectView.Key.Id];
                if (modelView != null) {
                    var targetObjectView = modelView.AsObjectView;
                    foreach (var modelMergedDifferenceInfo in modelObjectView) {
                        var sourceObjectView = modelViews[modelMergedDifferenceInfo.MergedViewId].AsObjectView;
                        var strategiesModel = ModuleDifferencesHelper.StrategiesModel((ModelNode)modelViews, modulesDifferences);
                        var optionsMergedDifferenceStrategy = ((IModelOptionsMergedDifferenceStrategy)((IModelApplication)strategiesModel).Options);
                        var modelMergedDifference = modelMergedDifferenceInfo.ModelMergedDifference;
                        var id = modelMergedDifference.Strategy.Id();
                        var differenceStrategy = optionsMergedDifferenceStrategy.MergedDifferenceStrategies[id];
                        MergeObjectViewValueInfos(differenceStrategy, sourceObjectView, targetObjectView);
                        CloneNodes(modelMergedDifference, targetObjectView, sourceObjectView);
                    }
                }
            }
        }

        void CloneNodes(IModelMergedDifference mergedDifference, IModelObjectView targetObjectView,IModelObjectView sourceObjectView) {
            foreach (var nodePath in GetIncludedNodePaths(mergedDifference)) {
                CalculatedModelNodeList<IModelNode> items = null;
                var needsUpdate = nodePath.NodePath == "Items" || nodePath.NodePath == "Columns";
                if (needsUpdate) {
                    var modelDetailView = targetObjectView as IModelDetailView;
                    var modelNodes = modelDetailView != null? (IEnumerable<IModelNode>) modelDetailView.Items: ((IModelListView) targetObjectView).Columns;
                    items = new CalculatedModelNodeList<IModelNode>(modelNodes);
                }
                RemoveUnrelatedNodes(targetObjectView, nodePath);
                CloneNodes(sourceObjectView, nodePath, targetObjectView);
                if (needsUpdate) {
                    UpdateDetailViewItems(items, targetObjectView);
                    UpdateColumns(items, targetObjectView, mergedDifference.InvisibleListViewColumns);
                }
            }
        }

        IEnumerable<IModelApplication> GetModulesDifferences(IEnumerable<ModelApplicationBase> modulesDifferences, string viewId) {
            return modulesDifferences.Cast<IModelApplication>().Where(application => application.Views[viewId] != null);
        }

        IEnumerable<IModelMergedDifferenceStrategyIncludedNodePath> GetIncludedNodePaths(IModelObjectView modelObjectView, IModelMergedDifferenceStrategy strategy) {
            var viewType = modelObjectView is IModelDetailView ? ViewType.DetailView : ViewType.ListView;
            return strategy.IncludedNodePaths.Where(path => path.ViewType == viewType || path.ViewType == ViewType.Any);
        }

        IEnumerable<IModelMergedDifferenceStrategyIncludedNodePath> GetIncludedNodePaths(IModelMergedDifference mergedDifference) {
            return GetIncludedNodePaths((IModelObjectView) mergedDifference.Parent.Parent,mergedDifference.Strategy);
        }

        void MergeObjectViewValueInfos(IModelMergedDifferenceStrategy differenceStrategy, IModelView sourceObjectView, IModelObjectView targetObjectView) {
            foreach (var valueInfo in GetValuesInfos(sourceObjectView,differenceStrategy.MergedViewValueInfos)) {
                var sourceValue = sourceObjectView.GetValue(valueInfo.Name, valueInfo.PropertyType);
                var targetValue = targetObjectView.GetValue(valueInfo.Name, valueInfo.PropertyType);
                if (sourceValue!=targetValue)
                    targetObjectView.SetValue(valueInfo.Name, valueInfo.PropertyType, sourceValue);
            }
        }

        IEnumerable<ModelValueInfo> GetValuesInfos(IModelView sourceObjectView, IModelMergedViewValueInfos mergedViewValueInfos) {
            var modelViewValueInfos = mergedViewValueInfos.ListViewValueInfos;
            if (sourceObjectView is IModelDetailView) {
                modelViewValueInfos = mergedViewValueInfos.DetailViewValueInfos;
            }
            return modelViewValueInfos.Where(info => info.NodeEnabled).Select(info => info.ModelValueInfo);
        }

        void CloneNodes(IModelView modelView, IModelMergedDifferenceStrategyIncludedNodePath nodePath,IModelObjectView modelObjectView) {
            for (int i = 0; i < modelView.NodeCount; i++) {
                var modelNode = modelView.GetNode(i);
                var id = modelNode.Id();
                if (id == nodePath.NodePath) {
                    ModelEditorHelper.AddCloneNode((ModelNode) modelObjectView, (ModelNode) modelNode, id);
                    break;
                }
            }
        }

        void RemoveUnrelatedNodes(IModelObjectView modelObjectView,IModelMergedDifferenceStrategyIncludedNodePath nodePath) {
            for (int i = modelObjectView.NodeCount - 1; i >= 0; i--) {
                var modelNode = modelObjectView.GetNode(i);
                if (modelNode.Id() == nodePath.NodePath) {
                    modelNode.Remove();
                    break;
                }
            }
        }

        void UpdateDetailViewItems(CalculatedModelNodeList<IModelNode> items, IModelObjectView targetObjectView) {
            var detailView = targetObjectView as IModelDetailView;
            if (detailView!=null) {
                for (int index = items.Count - 1; index >= 0; index--) {
                    var modelViewItem = items[index];
                    var id = modelViewItem.Id();
                    var viewItem = detailView.Items[id];
                    if (viewItem == null) {
                        ModelEditorHelper.AddCloneNode((ModelNode) detailView.Items, (ModelNode) modelViewItem, id);
                    }
                }
            }
        }

        void UpdateColumns(CalculatedModelNodeList<IModelNode> columns, IModelObjectView targetObjectView, bool invisibleListViewColumns) {
            var modelListView = targetObjectView as IModelListView;
            if (modelListView!=null) {
                for (int index = columns.Count - 1; index >= 0; index--) {
                    var column = columns[index];
                    var id = column.Id();
                    var viewItem = modelListView.Columns[id];
                    if (viewItem == null) {
                        var source = (ModelNode) column;
                        var cloneNode = ModelEditorHelper.AddCloneNode((ModelNode) modelListView.Columns, source, id);
                        if (invisibleListViewColumns)
                            cloneNode.Index = -1;
                    }
                }
            }
        }

        ModelApplicationBase CreateInheritanceLayer(ModelNode node, ModelApplicationBase applicationBase, ModelMergedDifferenceInfo mergedDifference, ModelApplicationBase master) {
            var modelApplication = node.CreatorInstance.CreateModelApplication();
            var targetViewId= ((ModelNode) mergedDifference.TargetView).Id;
            modelApplication.Id = string.Format("{0}_" + applicationBase.Id, targetViewId);
            new ModelXmlReader().ReadFromModel(modelApplication, applicationBase);
            var application = ((IModelApplication) modelApplication);
            RemoveUnrelatedNodes(mergedDifference, application);
            application.Views[mergedDifference.MergedViewId].Id=targetViewId;


            var modelSources = ((IModelSources) application);
            modelSources.BOModelTypes = ((IModelSources) master).BOModelTypes;
            modelSources.Modules = ((IModelSources)master).Modules;
            modelSources.Controllers = ((IModelSources)master).Controllers;
            modelSources.Localizers = ((IModelSources)master).Localizers;
            modelSources.EditorDescriptors = new EditorDescriptors(((IModelSources)master).EditorDescriptors);
            return modelApplication;
        }

        void RemoveUnrelatedNodes(ModelMergedDifferenceInfo mergedDifference, IModelApplication application) {
            var mergedDifferenceViewId = mergedDifference.MergedViewId;
            var strategy = mergedDifference.ModelMergedDifference.Strategy;
            var modelObjectView = application.Views[mergedDifferenceViewId].AsObjectView;
            RemoveUnrelatedNodesExceptViews(application);
            RemoveUnrelatedViewExceptDifferenceView(mergedDifferenceViewId, application.Views);
            RemoveStrategyUnrelatedNodes(modelObjectView, strategy);
            ClearViewAttributes(strategy, modelObjectView);
        }

        void ClearViewAttributes(IModelMergedDifferenceStrategy strategy, IModelObjectView modelObjectView) {
            foreach (var valueInfo in GetValuesInfos(modelObjectView, strategy.MergedViewValueInfos)) {
                modelObjectView.ClearValue(valueInfo.Name);
            }
        }

        void RemoveStrategyUnrelatedNodes(IModelObjectView modelObjectView,IModelMergedDifferenceStrategy strategy) {
            var nodePaths = new HashSet<string>(GetIncludedNodePaths(modelObjectView, strategy).Select(path => path.NodePath));
            for (int i = modelObjectView.NodeCount - 1; i >= 0; i--) {
                var modelNode = modelObjectView.GetNode(i);
                var id = modelNode.Id();
                if (!nodePaths.Contains(id)) {
                    modelNode.Remove();
                }
            }
        }

        void RemoveUnrelatedViewExceptDifferenceView(string mergedDifferenceViewId, IModelViews modelViews) {
            for (int index = modelViews.Count - 1; index >= 0; index--) {
                var view = modelViews[index];
                if (view.Id != mergedDifferenceViewId)
                    view.Remove();
            }
        }

        void RemoveUnrelatedNodesExceptViews(IModelApplication application) {
            for (int i = application.NodeCount - 1; i >= 0; i--) {
                var modelNode = application.GetNode(i);
                if (((ModelNode) modelNode).Id != "Views")
                    modelNode.Remove();
            }
        }
    }
    
    static class ModuleDifferencesHelper {

        public static List<ModelApplicationBase> GetModuleDifferences(ModelNode node) {
            var modelSources = ((IModelSources) node.Application);
            var modelApplicationBases = new List<ModelApplicationBase>();
            foreach (var moduleBase in modelSources.Modules) {
                ModelApplicationCreator modelApplicationCreator = node.CreatorInstance;
                var modelApplicationBase = modelApplicationCreator.CreateModelApplication();
                modelApplicationBase.Id = moduleBase.Name;
                InitializeModelSources(modelApplicationBase, node);
                var resourcesModelStore = new ResourcesModelStore(moduleBase.GetType().Assembly);
                resourcesModelStore.Load(modelApplicationBase);
                var modelViews = ((IModelApplication)modelApplicationBase).Views;
                if (modelViews != null) {
                    modelApplicationBases.Add(modelApplicationBase);
                }
            }

            ReadFromOtherLayers(modelApplicationBases, node);
            return modelApplicationBases;
        }

        static void InitializeModelSources(ModelApplicationBase modelApplicationBase, ModelNode node) {
            var sources = ((IModelSources) node.Application);
            var targetSources = ((IModelSources) modelApplicationBase.Application);
            targetSources.BOModelTypes = sources.BOModelTypes;
            targetSources.Modules = sources.Modules;
            targetSources.Controllers = sources.Controllers;
            targetSources.Localizers = sources.Localizers;
            targetSources.EditorDescriptors = new EditorDescriptors(sources.EditorDescriptors);
        }

        static void ReadViewsFromOtherLayers(IEnumerable<ModelApplicationBase> modelApplicationBases, IModelMergedDifference modelMergedDifference, ModelApplicationBase modelApplicationBase) {
            var viewId = GetViewId(modelMergedDifference, modelApplicationBases);
            var mergedView = (IModelObjectView)modelApplicationBases.Cast<IModelApplication>().Select(application
                                                                                                      => application.Views[viewId]).First(view => view != null);
            var xml = "<Application><Views>" + ((ModelNode)mergedView).Xml + "</Views></Application>";
            new ModelXmlReader().ReadFromString(modelApplicationBase, "", xml);
        }

        public static string GetViewId(this IModelMergedDifference modelMergedDifference, IEnumerable<ModelApplicationBase> modelApplicationBases) {
            var viewId = GetViewIdCore(modelMergedDifference);
            if (string.IsNullOrEmpty(viewId)) {
                var id = modelMergedDifference.Parent.Parent.Id();
                var mergeDifferences = modelApplicationBases.Cast<IModelApplication>().Select(application =>
                    application.Views[id]).OfType<IModelObjectViewMergedDifferences>().Where(view =>
                        view != null && view.MergedDifferences != null).SelectMany(differences => differences.MergedDifferences);
                viewId = mergeDifferences.Select(GetViewIdCore).First(value => !string.IsNullOrEmpty(value));
            }
            return viewId;
        }

        static string GetViewIdCore(IModelMergedDifference modelMergedDifference) {
            return Regex.Match(((ModelNode)modelMergedDifference).Xml, "View=\"([^\"]*)",
                               RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value;
        }

        static ModelApplicationBase _strategiesModel;
        public static ModelApplicationBase StrategiesModel(ModelNode node, IEnumerable<ModelApplicationBase> modelApplicationBases) {
            if (_strategiesModel==null) {
                var strategies = ((IModelOptionsMergedDifferenceStrategy)node.Application.Options).MergedDifferenceStrategies;
                var xml = string.Format("<Application><Options>{0}</Options></Application>", ((ModelNode)strategies).Xml);
                var modelApplicationBase = node.CreatorInstance.CreateModelApplication();
                new ModelXmlReader().ReadFromString(modelApplicationBase, "", xml);
                ReadFromOtherLayers(modelApplicationBases, strategies, modelApplicationBase);
                UpdateRemovedNodes(modelApplicationBase);
                _strategiesModel=modelApplicationBase;
            }
            return _strategiesModel;
        }

        static void ReadFromOtherLayers(IEnumerable<ModelApplicationBase> modelApplicationBases, IModelMergedDifferenceStrategies strategies,
                                        ModelApplicationBase modelApplicationBase) {
            var xmls = modelApplicationBases.Cast<IModelApplication>().Select(application =>
                application.Options).Cast<IModelOptionsMergedDifferenceStrategy>().Where(strategy =>
                    strategy != null).Select(strategy => strategies.Xml()).Where(s => s != null);
            foreach (var x in xmls) {
                string xml = string.Format("<Application><Options>{0}</Options></Application>", x);
                new ModelXmlReader().ReadFromString(modelApplicationBase, "", xml);
            }
        }

        static void UpdateRemovedNodes(IModelNode modelNode) {
            for (int i = modelNode.NodeCount - 1; i >= 0; i--) {
                var node = modelNode.GetNode(i);
                var modelNode1 = ((ModelNode)node);
                if (CanBeRemoved(modelNode1))
                    node.Remove();
                UpdateRemovedNodes(node);
            }
        }

        static bool CanBeRemoved(ModelNode modelNode1) {
            if (modelNode1.IsRemovedNode) {
                for (int i = modelNode1.NodeCount - 1; i >= 0; i--) {
                    if (!CanBeRemoved(modelNode1.GetNode(i)))
                        return false;
                }
                return true;
            }
            return modelNode1.IsRemovedNode && !modelNode1.IsNewNode;
        }

        static void ReadFromOtherLayers(IEnumerable<ModelApplicationBase> modelApplicationBases, ModelNode node) {
            var strategiesModel = StrategiesModel(node, modelApplicationBases);
            foreach (var modelApplicationBase in modelApplicationBases) {
                var modelMergedDifferences = ModelMergedDifferences(modelApplicationBase);
                foreach (var modelMergedDifference in modelMergedDifferences) {
                    if (modelMergedDifference.View == null) {
                        ReadViewsFromOtherLayers(modelApplicationBases, modelMergedDifference, modelApplicationBase);
                    }
                    if (!modelMergedDifference.HasValue("Strategy")) {
                        new ModelXmlReader().ReadFromModel(modelApplicationBase, strategiesModel);
                    }
                }
            }
        }

        static IEnumerable<IModelMergedDifference> ModelMergedDifferences(ModelApplicationBase modelApplicationBase) {
            var modelViews = ((IModelApplication)modelApplicationBase).Views.OfType<IModelObjectViewMergedDifferences>();
            return modelViews.Where(differences=> differences.MergedDifferences != null && differences.MergedDifferences.Any())
                                                   .SelectMany(differences => differences.MergedDifferences);
        }

    }

    class MergeDifferencesHelper {
        static bool IsDifferenceValid(ModelMergedDifferenceInfo difference, IModelViews views) {
            var modelView = views[difference.MergedViewId] as IModelObjectView;
            return modelView != null && modelView.ModelClass != null;
        }

        static IEnumerable<ModelMergedDifferenceInfo> GetModelObjectViewMergedDifferenceses(IModelViews modelViews, IModelApplication modelApplication, IEnumerable<ModelApplicationBase> modulesDifferences) {
            var modelMergedDifferences = modelApplication.Views.OfType<IModelObjectViewMergedDifferences>().Where(differences 
                => differences.MergedDifferences != null).SelectMany(differences => differences.MergedDifferences);
            return modelMergedDifferences.Select(MergedDifferenceInfo(modulesDifferences)).Where(differences =>
                IsDifferenceValid(differences, modelViews));
        }

        public static IEnumerable<ModelMergedDifferenceInfo> GetMergedDifferenceInfos(IModelViews modelViews, IEnumerable<ModelApplicationBase> modulesDifferences) {
            var modelMergedDifferenceInfos = modulesDifferences.Cast<IModelApplication>().SelectMany(modelApplication 
                => GetModelObjectViewMergedDifferenceses(modelViews, modelApplication, modulesDifferences));
            return modelMergedDifferenceInfos.GroupBy(info =>
                    new { Id = info.ModelMergedDifference.Id() ,info.TargetView}).Select(infos => infos.Last());
        }

        static Func<IModelMergedDifference, ModelMergedDifferenceInfo> MergedDifferenceInfo(IEnumerable<ModelApplicationBase> modulesDifferences) {
            return difference => new ModelMergedDifferenceInfo(difference, difference.GetViewId(modulesDifferences));
        }
    }

    class ModelMergedDifferenceInfo {
        readonly IModelMergedDifference _modelMergedDifference;
        readonly string _mergedViewId;
        readonly IModelNode _targetView;

        public ModelMergedDifferenceInfo(IModelMergedDifference modelMergedDifference, string mergedViewId) {
            _modelMergedDifference = modelMergedDifference;
            _mergedViewId = mergedViewId;
            _targetView = modelMergedDifference.Parent.Parent;
        }

        public IModelNode TargetView {
            get { return _targetView; }
        }

        public IModelMergedDifference ModelMergedDifference {
            get { return _modelMergedDifference; }
        }

        public override string ToString() {
            return _modelMergedDifference != null
                       ? String.Format("{0}-{1}-{2}", _targetView.Id(), _modelMergedDifference.Id(), MergedViewId)
                       : base.ToString();
        }

        public string MergedViewId {
            get { return _mergedViewId; }
        }
    }
    [ModelAbstractClass]
    public interface IModelObjectViewMergedDifferences : IModelView {
        IModelMergedDifferences MergedDifferences { get; }
    }

    [ModelNodesGenerator(typeof(MergedDifferencesGenerator))]
    public interface IModelMergedDifferences : IModelNode, IModelList<IModelMergedDifference> {

    }

    public class MergedDifferencesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
            
        }
    }

    public interface IModelOptionsMergedDifferenceStrategy {
        IModelMergedDifferenceStrategies MergedDifferenceStrategies { get; }
    }

    [ModelNodesGenerator(typeof(MergedDifferencesStrategiesGenerator))]
    public interface IModelMergedDifferenceStrategies:IModelList<IModelMergedDifferenceStrategy>,IModelNode {
    }

    public class MergedDifferencesStrategiesGenerator : ModelNodesGeneratorBase {
        public const string Everything = "Everything";
        public const string EverythingButLayout = "EverythingButLayout";
        public const string OnlyLayout = "OnlyLayout";
        protected override void GenerateNodesCore(ModelNode node) {
            var modelMergedDifferenceStrategies = ((IModelMergedDifferenceStrategies) node);
            modelMergedDifferenceStrategies.AddNode<IModelMergedDifferenceStrategy>(Everything);
            modelMergedDifferenceStrategies.AddNode<IModelMergedDifferenceStrategy>(OnlyLayout);
            modelMergedDifferenceStrategies.AddNode<IModelMergedDifferenceStrategy>(EverythingButLayout);
        }
    }

    public interface IModelMergedDifferenceStrategy:IModelNode {
        IModelMergedDifferenceStrategyIncludedNodePaths IncludedNodePaths { get; }
        [Browsable(false)]
        IModelMergedDifferenceStrategyExcludedNodePaths ExcludedNodePaths { get; }
        IModelMergedViewValueInfos MergedViewValueInfos { get; }
    }

    [ModelNodesGenerator(typeof(MergedDifferenceStrategyIncludedNodePathsGenerator))]
    public interface IModelMergedDifferenceStrategyIncludedNodePaths : IModelNode, IModelList<IModelMergedDifferenceStrategyIncludedNodePath> {
//TODO:        Ilya+defaultvalue+check old masterdetail
    }

    [ModelNodesGenerator(typeof(MergedDifferenceStrategyExcludedNodePathsGenerator))]
    public interface IModelMergedDifferenceStrategyExcludedNodePaths : IModelNode, IModelList<IModelMergedDifferenceStrategyExcludedNodePath> {
         
    }

    public class MergedDifferenceStrategyIncludedNodePathsGenerator : ModelNodesGeneratorBase {
        public static readonly HashSet<string> LayoutNodeNames;

        static MergedDifferenceStrategyIncludedNodePathsGenerator() {
            LayoutNodeNames = new HashSet<string>{"Layout", "Items", "Columns", "SplitLayout"};
        }

        protected override void GenerateNodesCore(ModelNode node) {
            var modelMergedDifferenceStrategyNodePaths = ((IModelMergedDifferenceStrategyIncludedNodePaths) node);
            var modelMergedDifferenceStrategies = ((IModelMergedDifferenceStrategy) modelMergedDifferenceStrategyNodePaths.Parent);
            var id = modelMergedDifferenceStrategies.Id();

            GenerateAll(node, modelMergedDifferenceStrategyNodePaths);
            Func<HashSet<string>, string, bool> func = null;
            switch (id) {
                case MergedDifferencesStrategiesGenerator.OnlyLayout:
                    func = (set, s) => !set.Contains(s);
                    break;
                case MergedDifferencesStrategiesGenerator.EverythingButLayout:
                    func = (set, s) => set.Contains(s);
                    break;
            }
            if (func!=null)
                RemoveNodes(modelMergedDifferenceStrategyNodePaths, func);
            var mergedDifferencesPath = modelMergedDifferenceStrategyNodePaths.FirstOrDefault(path => path.Id() == "MergedDifferences");
            if (mergedDifferencesPath!=null)
                mergedDifferencesPath.Remove();
        }

        void RemoveNodes(IModelMergedDifferenceStrategyIncludedNodePaths nodePaths,Func<HashSet<string>,string,bool> func) {
            for (int i = nodePaths.NodeCount - 1; i >= 0; i--) {
                var modelNode = nodePaths.GetNode(i);
                var value = modelNode.Id();
                if (func.Invoke(LayoutNodeNames, value))
                    modelNode.Remove();
            }
        }

        void GenerateAll(ModelNode node, IModelMergedDifferenceStrategyIncludedNodePaths modelMergedDifferenceStrategyNodePaths) {
            var modelApplicationCreator = ((ModelNode) node.Application.Views).CreatorInstance;
            var modelNodeInfo = modelApplicationCreator.GetNodeInfo(typeof (IModelDetailView));
            GenerateNodePath(modelNodeInfo, modelMergedDifferenceStrategyNodePaths, ViewType.DetailView);
            modelNodeInfo = modelApplicationCreator.GetNodeInfo(typeof (IModelListView));
            GenerateNodePath(modelNodeInfo, modelMergedDifferenceStrategyNodePaths, ViewType.ListView);
        }

        void GenerateNodePath(ModelNodeInfo modelNodeInfo,IModelMergedDifferenceStrategyIncludedNodePaths modelMergedDifferenceStrategyNodePaths,ViewType viewType) {
            var names = ((IEnumerable<object>) modelNodeInfo.GetPropertyValue("ChildrenInfo")).Select(o => o.GetPropertyValue("Name")).Cast<string>();
            foreach (var name in names) {
                var path = modelMergedDifferenceStrategyNodePaths[name];
                if (path==null) {
                    var nodePath =modelMergedDifferenceStrategyNodePaths.AddNode<IModelMergedDifferenceStrategyIncludedNodePath>(name);
                    nodePath.NodePath = name;
                    nodePath.ViewType = viewType;
                }
                else {
                    path.ViewType=ViewType.Any;
                }                
            }
        }
    }

    public class MergedDifferenceStrategyExcludedNodePathsGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
        }
    }

    [ModelAbstractClass]
    public interface IModelMergedDifferenceStrategyNodePath:IModelNode {
        [Required]
        string NodePath { get; set; }
        [RuleValueComparison(DefaultContexts.Save, ValueComparisonType.NotEquals, ViewType.DashboardView)]
        ViewType ViewType { get; set; }
    }

    public interface IModelMergedViewValueInfos : IModelNode {
        IModelViewValueInfos ListViewValueInfos { get; }
        IModelViewValueInfos DetailViewValueInfos { get; }
    }

    [KeyProperty("Name")]
    public interface IModelMergedViewValueInfo:IModelNodeEnabled {
        [Required]
        [DataSourceProperty("Names")]
        string Name { get; set; }
        [Browsable(false)]
        IEnumerable<string> Names { get; }
        [Browsable(false)]
        ModelValueInfo ModelValueInfo { get; }
    }

    [DomainLogic(typeof(IModelMergedViewValueInfo))]
    public class IModelMergedViewValueInfoLogic {
        public static ModelValueInfo Get_ModelValueInfo(IModelMergedViewValueInfo mergedViewValueInfo) {
            return string.IsNullOrEmpty(mergedViewValueInfo.Name)? null
                       : ModelMergedViewValueInfosNodeGenerator.GetModelValueInfos(mergedViewValueInfo, info => true)
                                                               .First(info => info.Name == mergedViewValueInfo.Name);
        }

        public static IEnumerable<string> Get_Names(IModelMergedViewValueInfo mergedViewValueInfo) {
            var modelViewValueInfos = (IModelViewValueInfos) mergedViewValueInfo.Parent;
            var modelValueInfos = ModelMergedViewValueInfosNodeGenerator.GetModelValueInfos(modelViewValueInfos, info => true);
            return modelValueInfos.Select(info => info.Name).Where(s => modelViewValueInfos[s]==null);
        }

        public static bool Get_NodeEnabled(IModelMergedViewValueInfo mergedViewValueInfo) {
            return ((IModelNodeEnabled) mergedViewValueInfo.Parent).NodeEnabled;
        }
    }

    [ModelNodesGenerator(typeof(ModelMergedViewValueInfosNodeGenerator))]
    public interface IModelViewValueInfos : IModelList<IModelMergedViewValueInfo>, IModelNodeEnabled {
        
    }

    [DomainLogic(typeof(IModelViewValueInfos))]
    public class IModelViewValueInfosLogic {
        public static bool Get_NodeEnabled(IModelViewValueInfos viewValueInfos) {
            return false;
        }        
    }

    public  class ModelMergedViewValueInfosNodeGenerator : ModelNodesGeneratorBase  {
        static readonly HashSet<string> _invalidValueInfoNames = new HashSet<string> { "AsObjectView",  "Id", "Index", "IsNewNode",  "Removed" };

        static ModelMergedViewValueInfosNodeGenerator() {
            foreach (var excludedNodeMember in ModelAdapter.ModelSynchronizer<object, IModelNode>.ExcludedNodeMembers) {
                InvalidValueInfoNames.Add(excludedNodeMember);
            }
        }

        public static HashSet<string> InvalidValueInfoNames {
            get { return _invalidValueInfoNames; }
        }
        protected override void GenerateNodesCore(ModelNode node) {
            var modelViewValueInfos = ((IModelViewValueInfos) node);
            var modelValueInfos = GetModelValueInfos(modelViewValueInfos,IsValid);
            foreach (var valueInfo in modelValueInfos) {
                modelViewValueInfos.AddNode<IModelMergedViewValueInfo>(valueInfo.Name);
            }
        }

        public static IEnumerable<ModelValueInfo> GetModelValueInfos(IModelMergedViewValueInfo mergedViewValueInfo, Func<ModelValueInfo, bool> isValid) {
            var modelViewValueInfos = (IModelViewValueInfos)mergedViewValueInfo.Parent;
            return GetModelValueInfos(modelViewValueInfos, isValid);
        }

        public static IEnumerable<ModelValueInfo> GetModelValueInfos(IModelViewValueInfos modelViewValueInfos, Func<ModelValueInfo, bool> isValid) {
            var nodeType = GetNodeType(modelViewValueInfos);
            var modelNodeInfo = ((ModelNode) modelViewValueInfos).CreatorInstance.GetNodeInfo(nodeType);
            return modelNodeInfo.ValuesInfo.Where(info => !info.IsReadOnly).Where(isValid);
        }

        static Type GetNodeType(IModelViewValueInfos node) {
            return node.Id().StartsWith("ListView") ? typeof (IModelListView) : typeof (IModelDetailView);
        }

        static bool IsValid(ModelValueInfo modelValueInfo) {
            return !_invalidValueInfoNames.Contains(modelValueInfo.Name) &&
                   !typeof (IModelNode).IsAssignableFrom(modelValueInfo.PropertyType) && 
                   modelValueInfo.PropertyType != typeof (Type);
        }
    }

    [ModelDisplayName("IncludedNode")]
    public interface IModelMergedDifferenceStrategyIncludedNodePath : IModelMergedDifferenceStrategyNodePath {
        
    }

    [ModelDisplayName("ExcludedNode")]
    public interface IModelMergedDifferenceStrategyExcludedNodePath : IModelMergedDifferenceStrategyNodePath {
        
    }

    public interface IModelMergedDifference : IModelNode {
        [DataSourceProperty("Views")]
        [Required]
        [RefreshProperties(RefreshProperties.All)]
        IModelObjectView View { get; set; }
        [Browsable(false)]
        IModelList<IModelObjectView> Views { get; }
        [ModelBrowsable(typeof(MergedDifferenceColumnVisibilityCalculator))]
        bool InvisibleListViewColumns { get; set; }
        [DataSourceProperty("Strategies")]
        [Required]
        IModelMergedDifferenceStrategy Strategy { get; set; }
        [Browsable(false)]
        IModelList<IModelMergedDifferenceStrategy> Strategies { get; }
    }

    public class MergedDifferenceColumnVisibilityCalculator:IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return ((IModelMergedDifference) node).View as IModelListView!=null;
        }
    }

    [DomainLogic(typeof(IModelMergedDifference))]
    public class ModelMergedDViifferenceDomainLogic {

        public static IModelMergedDifferenceStrategy Get_Strategy(IModelMergedDifference differences) {
            return Get_Strategies(differences).FirstOrDefault(strategy => strategy.Id() == MergedDifferencesStrategiesGenerator.Everything);
        }

        public static IModelList<IModelMergedDifferenceStrategy> Get_Strategies(IModelMergedDifference differences) {
            return new CalculatedModelNodeList<IModelMergedDifferenceStrategy>(((IModelOptionsMergedDifferenceStrategy) differences.Application.Options).MergedDifferenceStrategies);
        }

        public static IModelList<IModelObjectView> Get_Views(IModelMergedDifference differences) {
            var modelObjectView = ((IModelObjectView)differences.Parent.Parent);
            if (modelObjectView.ModelClass == null)
                return new CalculatedModelNodeList<IModelObjectView>(differences.Application.Views.OfType<IModelObjectView>());
            var modelObjectViews = differences.Application.Views.OfType<IModelObjectView>().Where(view
                => view.ModelClass != null && (view.ModelClass.TypeInfo.Type.IsAssignableFrom(modelObjectView.ModelClass.TypeInfo.Type)));
            return new CalculatedModelNodeList<IModelObjectView>(modelObjectViews);
        }
    }
}