using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Utils.Helpers;
using Xpand.Utils.Linq;
using Guard = DevExpress.ExpressApp.Utils.Guard;

namespace Xpand.Persistent.Base.ModelAdapter {
    class ParentCalculator {
        public static ModelNode GetParent(IModelNode node){
            var modelColumn = node.GetParent<IModelColumn>();
            return modelColumn != null ? (ModelNode) modelColumn : (ModelNode) node.GetParent<IModelListView>();
        }
    }
    public class MapModelReadOnlyCalculator : IModelIsReadOnly {

        #region Implementation of IModelIsReadOnly
        public bool IsReadOnly(IModelNode node, string propertyName) {
            var parent = ParentCalculator.GetParent(node);
            var helper = new FastModelEditorHelper();
            return helper.IsPropertyModelBrowsableVisible(parent, propertyName);
        }

        public bool IsReadOnly(IModelNode node, IModelNode childNode) {
            throw new NotImplementedException();
        }

        public bool IsReadOnly(IModelNode node) {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class MapModelValueCalculator : IModelValueCalculator {
        public object Calculate(ModelNode node, string propertyName) {
            if (node is IModelNodeEnabled) {
                var modelNode = ParentCalculator.GetParent(node);
                if (modelNode != null) {
                    var calculate = modelNode.GetValue(propertyName);
                    if (calculate != null && calculate.GetType().IsEnum) {
                        var propertyType = node.GetValueInfo(propertyName).PropertyType;
                        if (propertyType.IsNullableType()) {
                            var genericArgument = propertyType.GetGenericArguments()[0];
                            if (genericArgument != calculate.GetType())
                                return Enum.Parse(genericArgument, ((int)calculate).ToString(CultureInfo.InvariantCulture));
                        }
                    }
                    return calculate;
                }
            }
            throw new NotImplementedException(propertyName);
        }
        
    }

//    public interface IModelApplicationModelAdapterContexts:IModelApplication{
//        IModelModelAdapterContexts ModelAdapterContexts { get; }
//    }

//    [ModelNodesGenerator(typeof(ModelAdapterContextsNodeGenerator))]
//    public interface IModelModelAdapterContexts:IModelList<IModelModelAdapters>,IModelNode{
//
//    }

//    [DomainLogic(typeof (IModelModelAdapterContexts))]
//    public static class ModelModelAdapterContextsDomainLogic {
//        public static IEnumerable<T> GetAdapters<T>(this IModelModelAdapterContexts contexts) where T:IModelModelAdapter{
//            var modelAdapters = contexts.SelectMany(adapters => adapters);
//            return modelAdapters.Where(modelAdapter => modelAdapter.NodeEnabled).OfType<T>();
//        }
//    }
//    public class ModelAdapterContextsNodeGenerator:ModelNodesGeneratorBase{
//        public const string Default = "Default";
//        public const string ModelAdapterAttribute = "ModelAdapterAttribute";
//        protected override void GenerateNodesCore(ModelNode node){
//            node.AddNode<IModelModelAdapters>(Default);
//            node.AddNode<IModelModelAdapters>(ModelAdapterAttribute);
//        }
//    }

//    public interface IModelModelAdapterContext:IModelNode{
//        IModelModelAdapters DefaultModelAdapters { get; }
//    }

//    [ModelNodesGenerator(typeof(ModelAdaptersNodeGenerator))]
//    public interface IModelModelAdapters:IModelList<IModelModelAdapter>,IModelNode{
//         
//    }

//    [ModelAbstractClass]
//    public interface IModelModelAdapter:IModelNodeEnabled{
//    }

//    public class ModelAdaptersNodeGenerator:ModelNodesGeneratorBase{
//        protected override void GenerateNodesCore(ModelNode node){
//            var modelAdapterTypeInfos = XafTypesInfo.Instance.FindTypeInfo(typeof(IModelModelAdapter)).Descendants.Where(info
//                        => info.FindAttribute<ModelAbstractClassAttribute>(false) == null && info.IsInterface);
//            var installedInfos = GetInstalledAdapters(modelAdapterTypeInfos.ToArray(), node.Application).ToArray();
//            if (node.Id == ModelAdapterContextsNodeGenerator.Default){
//                foreach (var typeInfo in installedInfos){
//                    AddNode(node, typeInfo);
//                }
//            }
//            else if (node.Id == ModelAdapterContextsNodeGenerator.ModelAdapterAttribute){
//                foreach (var info in GetTypeInfos(node, installedInfos).SelectMany(infos => infos).Distinct()){
//                    var modelNode = AddNode(node, info);
//                    modelNode.Id = ModelAdapterContextsNodeGenerator.ModelAdapterAttribute + "-" + modelNode.Id;
//                }
//            }
//        }
//
//        private static IEnumerable<IEnumerable<ITypeInfo>> GetTypeInfos(ModelNode node, ITypeInfo[] installedInfos){
//            var modelMembers = node.Application.BOModel.SelectMany(c => c.OwnMembers)
//                    .Where(member => member.MemberInfo.FindAttributes<ModelAdapterAttribute>().Any());
//            foreach (var modelMember in modelMembers){
//                foreach (var adapterAttribute in modelMember.MemberInfo.FindAttributes<ModelAdapterAttribute>()){
//                    yield return installedInfos.Where(info => info.Type.Name == "IModel" + adapterAttribute.Adapter);
//                }
//            }
//        }
//
//        private ModelNode AddNode(ModelNode node, ITypeInfo typeInfo){
//            return node.AddNode(GetName(typeInfo), typeInfo.Type);
//        }
//
//        private IEnumerable<ITypeInfo> GetInstalledAdapters(ITypeInfo[] typeInfos, IModelApplication application){
//            var modules = ((IModelSources)application).Modules.ToArray();
//            var moduleAssemblies = modules.Select(@base => @base.GetType().Assembly);
//            var installedInfos = typeInfos.Where(info => moduleAssemblies.Contains(info.Type.Assembly));
//            var infos = typeInfos.Where(info => info.FindAttribute<ModuleUserAttribute>() != null)
//                .Select(source => new{source, moduleUserAttribute = source.FindAttribute<ModuleUserAttribute>()})
//                .Where(t => modules.Any(@base => t.moduleUserAttribute.ModuleType.IsInstanceOfType(@base)))
//                .Select(t => t.source);
//            return installedInfos.Concat(infos).Distinct();
//        }
//
//        private static string GetName(ITypeInfo typeInfo){
//            var displayNameAttribute = typeInfo.FindAttribute<ModelDisplayNameAttribute>();
//            return displayNameAttribute != null ? displayNameAttribute.ModelDisplayName : typeInfo.Type.Name.Replace("IModel", "");
//        }
//    }

    public enum FileLocation { None, ApplicationFolder, CurrentUserApplicationDataFolder }
//    public abstract class ModelAdapterController : ViewController {
//        protected void ExtendWithFont(ModelInterfaceExtenders extenders, InterfaceBuilder builder, Assembly assembly) {
//            var calcType = builder.CalcType(typeof(AppearanceObject), assembly);
//            extenders.Add(calcType, typeof(IModelAppearanceFont));
//            calcType = builder.CalcType(typeof(AppearanceObjectEx), assembly);
//            extenders.Add(calcType, typeof(IModelAppearanceFont));
//        }
//
//        protected IEnumerable<string> GetProperties(ModelInterfaceExtenders extenders, Type targetInterface) {
//            var types = extenders.GetInterfaceExtenders(targetInterface).Where(
//                    type => (type.Namespace + "").StartsWith("DevExpress"));
//            var properties = types.SelectMany(type => type.GetPublicProperties());
//            return targetInterface.GetPublicProperties().Union(properties).Select(info => info.Name);
//        }
//
//        public string GetPropertyName<TTarget>(Expression<Func<TTarget, object>> property) {
//            return ReflectionExtensions.GetPropertyName(property);
//        }
//
//        public virtual string GetPath(string name) {
//            const string folderName = "ModelAdaptor";
//            var storageFolder =InterfaceBuilder.RuntimeMode? Application.GetStorageFolder(folderName):Path.GetTempPath();
//            var modelAdaptorFolder = Path.Combine(storageFolder,folderName);
//            var path2 = folderName + name + ".dll";
//            var combine = Path.Combine(modelAdaptorFolder + "", path2);
//            return combine;
//        }
//    }

    public static class ModelAdapterExtension {
        public static ModelNode GetNodeByPath(this IModelNode node, string path) {
            const string rootNodeName = "Application";
            Guard.ArgumentNotNull(node, "node");
            Guard.ArgumentNotNullOrEmpty(path, "path");
            string[] items = path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var sourceNode = (items[0] == rootNodeName ? node.Root : node.GetNode(items[0])) ;
            sourceNode.ThrowIfNull();
            for (int i = 1; i < items.Length; ++i) {
                if (sourceNode == null) {
                    return null;
                }
                sourceNode = sourceNode.GetNode(items[i]);
            }
            return (ModelNode)sourceNode;
        }
    }

//    public interface IModelCommonModelAdapter<T> : IModelNodeEnabled where T:IModelModelAdapter{
//        [DataSourceProperty("ModelAdapters")]
//        T ModelAdapter { get; set; }
//        [Browsable(false)]
//        IModelList<T> ModelAdapters { get; }
//    }

//    public abstract class ModelAdapterNodeGeneratorBase<T, T2> : ModelNodesGeneratorBase
//                where T : IModelModelAdapter
//                where T2 : IModelCommonModelAdapter<T> {
//
//        protected override void GenerateNodesCore(ModelNode node) {
//            var generateNode = GenerateDefaultNode(node);
//            generateNode.SetValue(ModelValueNames.NeedsCachingKey, false);
//            foreach (var generateAttributeNode in GenerateAttributeNodes(node)){
//                generateAttributeNode.SetValue(ModelValueNames.NeedsCachingKey, false);
//            }
//            
//        }
//        protected override void UpdateCachedNodes(ModelNode node) {
//            if (node.GetNode(ModelAdapterContextsNodeGenerator.Default) == null) {
//                GenerateDefaultNode(node);
//                GenerateAttributeNodes(node);
//            }
//        }
//
//        private T2 GenerateDefaultNode(ModelNode node) {
//            var modelAdapter = ((IModelApplicationModelAdapterContexts)node.Application).ModelAdapterContexts.GetAdapters<T>().FirstOrDefault(arg => arg.GetParent<IModelModelAdapters>().Id()!= ModelAdapterContextsNodeGenerator.ModelAdapterAttribute);
//            var newNode = node.AddNode<T2>(ModelAdapterContextsNodeGenerator.Default);
//            newNode.ModelAdapter = modelAdapter;
//            newNode.Index = 0;
//            return newNode;
//        }
//        private List<T2> GenerateAttributeNodes(ModelNode node) {
//            var modelMemberViewItem = node.GetParent<IModelMemberViewItem>();
//            var nodes = new List<T2>();
//            if (modelMemberViewItem != null){
//                var modelAdapterAttributes = modelMemberViewItem.ModelMember.MemberInfo.FindAttributes<ModelAdapterAttribute>().ToArray();
//                
//                for (var index = 0;index < modelAdapterAttributes.Length; index++){
//                    var modelAdapterAttribute =modelAdapterAttributes[index];
//                    if (typeof(T).Name.EndsWith(modelAdapterAttribute.Adapter.ToString())){
//                        var id = ModelAdapterContextsNodeGenerator.ModelAdapterAttribute + "-"+ modelAdapterAttribute.Adapter;
//                        var newNode = node.AddNode<T2>(id);
//                        var modelAdapter = ((IModelApplicationModelAdapterContexts) node.Application).ModelAdapterContexts[
//                            ModelAdapterContextsNodeGenerator.ModelAdapterAttribute].OfType<T>().FirstOrDefault(arg => arg.Id() == id);
//                        newNode.ModelAdapter = modelAdapter;
//                        newNode.Index=index+1;
//                        nodes.Add(newNode);
//                    }
//                }
//            }
//            return nodes;
//        }
//    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ModelAdapterAttribute:Attribute{
        public ModelAdapterAlias Adapter { get; }

        public ModelAdapterAttribute( ModelAdapterAlias adapter){
            Adapter = adapter;
        }
    }

