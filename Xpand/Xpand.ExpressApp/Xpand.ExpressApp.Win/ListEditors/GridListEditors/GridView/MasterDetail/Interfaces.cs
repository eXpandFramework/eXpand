using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Summary;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Skins;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail {
    public static class MasterDetailColumnViewExtensions {
        public static bool IsDetailView(this IMasterDetailColumnView columnView, IColumnViewEditor editor) {
            return columnView != editor.Grid.MainView;
        } 
    }
    public interface IMasterDetailColumnView {
        Window Window { get; set; }
        Frame MasterFrame { get; set; }
        GridControl GridControl { get; set; }
        object DataSource { get; }
        string GetRelationName(int rowHandle, int relationIndex);
        event MasterRowGetRelationCountEventHandler MasterRowGetRelationCount;
        event MasterRowGetRelationNameEventHandler MasterRowGetRelationName;
        event MasterRowGetRelationNameEventHandler MasterRowGetRelationDisplayCaption;
        event MasterRowGetChildListEventHandler MasterRowGetChildList;
        event MasterRowEmptyEventHandler MasterRowEmpty;
        event MasterRowGetLevelDefaultViewEventHandler MasterRowGetLevelDefaultView;
        object GetRow(int rowIndex);
        int GetRelationIndex(int sourceRowHandle, string levelName);
        BaseView GetDetailView(int rowHandle, int relationIndex);
        event MasterRowCanExpandEventHandler MasterRowCollapsing;
    }
    public interface IColumnView : IGridDesignTime, IDataControllerRelationSupport,
                                               IDataControllerValidationSupport, IGridLookUp,
                                               ISummaryItemsOwner, IXtraSupportDeserializeCollection,
                                               IDataControllerVisualClient, IDataControllerSort, IDataControllerData2,
                                               IDataControllerThreadClient, IDataControllerCurrentSupport,
                                               ISupportInitialize, IXtraSerializable, ISkinProvider,
                                               IXtraSerializableLayout, IXtraSerializableLayoutEx,
                                               ISupportXtraSerializer, IServiceProvider, IStringImageProvider, ISupportNewItemRow, IMasterDetailColumnView {

        event EventHandler RestoreCurrentRow;
        int FocusedRowHandle { get; set; }
        bool CanFilterGroupSummaryColumns { get; set; }
        BaseGridController DataController { get; }
        GridOptionsView OptionsView { get; }
        GridColumnCollection Columns { get; }
        string PreviewFieldName { get; }
        GridOptionsBehavior OptionsBehavior { get; }
        GridOptionsSelection OptionsSelection { get; }
        GridOptionsNavigation OptionsNavigation { get; }
        GridOptionsDetail OptionsDetail { get; }
        ColumnViewOptionsFilter OptionsFilter { get; }
        DrawFocusRectStyle FocusRectStyle { get; set; }
        ShowButtonModeEnum ShowButtonMode { get; set; }
        GridOptionsMenu OptionsMenu { get; }
        ErrorMessages ErrorMessages { get; set; }
        bool SkipMakeRowVisible { get; set; }
        GridColumn FocusedColumn { get; set; }
        BaseEdit ActiveEditor { get; }
        bool IsServerMode { get; }
        int SelectedRowsCount { get; }
        bool IsLastColumnFocused { get; }
        GridColumnReadOnlyCollection VisibleColumns { get; }


        GridHitInfo CalcHitInfo(Point location);

        bool IsDataRow(int rowHandle);
        bool IsNewItemRow(int rowHandle);
        int[] GetSelectedRows();

        event InitNewRowEventHandler InitNewRow;
        event EventHandler CancelNewRow;
        event RowAllowEventHandler BeforeLeaveRow;
        event FocusedRowChangedEventHandler FocusedRowChanged;
        event EventHandler ColumnFilterChanged;
        event SelectionChangedEventHandler SelectionChanged;
        event EventHandler ShownEditor;
        event EventHandler HiddenEditor;
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseUp;
        event EventHandler Click;
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseWheel;
        event EventHandler ShowCustomizationForm;
        event EventHandler HideCustomizationForm;
        event RowCellStyleEventHandler RowCellStyle;
        event PopupMenuShowingEventHandler PopupMenuShowing;
        event EventHandler ColumnChanged;
        event RowEventHandler FocusedRowLoaded;
        event EventHandler FilterEditorPopup;
        event EventHandler FilterEditorClosed;
        event CalcPreviewTextEventHandler CalcPreviewText;
        event EventHandler<CreateCustomFilterColumnCollectionEventArgs> CreateCustomFilterColumnCollection;
        event EventHandler<CustomiseFilterFromFilterBuilderEventArgs> CustomiseFilterFromFilterBuilder;
        event ValidateRowEventHandler ValidateRow;
        event CustomRowCellEditEventHandler CustomRowCellEdit;
        string GetRowCellDisplayText(int rowHandle, GridColumn gridColumn);
        event CancelEventHandler ShowingEditor;
        bool IsValidRowHandle(int rowHandle);
        RowVisibleState IsRowVisible(int i);
        void BeginUpdate();
        void EndUpdate();
        bool IsRowLoaded(int contextObject);
        bool UpdateCurrentRow();
        void BeginDataUpdate();
        void CancelCurrentRowEdit();
        void EndDataUpdate();
        void LayoutChanged();
        void Dispose();
        int GetRowHandle(int dataSourceIndex);
        void SetRowExpanded(int focusedRowHandle, bool b, bool b1);
        new int TopRowIndex { get; set; }
        int DataRowCount { get; }
        GridViewAppearances Appearance { get; }
        bool IsLoading { get; }
        int RowCount { get; }
        bool ActiveFilterEnabled { get; set; }
        CriteriaOperator ActiveFilterCriteria { get; set; }
        string ActiveFilterString { get; set; }
        string GetFocusedDisplayText();
        bool PostEditor();
        GridColumn GetVisibleColumn(int i);
        void StartIncrementalSearch(string searchString);
        int GetVisibleIndex(int focusedRowHandle);
        int GetVisibleRowHandle(int i);
    }
    public class CustomGetSelectedObjectsArgs : HandledEventArgs {
        public CustomGetSelectedObjectsArgs(IList list) {
            List = list;
        }

        public IList List { get; set; }
    }
    public class CustomGridCreateEventArgs : HandledEventArgs {
        public GridControl Grid { get; set; }
    }

}
