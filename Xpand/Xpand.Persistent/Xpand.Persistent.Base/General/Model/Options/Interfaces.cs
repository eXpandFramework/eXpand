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

    public abstract class GridListEditorVisibilityCalculatorHelper : EditorTypeVisibilityCalculator {

    }
    public class GridListEditorVisibilityCalculator : EditorTypeVisibilityCalculator {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            var typesInfo = node.Application.GetTypesInfo();
            var typeToTypeInfo = typesInfo.FindTypeInfo(typeof(GridListEditorVisibilityCalculatorHelper));
            var typeInfo = ReflectionHelper.FindTypeDescendants(typeToTypeInfo).SingleOrDefault();
            if (typeInfo != null) {
                var calculatorHelper = (GridListEditorVisibilityCalculatorHelper)typeInfo.Type.CreateInstance();
                return calculatorHelper.IsVisible(node, propertyName);
            }
            return false;
        }

        #endregion
    }

    [ModelDisplayName("GridView")]
    public interface IModelOptionsGridView : IModelOptionsColumnView {
    }

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