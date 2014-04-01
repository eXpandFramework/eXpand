using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.Persistent.Base.ModelAdapter.Logic;
using Xpand.Utils.Linq;
using Fasterflect;
using ModelSynchronizerList = Xpand.Persistent.Base.ModelAdapter.ModelSynchronizerList;

namespace Xpand.Persistent.Base.General {
    public static class CustomModelSynchronizerHelper {
        public static void Assign(CreateCustomModelSynchronizerEventArgs e, IModelSynchronizable modelSynchronizer) {
            var modelSynchronizerList = e.ModelSynchronizer as ModelSynchronizerList;
            if (modelSynchronizerList == null) {
                e.ModelSynchronizer = new ModelSynchronizerList();
            }
            var synchronizerList = ((ModelSynchronizerList)e.ModelSynchronizer);
            synchronizerList.Add(modelSynchronizer);
        }

        public static void Assign<TModelAdaptorRule, TModelModelAdaptorRule>(CreateCustomModelSynchronizerEventArgs e, IModelSynchronizable modelSynchronizer, Frame frame, Func<TModelModelAdaptorRule, IModelSynchronizable> func)
            where TModelAdaptorRule : IModelAdaptorRule
            where TModelModelAdaptorRule : IModelNode {
            var modelAdaptorRuleController = frame.Controllers.ToList<Controller>().OfType<IModelAdaptorRuleController>().FirstOrDefault();
            if (modelAdaptorRuleController != null) {
                modelAdaptorRuleController.ExecuteLogic(typeof(TModelAdaptorRule), typeof(TModelModelAdaptorRule), rule => Assign(e, func.Invoke((TModelModelAdaptorRule)rule)));
            }
            Assign(e, modelSynchronizer);
        }
    }
    public static class ModelNodeExtensions {
        public static IModelNode GetParent<TNode>(this IModelNode node) where TNode : IModelNode {
            var modelNode = node;
            while (!(modelNode is TNode)) {
                if (modelNode.Parent == null)
                    return null;
                modelNode = modelNode.Parent;
            }
            return modelNode;
        }

        public static string Xml(this IModelNode modelNode) {
            return ((ModelNode) modelNode).Xml;
        }

        public static object GetValue(this IModelNode modelNode, string propertyName) {
            var modelValueInfo = ((ModelNode)modelNode).GetValueInfo(propertyName);
            return GetValue(modelNode, propertyName, modelValueInfo.PropertyType);
        }

        public static object GetValue(this IModelNode modelNode,string propertyName,Type propertyType) {
            return modelNode.CallMethod(new[]{propertyType}, "GetValue", propertyName);
        }
        public static void SetValue(this IModelNode modelNode,string propertyName,Type propertyType,object value) {
            modelNode.CallMethod(new[] { propertyType }, "SetValue", propertyName, value);
        }

        public static bool IsRemovedNode(this IModelNode modelNode) {
            return ((ModelNode) modelNode).IsRemovedNode;
        }

        public static bool IsNewNode(this IModelNode modelNode) {
            return ((ModelNode) modelNode).IsNewNode;
        }

        public static bool HasValue(this IModelNode modelNode,params Type[] interfacesToSearch  ){
            var valueInfos = ((ModelNode) modelNode).NodeInfo.ValuesInfo;
            string[] namesToSearch=valueInfos.Select(info => info.Name).ToArray();
            if (interfacesToSearch != null){
                namesToSearch = interfacesToSearch.SelectMany(type => type.Properties()).Select(info => info.Name).ToArray();
            }
            return valueInfos.Where(info => namesToSearch.Contains(info.Name)).Select(info => modelNode.HasValue(info.Name)).Any();
        }

        public static string Id(this IModelNode modelNode) {
            return ((ModelNode) modelNode).Id;
        }

        public static IEnumerable<IModelLayoutViewItem> ViewItems(this IEnumerable<IModelViewLayoutElement> modelViewLayout,IModelMemberViewItem memberViewItem) {
            var layoutViewItems = modelViewLayout.GetItems<ModelNode>(ModelEditorHelper.GetChildNodes).OfType<IModelLayoutViewItem>();
            return layoutViewItems.Where(item => item.ViewItem == memberViewItem);
        }
    }
    public class ModelNodeWrapper {
        readonly ModelNode _modelNode;

