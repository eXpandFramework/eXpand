using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;

namespace Xpand.ExpressApp.Web.ListEditors.EditableTabEnabledListEditor{
    [ModelAbstractClass]
    public interface IModelColumnSummaryItemEditabledTabEnabled:IModelColumnSummaryItem{
        [ModelBrowsable(typeof(EditableTabEnabledListEditorVisibilityCalculator))]
        IModelEditableTabEnabledEditorSummaryItem EditableTabEnabledEditorSummaryItem { get; }
    }

    public class EditableTabEnabledListEditorVisibilityCalculator : EditorTypeVisibilityCalculator<EditableTabEnabledListEditor> {
    }

    public interface IModelEditableTabEnabledEditorSummaryItem : IModelNode {
        [Description("Specifies caption next to the summary."), DefaultValue("SUM: ")]
        [Category("Appearance")]
        string SummaryCaption { get; set; }
    }

    [ModelAbstractClass]
    public interface IModelListViewEditableTabEnabledEditor:IModelListView{
        IModelEditableTabEnabledEditor EditableTabEnabledEditor { get; }
    }

    public interface IModelEditableTabEnabledEditor : IModelNode{
        [Description("Specifies the number of columns visible per row when EditableTabEnabledListEditor is used."),
         DefaultValue(8)]
        [Category("Appearance")]
        int ColumnsPerRow { get; set; }

        [Description(
            "Specifies whether the header row is rendered on top of all rows (or for every row) when EditableTabEnabledListEditor is used."
            ), DefaultValue(false)]
        [Category("Appearance")]
        bool HeadersOnTopOnly { get; set; }

        [Description("Specifies if the editor will show buttons for adding and deleting records."), DefaultValue(true)]
        [Category("Appearance")]
        bool ShowButtons { get; set; }
    }
}