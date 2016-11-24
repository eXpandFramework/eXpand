using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.Web.ListEditors.TwoDimensionListEditor{
    [ModelAbstractClass]
    public interface IModelColumnSummaryItemTwoDimensionListEditor:IModelColumnSummaryItem{
        [ModelBrowsable(typeof(TwoDimensionListEditorVisibilityCalculator))]
        IModelTwoDimensionListEditorSummaryItem TwoDimensionListEditor { get; }
    }

    public class TwoDimensionListEditorVisibilityCalculator : EditorTypeVisibilityCalculator<TwoDimensionListEditor,IModelListView> {
    }

    public interface IModelTwoDimensionListEditorSummaryItem : IModelNode {
        [Description("Specifies where the summary appears."), DefaultValue(DimensionsEnum.Both)]
        [Category("Appearance")]
        DimensionsEnum SummaryAppearance { get; set; }
    }

    public enum DimensionsEnum {
        Vertical,
        Horizontal,
        Both
    }

    [ModelAbstractClass]
    public interface IModelListViewTwoDimensionListEditor:IModelListView{
        [ModelBrowsable(typeof(TwoDimensionListEditorVisibilityCalculator))]
        IModelTwoDimensionListEditor TwoDimensionListEditor { get; }
    }

    public interface IModelTwoDimensionListEditor : IModelNode {
        [Description("Specifies whether the two dimension editor will have checkboxes for selection."),
         DefaultValue(false)][Category("Appearance")]
        bool ShowCheckboxes { get; set; }

        [Description("Specifies whether the two dimension editor will use fast mode when checkboxes are selected."),
         DefaultValue(false)]
        [Category("Appearance")]
        bool FastMode { get; set; }
    }
}