        public ModelNodeWrapper(ModelNode modelNode) {
            _modelNode = modelNode;
        }

        public ModelNode ModelNode {
            get { return _modelNode; }
        }

        public override string ToString() {
            return _modelNode.Id;
        }
    }

    public static class ModelApplicationBaseExtensions {
        public static List<ModelApplicationBase> GetModuleDifferences(this IModelApplication modelApplication) {
            var modelSources = ((IModelSources)modelApplication);
            var modelApplicationBases = new List<ModelApplicationBase>();
            var node = (ModelNode)modelApplication;
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
            var sources = ((IModelSources)node.Application);
            var targetSources = ((IModelSources)modelApplicationBase.Application);
            targetSources.BOModelTypes = sources.BOModelTypes;
            targetSources.Modules = sources.Modules;
            targetSources.Controllers = sources.Controllers;
            targetSources.Localizers = sources.Localizers;
            targetSources.EditorDescriptors = new EditorDescriptors(sources.EditorDescriptors);
        }

        static void ReadFromOtherLayers(IEnumerable<ModelApplicationBase> modelApplicationBases, ModelNode node) {
            var applicationBases = modelApplicationBases as ModelApplicationBase[] ?? modelApplicationBases.ToArray();
            var strategiesModel = node.Application.StrategiesModel(applicationBases);
            foreach (var modelApplicationBase in applicationBases) {
                var modelMergedDifferences = ModelMergedDifferences(modelApplicationBase);
                foreach (var modelMergedDifference in modelMergedDifferences) {
                    if (modelMergedDifference.View == null) {
                        ReadViewsFromOtherLayers(applicationBases, modelMergedDifference, modelApplicationBase);
                    }
                    if (!modelMergedDifference.HasValue("Strategy")) {
                        new ModelXmlReader().ReadFromModel(modelApplicationBase, strategiesModel);
                    }
                }
            }
        }

