using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;
using Fasterflect;

namespace Xpand.Persistent.Base.General.Model.Options {
//    [ModelAbstractClass]
//    public interface IModelListViewOptionsGridView : IModelListViewOptionsColumnView {
////        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
////        IModelOptionsGridView GridViewOptions { get; }
//        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
//        IModelGridViewModelAdapters GridViewModelAdapters { get; }
//    }

//    [ModelNodesGenerator(typeof(ModelGridViewAdaptersNodeGenerator))]
//    public interface IModelGridViewModelAdapters : IModelList<IModelGridViewModelAdapter>, IModelNode {
//        
//    }

//    [ModelDisplayName("Adapter")]
//    public interface IModelGridViewModelAdapter : IModelCommonModelAdapter<IModelOptionsGridView> {
//
//    }

//    [DomainLogic(typeof(IModelGridViewModelAdapter))]
//    public class ModelGridViewModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelOptionsGridView> {
//        public static IModelList<IModelOptionsGridView> Get_ModelAdapters(IModelGridViewModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }

//    public class ModelGridViewAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelOptionsGridView, IModelGridViewModelAdapter> {
//
//    }

    public abstract class GridListEditorVisibilityCalculatorHelper : EditorTypeVisibilityCalculator<IModelListView> {

    }
    public class GridListEditorVisibilityCalculator : EditorTypeVisibilityCalculator<IModelListView> {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            var typesInfo = XafTypesInfo.Instance;
            var typeToTypeInfo = typesInfo.FindTypeInfo(typeof(GridListEditorVisibilityCalculatorHelper));
            var typeInfo = ReflectionHelper.FindTypeDescendants(typeToTypeInfo).SingleOrDefault();
            if (typeInfo != null) {
                var calculatorHelper = (GridListEditorVisibilityCalculatorHelper)typeInfo.Type.CreateInstance();
                return IsVisibleCore(node, propertyName, calculatorHelper);
            }
            return false;
        }

        private static bool IsVisibleCore(IModelNode node, string propertyName, GridListEditorVisibilityCalculatorHelper calculatorHelper){
            var isVisible = calculatorHelper.IsVisible(node, propertyName);
            if (isVisible) {
                var modelListView = node as IModelListView ?? node.GetParent<IModelListView>();
                return !modelListView.BandsLayout.Enable;

            }
            return false;
        }

        #endregion
    }

//    [ModelDisplayName("GridView")]
//    [ModuleUser(typeof(IGridOptionsUser))]
//    public interface IModelOptionsGridView : IModelOptionsColumnView {
//    }

//    public interface IGridOptionsUser{
//    }

//    [AttributeUsage(AttributeTargets.Interface)]
//    public sealed class ModuleUserAttribute : Attribute{
//        public ModuleUserAttribute(Type moduleType){
//            ModuleType = moduleType;
//        }
//
//        public Type ModuleType { get; }
//    }

//    [ModelAbstractClass]
//    public interface IModelListViewOptionsColumnView : IModelListView {
//    }

//    [ModelAbstractClass]
//    public interface IModelOptionsColumnView : IModelModelAdapter {
//
//    }


//    [ModelAbstractClass]
//    public interface IModelColumnOptionsColumnView : IModelColumn, IModelModelAdapter {
//
//    }

//    [ModelAbstractClass]
//    public interface IModelColumnViewColumnOptions : IModelModelAdapter {
//
//    }

//    [ModelAbstractClass]
//    public interface IModelColumnOptionsGridView : IModelColumnOptionsColumnView {
////        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
////        IModelOptionsColumnGridView OptionsColumnGridView { get; }
////        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
////        IModelGridColumnModelAdapters OptionModelAdapters { get; }
//    }

//    [ModelNodesGenerator(typeof(ModelGridColumnAdaptersNodeGenerator))]
//    public interface IModelGridColumnModelAdapters : IModelList<IModelColumnGridViewModelAdapter>, IModelNode {
//        [DefaultValue(true)]
//        bool AlwaysApplyDefault { get; set; }
//    }

//    public class ModelGridColumnAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelOptionsColumnGridView, IModelColumnGridViewModelAdapter> {
//
//    }

//    [ModelDisplayName("Adapter")]
//    public interface IModelColumnGridViewModelAdapter : IModelCommonModelAdapter<IModelOptionsColumnGridView> {
//
//    }

//    [DomainLogic(typeof(IModelColumnGridViewModelAdapter))]
//    public class ModelGridColumnModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelOptionsColumnGridView> {
//        public static IModelList<IModelOptionsColumnGridView> Get_ModelAdapters(IModelColumnGridViewModelAdapter adapter) {
//            return GetModelAdapters(adapter.Application);
//        }
//    }


//    [ModelDisplayName("ColumnGridViewOptions")]
//    [ModuleUser(typeof(IGridOptionsUser))]
//    public interface IModelOptionsColumnGridView : IModelColumnViewColumnOptions {
//    }

}