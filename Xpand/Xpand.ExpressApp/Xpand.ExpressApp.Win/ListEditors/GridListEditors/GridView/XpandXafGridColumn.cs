using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView {
    public interface IXafGridColumn {
        bool AllowSummaryChange { get; set; }
        ColumnsListEditor Editor { get; }
        ITypeInfo TypeInfo { get; }
        IModelColumn Model { get; }
        string PropertyName { get; }
        int SortIndex { get; set; }
        ColumnSortOrder SortOrder { get; set; }
        GridColumnSummaryItemCollection Summary { get; }
        GridSummaryItem SummaryItem { get; }
        int GroupIndex { get; set; }
        ColumnGroupInterval GroupInterval { get; set; }
        OptionsColumn OptionsColumn { get; }
        int VisibleIndex { get; set; }
        string Caption { get; set; }
        string FieldName { get; set; }
        FormatInfo DisplayFormat { get; }
        FormatInfo GroupFormat { get; }
        int Width { get; set; }
        OptionsColumnFilter OptionsFilter { get; }
        ColumnSortMode SortMode { get; set; }
        ColumnFilterMode FilterMode { get; set; }
        RepositoryItem ColumnEdit { get; set; }
        AppearanceObjectEx AppearanceCell { get; }
        string FieldNameSortGroup { get; set; }
        int ImageIndex { get; set; }
        bool Visible { get; set; }
        void Assign(GridColumn gridColumn);
        IXafGridColumn CreateNew(ITypeInfo typeInfo, ColumnsListEditor editor);
        void ApplyModel(IModelColumn columnInfo);
        void SynchronizeModel();
    }

    public class XpandGridColumnWrapper : ColumnWrapper {
        private const int defaultColumnWidth = 75;
        static DefaultBoolean Convert(bool val, DefaultBoolean defaultValue) {
            if (!val)
                return DefaultBoolean.False;
            return defaultValue;
        }
        static bool Convert(DefaultBoolean val) {
            if (val == DefaultBoolean.False)
                return false;
            return true;
        }
        private readonly IXafGridColumn column;
        public XpandGridColumnWrapper(IXafGridColumn column) {
            this.column = column;
        }
        public override bool Visible {
            get {
                return column.Visible;
            }
        }
        public IXafGridColumn Column {
            get {
                return column;
            }
        }
        public override string Id {
            get {
                return column.Model.Id;
            }
        }
        public override string PropertyName {
            get {
                return column.PropertyName;
            }
        }
        public override int SortIndex {
            get {
                return column.SortIndex;
            }
            set {
                column.SortIndex = value;
            }
        }
        public override ColumnSortOrder SortOrder {
            get {
                return column.SortOrder;
            }
            set {
                column.SortOrder = value;
            }
        }
        public override IList<SummaryType> Summary {
            get {
                return (from GridColumnSummaryItem summaryItem in column.Summary select (SummaryType)Enum.Parse(typeof(SummaryType), summaryItem.SummaryType.ToString())).ToList();
            }
            set {
                column.Summary.Clear();
                if (value != null)
                    foreach (SummaryType summaryType in value) {
                        GridColumnSummaryItem summaryItem = column.Summary.Add((SummaryItemType)Enum.Parse(typeof(SummaryItemType), summaryType.ToString()));
                        summaryItem.DisplayFormat = summaryItem.GetDefaultDisplayFormat();
                    }
            }
        }
        public override string SummaryFormat {
            get {
                return column.SummaryItem.DisplayFormat;
            }
            set {
                column.SummaryItem.DisplayFormat = value;
            }
        }
        public override int GroupIndex {
            get {
                return column.GroupIndex;
            }
            set {
                column.GroupIndex = value;
            }
        }
        public override DateTimeGroupInterval GroupInterval {
            get {
                return DateTimeGroupIntervalConverter.Convert(column.GroupInterval);
            }
            set {
                column.GroupInterval = DateTimeGroupIntervalConverter.Convert(value);
            }
        }
        public override bool AllowGroupingChange {
            get {
                return Convert(column.OptionsColumn.AllowGroup);
            }
            set {
                column.OptionsColumn.AllowGroup = Convert(value, column.OptionsColumn.AllowGroup);
            }
        }
        public override bool AllowSortingChange {
            get {
                return Convert(column.OptionsColumn.AllowSort);
            }
            set {
                column.OptionsColumn.AllowSort = Convert(value, column.OptionsColumn.AllowSort);
            }
        }
        public override bool AllowSummaryChange {
            get {
                return column.AllowSummaryChange;
            }
            set {
                column.AllowSummaryChange = value;
            }
        }
        public override int VisibleIndex {
            get {
                return column.VisibleIndex;
            }
            set {
                column.VisibleIndex = value;
            }
        }
        public override string Caption {
            get {
                return column.Caption;
            }
            set {
                column.Caption = value;
                if (string.IsNullOrEmpty(column.Caption))
                    column.Caption = column.FieldName;
            }
        }
        public override string DisplayFormat {
            get {
                return column.DisplayFormat.FormatString;
            }
            set {
                column.DisplayFormat.FormatString = value;
                column.DisplayFormat.FormatType = FormatType.Custom;
                column.GroupFormat.FormatString = value;
                column.GroupFormat.FormatType = FormatType.Custom;
            }
        }
        public override int Width {
            get {
                if (column.Width == defaultColumnWidth)
                    return 0;
                return column.Width;
            }
            set {
                if (value == 0)
                    return;
                column.Width = value;
            }
        }
        public override bool ShowInCustomizationForm {
            get { return column.OptionsColumn.ShowInCustomizationForm; }
            set { column.OptionsColumn.ShowInCustomizationForm = value; }
        }
        public override void DisableFeaturesForProtectedContentColumn() {
            base.DisableFeaturesForProtectedContentColumn();
            column.OptionsFilter.AllowFilter = false;
            column.OptionsFilter.AllowAutoFilter = false;
            column.OptionsColumn.AllowIncrementalSearch = false;
            column.SortMode = ColumnSortMode.DisplayText;
        }
        public override void ApplyModel(IModelColumn columnInfo) {
            base.ApplyModel(columnInfo);
            column.ApplyModel(columnInfo);
        }
        public override void SynchronizeModel() {
            base.SynchronizeModel();
            column.SynchronizeModel();
        }
    }

    public class XpandXafGridColumn : GridColumn, IXafGridColumn {
        readonly ColumnsListEditor _columnsListEditor;

        readonly ITypeInfo _typeInfo;
        IModelColumn _model;

        public XpandXafGridColumn(ITypeInfo typeInfo, ColumnsListEditor columnsListEditor) {
            _columnsListEditor = columnsListEditor;
            _typeInfo = typeInfo;
        }


        public override Type ColumnType {
            get {
                if (string.IsNullOrEmpty(FieldName) || _typeInfo == null)
                    return base.ColumnType;
                IMemberInfo memberInfo = _typeInfo.FindMember(FieldName);
                return memberInfo != null ? memberInfo.MemberType : base.ColumnType;
            }
        }

        public ColumnsListEditor Editor {
            get { return _columnsListEditor; }
        }
        #region IXafGridColumn Members
        public IModelColumn Model {
            get { return _model; }
        }

        public string PropertyName {
            get { return _model != null ? _model.PropertyName : string.Empty; }
        }

        public void ApplyModel(IModelColumn columnInfo) {
            _model = columnInfo;
            CreateModelSynchronizer().ApplyModel();
        }

        public void SynchronizeModel() {
            CreateModelSynchronizer().SynchronizeModel();
        }

        public ITypeInfo TypeInfo {
            get { return _typeInfo; }
        }

        IXafGridColumn IXafGridColumn.CreateNew(ITypeInfo typeInfo, ColumnsListEditor editor) {
            return new XpandXafGridColumn(typeInfo, editor);
        }

        public new void Assign(GridColumn column) {
            base.Assign(column);
        }

        public bool AllowSummaryChange { get; set; }

        ColumnsListEditor IXafGridColumn.Editor {
            get { return Editor; }
        }
        #endregion
        ModelSynchronizer CreateModelSynchronizer() {
            return new ColumnWrapperModelSynchronizer(new XpandGridColumnWrapper(this), _model, _columnsListEditor);
        }
    }
}