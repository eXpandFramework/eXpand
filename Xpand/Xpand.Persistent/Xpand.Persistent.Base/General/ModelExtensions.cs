using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Utils.Helpers;
using Xpand.Utils.Linq;
using Fasterflect;
using ModelSynchronizerList = Xpand.Persistent.Base.ModelAdapter.ModelSynchronizerList;
using ResourcesModelStore = DevExpress.ExpressApp.ResourcesModelStore;

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
    }

    public static class ModelNodeExtensions {
        public static ITypeInfo GetGenericListArgument(this IModelNode nodeByPath) {
            var type = nodeByPath.GetType();
            if (typeof(IEnumerable).IsAssignableFrom(type)) {
                var genericModelList = type.GetInterfaces().First(type1 => typeof(IEnumerable).IsAssignableFrom(type1) && type1.IsGenericType);
                return XafTypesInfo.Instance.FindTypeInfo(genericModelList.GetGenericArguments()[0]);
            }
            return null;
        }

        public static IEnumerable<IModelChoiceActionItem> ActionChoiceItems(this IModelNode modelnode, Frame frame) {
            return modelnode.Application.ActionDesign.Actions.Where(action => action.ChoiceActionItems != null && action.ChoiceActionItems.Any()).SelectMany(action => action.ChoiceActionItems);
        }

        public static ActionBase ToAction(this IModelAction modelAction, Frame frame){
            return frame.Actions().First(@base => @base.Model == modelAction);
        }

        private static ExpressionEvaluator GetExpressionEvaluator(IModelNode dataSourceNode, CriteriaOperator criteriaOperator) {
            var typeInfo = dataSourceNode.GetGenericListArgument();
            var descendants = ReflectionHelper.FindTypeDescendants(typeInfo);
            var propertyDescriptors = descendants.SelectMany(info => info.Members).DistinctBy(info => info.Name).Select(info => new XafPropertyDescriptor(info,info.Name)).Cast<PropertyDescriptor>().ToArray();
            var evaluatorContextDescriptor = new EvaluatorContextDescriptorDefault(new PropertyDescriptorCollection(propertyDescriptors));
            return new ExpressionEvaluator(evaluatorContextDescriptor, criteriaOperator, false, XpandModuleBase.Dictiorary.CustomFunctionOperators);    
        }

        public static IEnumerable<T> GetNodes<T>(this IEnumerable<T> modelNodes, string criteria) where T:IModelNode{
            var expressionEvaluator = GetExpressionEvaluator((IModelNode) modelNodes, CriteriaOperator.Parse(criteria));
            return expressionEvaluator!=null ? modelNodes.Where(arg => (bool) expressionEvaluator.Evaluate(arg)) : Enumerable.Empty<T>();
        }

        public static XPClassInfo QueryXPClassInfo(this IModelClass modelClass){
            return XpandModuleBase.Dictiorary.QueryClassInfo(modelClass.TypeInfo.Type);
        }

        public static XPMemberInfo GetXpmemberInfo(this IModelMember modelMember){
            return XpandModuleBase.Dictiorary.GetClassInfo(modelMember.ModelClass.TypeInfo.Type).FindMember(modelMember.Name);
        }

        public static TNode GetParent<TNode>(this IModelNode modelNode) where TNode : class, IModelNode{
            if (modelNode is TNode)
                return (TNode) modelNode;
            var parent = modelNode.Parent;
            while (!(parent is TNode)) {
                parent = parent.Parent;
                if (parent == null)
                    break;
            }
            return parent != null ? (TNode)parent : default(TNode);
        }

        public static void Undo(this IModelNode modelNode){
            ((ModelNode) modelNode).Undo();
        }

        public static IModelNode FindNodeByPath(this IModelNode modelNode,string nodePath){
            return ModelEditorHelper.FindNodeByPath(nodePath, (ModelNode) modelNode);
        }

        public static string Path(this IModelNode modelNode){
            return ModelEditorHelper.GetModelNodePath((ModelNode) modelNode);
        }

        public static string Xml(this IModelNode modelNode) {
            return ((ModelNode) modelNode).Xml;
        }

        public static object GetValue<T>(this IModelNode modelNode, Expression<Func<T, object>> property){
            var name = typeof(T).GetMemberInfo(property).Name;
            return modelNode.GetValue(name);
        }

        public static object GetValue(this IModelNode modelNode, string propertyName){
            var modelValueInfo = GetModelValueInfo(modelNode, propertyName);
            return GetValue(modelValueInfo.Item2, propertyName.Split('.').Last(), modelValueInfo.Item1.PropertyType);
        }

        public static Tuple<ModelValueInfo,IModelNode> GetModelValueInfo(this IModelNode modelNode, string propertyName) {
            if (propertyName.Contains(".")){
                var split = propertyName.Split('.');
                var strings = string.Join(".", split.Skip(1));
                var node = ((IModelNode) modelNode.GetValue(split.First()));
                return node.GetModelValueInfo(strings);
            }
            var modelValueInfo = ((ModelNode) modelNode).GetValueInfo(propertyName);
            return new Tuple<ModelValueInfo, IModelNode>(modelValueInfo, modelNode);
        }

        public static object GetValue(this IModelNode modelNode,string propertyName,Type propertyType) {
            return modelNode.CallMethod(new[]{propertyType}, "GetValue", propertyName);
        }

        public static void SetChangedValue(this IModelNode modelNode, string propertyName,  string value){
            modelNode.SetValue(propertyName, null,value);
        }

        public static void SetValue(this IModelNode modelNode,string propertyName,Type propertyType,object value){
            if (propertyType==null){
                var modelValueInfo = modelNode.GetModelValueInfo(propertyName).Item1;
                var changedValue = modelValueInfo.ChangedValue(value, modelValueInfo.PropertyType);
                modelNode.CallMethod(new[] { modelValueInfo.PropertyType }, "SetValue", propertyName, changedValue);
            }
            else
                modelNode.CallMethod(new[] { propertyType }, "SetValue", propertyName, value);
        }

        public static object ChangedValue(this ModelValueInfo modelValueInfo,object value, Type destinationType){
            var typeConverter = modelValueInfo.TypeConverter;
            return typeConverter != null ? typeConverter.ConvertFrom(value) : value.Change(destinationType);
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
            modelApplicationBases=modelApplicationBases.Concat(GetEmbebedApplications(modelApplicationBases, node)).ToList();
            ReadFromOtherLayers(modelApplicationBases, node);
            return modelApplicationBases;
        }
        
        private static IEnumerable<ModelApplicationBase> GetEmbebedApplications(IEnumerable<ModelApplicationBase> modelApplicationBases, ModelNode node){
            var moduleBases = modelApplicationBases.Cast<IModelSources>().SelectMany(sources => sources.Modules);
            return ResourceModelCollector.GetEmbededModelStores(moduleBases).Select(pair =>{
                var modelApplication = node.CreatorInstance.CreateModelApplication();
                modelApplication.Id = pair.Key;
                pair.Value.Load(modelApplication);
                return modelApplication;
            }).Cast<IModelApplication>().Where(application => application.Views!=null).Cast<ModelApplicationBase>();
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
                        view != null && view.MergedDifferences != null).SelectMany(differences => differences.MergedDifferences).ToArray();
                viewId = mergeDifferences.Select(GetViewIdCore).First(value => !string.IsNullOrEmpty(value));
            }
            return viewId;
        }

        static string GetViewIdCore(IModelMergedDifference modelMergedDifference){
            return Regex.Match(((ModelNode)modelMergedDifference).Xml, "View=\"([^\"]*)",RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value;
        }

        static ModelApplicationBase _strategiesModel;
        public static ModelApplicationBase StrategiesModel(this IModelApplication application, IEnumerable<ModelApplicationBase> modelApplicationBases) {
            if (_strategiesModel == null) {
                var strategies = ((IModelOptionsMergedDifferenceStrategy)application.Application.Options).MergedDifferenceStrategies;
                var xml = string.Format("<Application><Options>{0}</Options></Application>", ((ModelNode)strategies).Xml);
                var modelApplicationBase = ((ModelApplicationBase) application).CreatorInstance.CreateModelApplication();
                new ModelXmlReader().ReadFromString(modelApplicationBase, "", xml);
                ReadFromOtherLayers(modelApplicationBases, modelApplicationBase);
                UpdateRemovedNodes(modelApplicationBase);
                _strategiesModel = modelApplicationBase;
            }
            return _strategiesModel;
        }

        static void ReadFromOtherLayers(IEnumerable<ModelApplicationBase> modelApplicationBases,ModelApplicationBase modelApplicationBase) {
            foreach (var applicationBase in modelApplicationBases.Cast<IModelApplication>()){
                var mergedDifferenceStrategy = ((IModelOptionsMergedDifferenceStrategy) applicationBase.Options);
                if (mergedDifferenceStrategy != null){
                    var xml = mergedDifferenceStrategy.Xml();
                    if (!string.IsNullOrEmpty(xml)){
                        xml = string.Format("<Application>{0}</Application>", xml);
                        new ModelXmlReader().ReadFromString(modelApplicationBase, "", xml);
                    }
                }
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

        public static ModelApplicationBase GetLayer(this ModelApplicationBase modelApplicationBase, string id){
            var modelNodeWrapper = modelApplicationBase.GetLayers().FirstOrDefault(wrapper => wrapper.ModelNode.Id==id);
            return modelNodeWrapper != null ? (ModelApplicationBase) modelNodeWrapper.ModelNode : null;
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