        static void ReadViewsFromOtherLayers(IEnumerable<ModelApplicationBase> modelApplicationBases, IModelMergedDifference modelMergedDifference, ModelApplicationBase modelApplicationBase) {
            var applicationBases = modelApplicationBases as ModelApplicationBase[] ?? modelApplicationBases.ToArray();
            var viewId = GetViewId(modelMergedDifference, applicationBases);
            var mergedView = (IModelObjectView)applicationBases.Cast<IModelApplication>().Select(application
                                                                                                      => application.Views[viewId]).FirstOrDefault(view => view != null);
            var modelNode = (ModelNode)mergedView;
            if (modelNode != null) {
                string viewName = "DetailView";
                if (modelNode is IModelListView)
                    viewName = "ListView";
                var s = @"<" + viewName + @" Id=""" + modelNode.Id + @"""></" + viewName + ">";
                var xml = "<Application><Views>" + s + "</Views></Application>";
                new ModelXmlReader().ReadFromString(modelApplicationBase, "", xml);
            }
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
        public static ModelApplicationBase StrategiesModel(this IModelApplication application, IEnumerable<ModelApplicationBase> modelApplicationBases) {
            if (_strategiesModel == null) {
                var strategies = ((IModelOptionsMergedDifferenceStrategy)application.Application.Options).MergedDifferenceStrategies;
                var xml = string.Format("<Application><Options>{0}</Options></Application>", ((ModelNode)strategies).Xml);
                var modelApplicationBase = ((ModelApplicationBase) application).CreatorInstance.CreateModelApplication();
                new ModelXmlReader().ReadFromString(modelApplicationBase, "", xml);
                ReadFromOtherLayers(modelApplicationBases, strategies, modelApplicationBase);
                UpdateRemovedNodes(modelApplicationBase);
                _strategiesModel = modelApplicationBase;
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
                return !modelNode1.IsNewNode;
            }
            return false;
        }

        static IEnumerable<IModelMergedDifference> ModelMergedDifferences(ModelApplicationBase modelApplicationBase) {
            var modelViews = ((IModelApplication)modelApplicationBase).Views.OfType<IModelObjectViewMergedDifferences>();
            return modelViews.Where(differences => differences.MergedDifferences != null && differences.MergedDifferences.Any())
                                                   .SelectMany(differences => differences.MergedDifferences);
        }

        public static void RemoveLayer(this ModelApplicationBase application, string id) {
            RefreshLayers(application, @base => @base.Id == id ? null : @base);
        }

        public static ITypesInfo GetTypesInfo(this IModelApplication application) {
            return ((IModelTypesInfoProvider) application).TypesInfo;
        }

        public static void ReplaceLayer(this ModelApplicationBase application, ModelApplicationBase layer) {
            RefreshLayers(application, @base => application.LastLayer.Id == layer.Id ? layer : @base);
        }

        static void RefreshLayers(ModelApplicationBase application, Func<ModelApplicationBase, ModelApplicationBase> func) {
            var modelApplicationBases = new List<ModelApplicationBase>();
            var lastLayer = application.LastLayer;
            ModelApplicationHelper.RemoveLayer(application);
            var afterSetup = application.LastLayer;
            ModelApplicationHelper.RemoveLayer(application);
            while (application.LastLayer.Id != "Unchanged Master Part") {
                ModelApplicationBase modelApplicationBase = application.LastLayer;
                modelApplicationBase = func.Invoke(modelApplicationBase);
                if (modelApplicationBase != null)
                    modelApplicationBases.Add(modelApplicationBase);
                ModelApplicationHelper.RemoveLayer(application);
            }
            modelApplicationBases.Reverse();
            foreach (var modelApplicationBase in modelApplicationBases) {
                ModelApplicationHelper.AddLayer(application, modelApplicationBase);
            }
            ModelApplicationHelper.AddLayer(application, afterSetup);
            ModelApplicationHelper.AddLayer(application, lastLayer);
        }

        public static void AddLayer(this ModelApplicationBase application, ModelNode layer) {
            ModelApplicationHelper.AddLayer(application, (ModelApplicationBase) layer);
        }

        public static void InsertLayer(this ModelApplicationBase application, int index,ModelApplicationBase layer) {
            application.CallMethod("InsertLayerAtInternal", layer, index);
        }

        public static void AddLayerBeforeLast(this ModelApplicationBase application, ModelApplicationBase layer) {
            ModelApplicationBase lastLayer = application.LastLayer;
            if (lastLayer.Id != "After Setup" && lastLayer.Id != "UserDiff")
                throw new ArgumentException("LastLayer.Id", lastLayer.Id);
            ModelApplicationHelper.RemoveLayer(application);
            ModelApplicationHelper.AddLayer(application, layer);
            ModelApplicationHelper.AddLayer(application, lastLayer);
        }

        public static ModelApplicationBase GetLayer(this ModelApplicationBase modelApplicationBase, int index) {
            return (ModelApplicationBase) ((List<ModelNode>)modelApplicationBase.GetPropertyValue("Layers"))[index];
        }

        public static List<ModelNodeWrapper> GetLayers(this ModelApplicationBase modelApplicationBase) {
            return ((List<ModelNode>)modelApplicationBase.GetPropertyValue("Layers")).Select(node => new ModelNodeWrapper(node)).ToList();
        }

        public static void ReInitLayers(this ModelApplicationBase modelApplicationBase) {
            if (modelApplicationBase.Id == "Application") {
                var lastLayer = modelApplicationBase.LastLayer;
                while (lastLayer.Id != "Unchanged Master Part") {
                    ModelApplicationHelper.RemoveLayer(lastLayer);
                    lastLayer = modelApplicationBase.LastLayer;
                }
                var afterSetupLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
                afterSetupLayer.Id = "After Setup";
                ModelApplicationHelper.AddLayer(modelApplicationBase, afterSetupLayer);
            }
        }

        public static bool HasAspect(this ModelApplicationBase modelApplicationBase, string aspectName) {
            for (int i = 0; i < modelApplicationBase.AspectCount; i++) {
                if (modelApplicationBase.GetAspect(i) == aspectName)
                    return true;
            }
            return false;
        }
    }
}