    public enum ModelAdapterAlias{

        ASPxDateEditControl,
        DashboardViewEditor,
        DashboardViewer,
        HtmlEditor,
        ASPxHyperLinkControl,
        ASPxLookupDropDownEditControl,
        ASPxLookupFindEditControl,
        ASPxSearchDropDownEditControl,
        ASPxSpinEditControl,
        RepositoryItemBaseSpinEdit,
        RepositoryItemBlobBaseEdit,
        RepositoryItemButtonEdit,
        RepositoryItemCalcEdit,
        RepositoryItemCheckedComboBoxEdit,
        RepositoryItemCheckEdit,
        RepositoryItemColorEdit,
        RepositoryItemColorPickEdit,
        RepositoryItemColorComboBox,
        RepositoryItemDateEdit,
        RepositoryItemFontEdit,
        RepositoryItemHyperLinkEdit,
        RepositoryItemImageComboBox,
        RepositoryItemImageEdit,
        RepositoryItemLookUpEdit,
        RepositoryItemLookUpEditBase,
        RepositoryItemMarqueeProgressBar,
        RepositoryItemMemoEdit,
        RepositoryItemMemoExEdit,
        RepositoryItemMRUEdit,
        RepositoryItemObjectEdit,
        RepositoryItemPictureEdit,
        RepositoryItemPopupBase,
        RepositoryItemPopupBaseAutoSearchEdit,
        RepositoryItemPopupContainerEdit,
        RepositoryItemPopupCriteriaEdit,
        RepositoryItemPopupExpressionEdit,
        RepositoryItemPopupProgressEdit,
        RepositoryItemProtectedContextTextEdit,
        RepositoryItemRadioGroup,
        RepositoryItemRangeTrackBar,
        RepositoryItemRtfEditEx,
        RepositoryItemSpinEdit,
        RepositoryItemTextEdit,
        RepositoryItemTimeEdit,
        RepositoryItemTrackBar,
        RepositoryItemZoomTrackBar,
        RichEdit,
        LabelControl,
        FilterControl,
        LayoutControlGroup,
        UploadControl

    }

//    public abstract class ModelAdapterDomainLogicBase<T> where T : IModelModelAdapter{
//
//        public static IModelList<T> GetModelAdapters(IModelApplication modelApplication) {
//            return new CalculatedModelNodeList<T>(((IModelApplicationModelAdapterContexts)modelApplication).ModelAdapterContexts.GetAdapters<T>());
//        }
//    }

//    public interface IModelModelAdapterLink : IModelNode {
//        [DataSourceProperty("ModelAdapterContexts")]
//        [Category("eXpand.ModelAdapters")]
//        [RefreshProperties(RefreshProperties.All)]
//        [Required]
//        IModelList<IModelModelAdapter> ModelAdapterContext { get; set; }
//
//        [Browsable(false)]
//        IEnumerable<IModelModelAdapters> ModelAdapterContexts { get; }
//
//        [Category("eXpand.ModelAdapters")]
//        [DataSourceProperty("ModelAdapters")]
//        IModelModelAdapter ModelAdapter { get; set; }
//
//        [Browsable(false)]
//        IEnumerable<IModelModelAdapter> ModelAdapters { get; }
//    }

//    [DomainLogic(typeof(IModelModelAdapterLink))]
//    public class ModelModelAdapterLinkDomainLogic {
//        public static IEnumerable<IModelModelAdapters> Get_ModelAdapterContexts(IModelModelAdapterLink controlGroup) {
//            return ((IModelApplicationModelAdapterContexts)controlGroup.Application).ModelAdapterContexts;
//        }
//
//        public static IModelList<IModelModelAdapter> Get_ModelAdapterContext(IModelModelAdapterLink controlGroup) {
//            return new CalculatedModelNodeList<IModelModelAdapter>(((IModelApplicationModelAdapterContexts)controlGroup.Application).ModelAdapterContexts.SelectMany(adapters => adapters));
//        }
//
//        public static IEnumerable<IModelModelAdapter> Get_ModelAdapters(IModelModelAdapterLink controlGroup) {
//            return controlGroup.ModelAdapterContext ?? Enumerable.Empty<IModelModelAdapter>();
//        }
//    }
}