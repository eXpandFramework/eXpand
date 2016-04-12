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
                        if (IsNullableType(propertyType)) {
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
        public bool IsNullableType(Type theType) {
            if (theType.IsGenericType) {
                var genericTypeDefinition = theType.GetGenericTypeDefinition();
                if (genericTypeDefinition != null) return (genericTypeDefinition == typeof(Nullable<>));
            }
            return false;
        }
    }

    public interface IModelApplicationModelAdapterContexts:IModelApplication{
        IModelModelAdapterContexts ModelAdapterContexts { get; }
    }

    [ModelNodesGenerator(typeof(ModelAdapterContextsNodeGenerator))]
    public interface IModelModelAdapterContexts:IModelList<IModelModelAdapters>,IModelNode{

    }

    [DomainLogic(typeof (IModelModelAdapterContexts))]
    public static class ModelModelAdapterContextsDomainLogic {
        public static IEnumerable<T> GetAdapters<T>(this IModelModelAdapterContexts contexts) where T:IModelModelAdapter{
            var modelAdapters = contexts.SelectMany(adapters => adapters);
            return modelAdapters.Where(modelAdapter => modelAdapter.NodeEnabled).OfType<T>();
        }
    }
    public class ModelAdapterContextsNodeGenerator:ModelNodesGeneratorBase{
        public const string Default = "Default";
        protected override void GenerateNodesCore(ModelNode node){
            node.AddNode<IModelModelAdapters>(Default);
        }
    }

    public interface IModelModelAdapterContext:IModelNode{
        IModelModelAdapters DefaultModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelAdaptersNodeGenerator))]
    public interface IModelModelAdapters:IModelList<IModelModelAdapter>,IModelNode{
         
    }

    [ModelAbstractClass]
    public interface IModelModelAdapter:IModelNodeEnabled{
    }

    public static class ModelModelAdapterDomainLogic{
        public static IEnumerable<IModelModelAdapter> GetContextAdapters(this IModelModelAdapter modelModelAdapter) {

            var modelAdapters = modelModelAdapter.GetNode("ModelAdapters");
            var adapters = new List<IModelModelAdapter>();
            if (modelAdapters!=null){
                for (int i = 0; i < modelAdapters.NodeCount; i++){
                    var modelAdapter = modelAdapters.GetNode(i);
                    var modelModelAdapters =
                        ((IModelApplicationModelAdapterContexts) modelModelAdapter.Application).ModelAdapterContexts[
                            modelAdapter.Id()];
                    var id = ((ModelNode) modelAdapter.GetValue("ModelAdapter")).Id;
                    adapters.Add(modelModelAdapters[id]);
                }
            }
            return adapters;
        } 
    }
    public class ModelAdaptersNodeGenerator:ModelNodesGeneratorBase{
        protected override void GenerateNodesCore(ModelNode node){
            var modelAdapterTypeInfos = XafTypesInfo.Instance.FindTypeInfo(typeof(IModelModelAdapter)).Descendants.Where(info 
                => info.FindAttribute<ModelAbstractClassAttribute>(false) == null&&info.IsInterface );
            
            var installedInfos = GetInstalledAdapters(modelAdapterTypeInfos, node.Application);
            foreach (var typeInfo in installedInfos) {
                node.AddNode(GetName(typeInfo), typeInfo.Type);
            }
        }

        private static IEnumerable<ITypeInfo> GetInstalledAdapters(IEnumerable<ITypeInfo> typeInfos, IModelApplication application){
            var modules = ((IModelSources)application).Modules;
            var moduleBases = modules as ModuleBase[] ?? modules.ToArray();
            var moduleAssemblies = moduleBases.Select(@base => @base.GetType().Assembly);
            var enumerable = typeInfos as ITypeInfo[] ?? typeInfos.ToArray();
            var installedInfos = enumerable.Where(info => moduleAssemblies.Contains(info.Type.Assembly));
            var infos = enumerable.Where(info => info.FindAttribute<ModuleUserAttribute>() != null)
                .Select(source => new{source, moduleUserAttribute = source.FindAttribute<ModuleUserAttribute>()})
                .Where(@t => moduleBases.Any(@base => t.moduleUserAttribute.ModuleType.IsInstanceOfType(@base)))
                .Select(@t => @t.source);
            return installedInfos.Concat(infos);
        }

        private static string GetName(ITypeInfo typeInfo){
            var displayNameAttribute = typeInfo.FindAttribute<ModelDisplayNameAttribute>();
            return displayNameAttribute != null ? displayNameAttribute.ModelDisplayName : typeInfo.Type.Name.Replace("IModel", "");
        }
    }

    public enum FileLocation { None, ApplicationFolder, CurrentUserApplicationDataFolder }
    public abstract class ModelAdapterController : ViewController {
        protected void ExtendWithFont(ModelInterfaceExtenders extenders, InterfaceBuilder builder, Assembly assembly) {
            var calcType = builder.CalcType(typeof(AppearanceObject), assembly);
            extenders.Add(calcType, typeof(IModelAppearanceFont));
            calcType = builder.CalcType(typeof(AppearanceObjectEx), assembly);
            extenders.Add(calcType, typeof(IModelAppearanceFont));
        }

        protected IEnumerable<string> GetProperties(ModelInterfaceExtenders extenders, Type targetInterface) {
            var types = extenders.GetInterfaceExtenders(targetInterface).Where(
                    type => (type.Namespace + "").StartsWith("DevExpress"));
            var properties = types.SelectMany(type => type.GetPublicProperties());
            return targetInterface.GetPublicProperties().Union(properties).Select(info => info.Name);
        }

        public string GetPropertyName<TTarget>(Expression<Func<TTarget, object>> property) {
            return ReflectionExtensions.GetPropertyName(property);
        }

        public virtual string GetPath(string name) {
            const string folderName = "ModelAdaptor";
            var modelAdaptorFolder = Path.Combine(Application.GetStorageFolder(folderName),folderName);
            var path2 = folderName + name + ".dll";
            return Path.Combine(modelAdaptorFolder + "", path2);
        }
    }

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

    public interface IModelCommonModelAdapter<T> : IModelNodeEnabled where T:IModelModelAdapter{
        [DataSourceProperty("ModelAdapters")]
        T ModelAdapter { get; set; }
        [Browsable(false)]
        IModelList<T> ModelAdapters { get; }
    }

    public abstract class ModelAdapterNodeGeneratorBase<T, T2> : ModelNodesGeneratorBase
        where T : IModelModelAdapter
        where T2 : IModelCommonModelAdapter<T> {
        protected override void GenerateNodesCore(ModelNode node){
            var modelOptionsAdvBandedView = ((IModelApplicationModelAdapterContexts)node.Application).ModelAdapterContexts.GetAdapters<T>().FirstOrDefault();
            var optionsAdvBandedView = node.AddNode<T2>("Default");
            optionsAdvBandedView.ModelAdapter=modelOptionsAdvBandedView;
        }
    }

    public abstract class ModelAdapterDomainLogicBase<T> where T : IModelModelAdapter{
        public static IModelList<T> GetModelAdapters(IModelApplication modelApplication) {
            return new CalculatedModelNodeList<T>(((IModelApplicationModelAdapterContexts)modelApplication).ModelAdapterContexts.GetAdapters<T>());
        }
    }

    public interface IModelModelAdapterLink : IModelNode {
        [DataSourceProperty("ModelAdapterContexts")]
        [Category("eXpand.ModelAdapters")]
        [RefreshProperties(RefreshProperties.All)]
        [Required]
        IModelModelAdapters ModelAdapterContext { get; set; }

        [Browsable(false)]
        IEnumerable<IModelModelAdapters> ModelAdapterContexts { get; }

        [Category("eXpand.ModelAdapters")]
        [DataSourceProperty("ModelAdapters")]
        IModelModelAdapter ModelAdapter { get; set; }

        [Browsable(false)]
        IEnumerable<IModelModelAdapter> ModelAdapters { get; }
    }

    [DomainLogic(typeof(IModelModelAdapterLink))]
    public class ModelModelAdapterLinkDomainLogic {
        public static IEnumerable<IModelModelAdapters> Get_ModelAdapterContexts(IModelModelAdapterLink controlGroup) {
            return ((IModelApplicationModelAdapterContexts)controlGroup.Application).ModelAdapterContexts;
        }

        public static IModelModelAdapters Get_ModelAdapterContext(IModelModelAdapterLink controlGroup) {
            return ((IModelApplicationModelAdapterContexts)controlGroup.Application).ModelAdapterContexts[ModelAdapterContextsNodeGenerator.Default];
        }

        public static IEnumerable<IModelModelAdapter> Get_ModelAdapters(IModelModelAdapterLink controlGroup) {
            return controlGroup.ModelAdapterContext ?? Enumerable.Empty<IModelModelAdapter>();
        }
    }
}