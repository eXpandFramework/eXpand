﻿using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;
using Fasterflect;

namespace Xpand.Persistent.Base.General.Model.Options {
    [ModelAbstractClass]
    public interface IModelListViewOptionsGridView : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
        IModelOptionsGridView GridViewOptions { get; }
        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
        IModelGridViewModelAdapters GridViewModelAdapters { get; }
    }

    [ModelNodesGenerator(typeof(ModelGridViewAdaptersNodeGenerator))]
    public interface IModelGridViewModelAdapters : IModelList<IModelGridViewModelAdapter>, IModelNode {

    }

    [ModelDisplayName("Adapter")]
    public interface IModelGridViewModelAdapter : IModelCommonModelAdapter<IModelOptionsGridView> {

    }

    [DomainLogic(typeof(IModelGridViewModelAdapter))]
    public class ModelGridViewModelAdapterDomainLogic : ModelAdapterDomainLogicBase<IModelOptionsGridView> {
        public static IModelList<IModelOptionsGridView> Get_ModelAdapters(IModelGridViewModelAdapter adapter) {
            return GetModelAdapters(adapter.Application);
        }
    }

    public class ModelGridViewAdaptersNodeGenerator : ModelAdapterNodeGeneratorBase<IModelOptionsGridView, IModelGridViewModelAdapter> {

    }

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

    [ModelDisplayName("GridView")]
    [ModuleUserAttribute(typeof(IGridOptionsUser))]
    public interface IModelOptionsGridView : IModelOptionsColumnView {
    }

    public interface IGridOptionsUser{
    }

    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class ModuleUserAttribute : Attribute{
        private readonly Type _moduleType;

        public ModuleUserAttribute(Type moduleType){
            _moduleType = moduleType;
        }

        public Type ModuleType{
            get { return _moduleType; }
        }
    }

    [ModelAbstractClass]
    public interface IModelListViewOptionsColumnView : IModelListView {
    }

    [ModelAbstractClass]
    public interface IModelOptionsColumnView : IModelModelAdapter {

    }

    [ModelAbstractClass]
    public interface IModelColumnOptionsColumnView : IModelColumn {

    }

    [ModelAbstractClass]
    public interface IModelColumnViewColumnOptions : IModelNodeEnabled{

    }

    [ModelAbstractClass]
    public interface IModelColumnOptionsGridView : IModelColumnOptionsColumnView {
        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
        IModelOptionsColumnGridView OptionsColumnGridView { get; }
    }

    [ModelDisplayName("ColumnGridViewOptions")]
    public interface IModelOptionsColumnGridView : IModelColumnViewColumnOptions {
    }

    
}