using System;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.ListEditors;
using Xpand.Persistent.Base.ModelAdapter;
using System.Linq;

namespace Xpand.ExpressApp.Model.Options {
    [ModelAbstractClass]
    public interface IModelListViewOptionsGridView : IModelListViewOptionsColumnView {
        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
        IModelOptionsGridView GridViewOptions { get; }
    }

    public abstract class GridListEditorVisibilityCalculatorHelper : EditorTypeVisibilityCalculator {

    }
    public class GridListEditorVisibilityCalculator : EditorTypeVisibilityCalculator {
        #region Overrides of EditorTypeVisibilityCalculator
        public override bool IsVisible(IModelNode node, string propertyName) {
            var typesInfo = node.Application.GetTypesInfo();
            var typeToTypeInfo = typesInfo.FindTypeInfo(typeof(GridListEditorVisibilityCalculatorHelper));
            var typeInfo = ReflectionHelper.FindTypeDescendants(typeToTypeInfo).Single();
            var calculatorHelper = (GridListEditorVisibilityCalculatorHelper)Activator.CreateInstance(typeInfo.Type);
            return calculatorHelper.IsVisible(node, propertyName);
        }

        #endregion
    }

    public interface IModelOptionsGridView : IModelOptionsColumnView {
    }

    public interface IModelListViewOptionsColumnView : IModelListView {
    }
    public interface IModelOptionsColumnView : IModelNodeEnabled {

    }
    [ModelAbstractClass]
    public interface IModelColumnOptionsColumnView : IModelColumn {

    }
    public interface IModelColumnViewColumnOptions : IModelNodeEnabled {

    }

    [ModelAbstractClass]
    public interface IModelColumnOptionsGridView : IModelColumnOptionsColumnView {
        [ModelBrowsable(typeof(GridListEditorVisibilityCalculator))]
        IModelOptionsColumnGridView OptionsColumnGridView { get; }
    }
    public interface IModelOptionsColumnGridView : IModelColumnViewColumnOptions {
    }

}