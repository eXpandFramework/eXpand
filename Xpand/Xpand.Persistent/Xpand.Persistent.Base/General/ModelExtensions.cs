using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using Xpand.Utils.Linq;
using Fasterflect;
using Xpand.Persistent.Base.Security;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.General {
    public static class ModelNodeExtensions {

        public static IEnumerable<IModelPropertyEditor> GetModelPropertyEditors(this IModelDetailView modelDetailView){
            var modelPropertyEditors = modelDetailView.Items.OfType<IModelPropertyEditor>();
            foreach (var modelPropertyEditor in modelPropertyEditors){
                if (modelPropertyEditor.ModelMember.MemberInfo.IsList){
                    if (modelPropertyEditor.View is IModelListView modelListView){
                        if (modelListView.MasterDetailMode==MasterDetailMode.ListViewAndDetailView){
                            foreach (var propertyEditor in modelListView.MasterDetailView.GetModelPropertyEditors()){
                                yield return propertyEditor;
                            }
                        }
                        else{
                            yield return modelPropertyEditor;
                        }
                    }
                }
                else{
                    yield return modelPropertyEditor;
                }
            }
        }

        public static IModelList<IModelClass> ModelClasses(this IModelNode modelNode, Type assignableFromType) {
            var modelClasses = modelNode.Application.BOModel.Where(modelClass
                => assignableFromType.IsAssignableFrom(modelClass.TypeInfo.Type) && !modelClass.TypeInfo.IsAbstract &&
                   new[] { typeof(ISecurityRelated), typeof(ISecurityPermisssionPolicyRelated) }.Any(
                       type => type.IsAssignableFrom(modelClass.TypeInfo.Type)));
            return new CalculatedModelNodeList<IModelClass>(modelClasses);
        }

        public static void UpdateValue<T>(this T targetNode,IModelNode sourceNode, params Expression<Func<T, object>>[] expressions) where T:IModelNode {
            foreach (var expression in expressions){
                var attributeName = targetNode.GetPropertyName(expression);
                if (sourceNode.HasValue(attributeName)) {
                    var value = sourceNode.GetValue<T>(attributeName);
                    sourceNode.ClearValue(attributeName);
                    targetNode.SetValue(attributeName, value);
                }
            }
        }

        public static void ClearValue<TNode>(this TNode node,Expression<Func<TNode, object>> expression) where TNode:IModelNode{
            node.ClearValue(node.GetPropertyName(expression));
        }

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

        public static ActionBase ToAction(this IModelAction modelAction) {
            return modelAction.GetValue<ActionBase>(ModelActionsNodesGenerator.ActionPropertyName);
        }

        public static ActionBase ToAction(this IModelAction modelAction, Frame frame){
            return frame.Actions().First(@base => @base.Model.Id == modelAction.Id);
        }

        private static ExpressionEvaluator GetExpressionEvaluator(IModelNode dataSourceNode, CriteriaOperator criteriaOperator) {
            var typeInfo = dataSourceNode.GetGenericListArgument();
            var descendants = ReflectionHelper.FindTypeDescendants(typeInfo);
            var propertyDescriptors = descendants.SelectMany(info => info.Members).DistinctBy(info => info.Name).Select(info => new XafPropertyDescriptor(info,info.Name)).Cast<PropertyDescriptor>().ToArray();
            var evaluatorContextDescriptor = new EvaluatorContextDescriptorDefault(new PropertyDescriptorCollection(propertyDescriptors));
            return new ExpressionEvaluator(evaluatorContextDescriptor, criteriaOperator, false,
                ((TypesInfo) XafTypesInfo.Instance).EntityStores.OfType<XpoTypeInfoSource>().First().XPDictionary.CustomFunctionOperators);    
        }

        public static IEnumerable<T> GetNodes<T>(this IEnumerable<T> modelNodes, string criteria) where T:IModelNode{
            var expressionEvaluator = GetExpressionEvaluator((IModelNode) modelNodes, CriteriaOperator.Parse(criteria));
            return expressionEvaluator!=null ? modelNodes.Where(arg => (bool) expressionEvaluator.Evaluate(arg)) : Enumerable.Empty<T>();
        }

        public static XPClassInfo QueryXPClassInfo(this IModelClass modelClass){
            return modelClass.TypeInfo.QueryXPClassInfo();
        }

        public static XPMemberInfo GetXpmemberInfo(this IModelMember modelMember){
            return ((XpoTypeInfoSource) ((TypeInfo) modelMember.ModelClass.TypeInfo).Source).XPDictionary.GetClassInfo(
                    modelMember.ModelClass.TypeInfo.Type).FindMember(modelMember.Name);
        }
        public static TNode GetParent<TNode>(this IModelNode modelNode) where TNode : class, IModelNode{
            if (modelNode is TNode node)
                return node;
            var parent = modelNode.Parent;
            while (!(parent is TNode)) {
                parent = parent.Parent;
                if (parent == null)
                    break;
            }
            return (TNode) parent;
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
            var valueInfos = ((ModelNode) modelNode).NodeInfo.ValuesInfo.ToArray();
            var namesToSearch=valueInfos.Select(info => info.Name);
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

        public ModelNode ModelNode => _modelNode;

        public override string ToString() {
            return _modelNode.Id;
        }
    }

    public static class ModelApplicationBaseExtensions {

        public static Platform GetPlatform(this IModelApplication application){
            return ((IModelSources) application).Modules.GetPlatform();
        }

        public static Platform GetPlatform(this ModelApplicationBase applicationBase){
            return ((IModelApplication) applicationBase).GetPlatform();
        }

        public static void RemoveLayer(this ModelApplicationBase application){
            ModelApplicationHelper.RemoveLayer(application);
        }

        public static void RemoveLayer(this ModelApplicationBase application, string id) {
            RefreshLayers(application, @base => @base.Id == id ? null : @base);
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
                throw new ArgumentException(@"LastLayer.Id", lastLayer.Id);
            ModelApplicationHelper.RemoveLayer(application);
            ModelApplicationHelper.AddLayer(application, layer);
            ModelApplicationHelper.AddLayer(application, lastLayer);
        }

        public static ModelApplicationBase GetLayer(this ModelApplicationBase modelApplicationBase, string id){
            var modelNodeWrapper = modelApplicationBase.GetLayers().FirstOrDefault(wrapper => wrapper.ModelNode.Id==id);
            return (ModelApplicationBase) modelNodeWrapper?.ModelNode;
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
