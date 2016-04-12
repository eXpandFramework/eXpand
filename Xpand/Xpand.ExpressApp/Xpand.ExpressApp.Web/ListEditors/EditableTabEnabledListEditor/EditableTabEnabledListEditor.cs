using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.Web.ListEditors.EditableTabEnabledListEditor{
    [ListEditor(typeof (object), false)]
    public class EditableTabEnabledListEditor : ListEditor, IComplexListEditor, IProcessCallbackComplete,
        ISupportAppearanceCustomization{
        public delegate void ControlCreatedHandler(string column, ASPxEdit control, XPBaseObject obj);

        private XafApplication _application;

        private Dictionary<ASPxEdit, Tuple<XPBaseObject, string>> _baseObjectsHandledByControl =
            new Dictionary<ASPxEdit, Tuple<XPBaseObject, string>>();

        private ASPxButton _btnAddFirst;
        private ASPxCallbackPanel _callbackPanel;
        private CollectionSourceBase _collectionSource;

        private List<ASPxEdit> _controls = new List<ASPxEdit>();

        private Dictionary<object, Dictionary<string, ASPxEdit>> _controlsPerObjectInList =
            new Dictionary<object, Dictionary<string, ASPxEdit>>();

        private IList _dataSourceList;
        private Dictionary<string, TableCell> _footerCellsPerColumn = new Dictionary<string, TableCell>();
        private List<TableRow> _footerRows = new List<TableRow>();
        private Dictionary<string, TableCell> _headerCellsPerColumn = new Dictionary<string, TableCell>();
        private List<TableRow> _headerRows = new List<TableRow>();
        private IObjectSpace _objectSpace;

        private Dictionary<string, List<XPBaseObject>> _propertiesAlreadyChanged =
            new Dictionary<string, List<XPBaseObject>>();

        private Dictionary<object, List<TableRow>> _rowsPerObject = new Dictionary<object, List<TableRow>>();
        private Table _table;

        public EditableTabEnabledListEditor(IModelListView info) : base(info){
        }

        public string ClientSideBeginCallback{
            get { return _callbackPanel.ClientSideEvents.BeginCallback; }
            set{
                _callbackPanel.ClientSideEvents.BeginCallback = value;
                _callbackPanel.SettingsLoadingPanel.Enabled = string.IsNullOrEmpty(value);
            }
        }

        public override object FocusedObject{
            get { return null; }
            set { }
        }

        public override SelectionType SelectionType{
            get { return SelectionType.None; }
        }

        public override IContextMenuTemplate ContextMenuTemplate{
            get { return null; }
        }

        public void Setup(CollectionSourceBase cs, XafApplication app){
            _application = app;
            _collectionSource = cs;
            _objectSpace = cs.ObjectSpace;
        }

        public void ProcessCallbackComplete(){
            if (AllowEdit)
                ValidateControls();
        }

        public event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;

        public event ControlCreatedHandler ControlCreated;
        public event EventHandler CallBackPerformed;

        public void OnControlCreated(string column, ASPxEdit control, XPBaseObject obj){
            if (ControlCreated != null)
                ControlCreated(column, control, obj);
        }

        public void OnCallbackPerformed(){
            if (CallBackPerformed != null)
                CallBackPerformed(this, EventArgs.Empty);
        }

        protected override object CreateControlsCore(){
            _table = new Table{
                CssClass = "editablelisteditor",
                Width = new Unit(100, UnitType.Percentage),
                CellPadding = 3,
                ID = "EditableTabEnabledListEditor_Table"
            };
            _table.PreRender += control_PreRender;

            _callbackPanel = new ASPxCallbackPanel{
                Width = new Unit(100, UnitType.Percentage),
                ClientInstanceName = "CallbackPanel" + Model.Id
            };
            _callbackPanel.Callback += callbackPanel_Callback;
            _callbackPanel.Controls.Add(_table);
            return _callbackPanel;
        }

        public override void BreakLinksToControls(){
            if (_table != null)
                _table.PreRender -= control_PreRender;
            if (_callbackPanel != null)
                _callbackPanel.Callback -= callbackPanel_Callback;
            foreach (ASPxEdit item in _controls){
                var box = item as ASPxComboBox;
                if (box != null){
                    box.SelectedIndexChanged -= DetailItemControlValueChanged;
                    box.Load -= c_Load;
                }
                else{
                    var checkBox = item as ASPxCheckBox;
                    if (checkBox != null)
                        checkBox.CheckedChanged -= DetailItemControlValueChanged;
                    else{
                        var textBox = item as ASPxTextBox;
                        if (textBox != null)
                            textBox.TextChanged -= DetailItemControlValueChanged;
                        else if (item is ASPxSpinEdit)
                            item.ValueChanged -= DetailItemControlValueChanged;
                        else{
                            var edit = item as ASPxDateEdit;
                            if (edit != null)
                                edit.DateChanged -= DetailItemControlValueChanged;
                        }
                    }
                }
            }

            ClearAllCollectionsOfControls();

            _table = null;
            _callbackPanel = null;

            base.BreakLinksToControls();
        }

        private void callbackPanel_Callback(object sender, CallbackEventArgsBase e){
            if (e.Parameter == "add"){
                var newObject = (XPBaseObject) _objectSpace.CreateObject(Model.ModelClass.TypeInfo.Type);
                _collectionSource.Add(newObject);

                BindDataSource();

                object itemOid = newObject.GetMemberValue(Model.ModelClass.KeyProperty); // newObject.Oid;
                try{
                    var firstOrDefault = Model.Columns.Where(t => t.Index.HasValue && t.Index.Value >= 0)
                        .OrderBy(t => t.Index.Value)
                        .FirstOrDefault();
                    if (firstOrDefault != null)
                        _controlsPerObjectInList[itemOid][
                            firstOrDefault
                                .PropertyName].Focus();
                }
                catch{
                }
            }
            else if (e.Parameter != null && e.Parameter.StartsWith("changed_")){
                string[] split = e.Parameter.Split('_');
                string itemOid = split[2];
                string propertyName = split[1];
                BindDataSource();
                try{
                    object key = _controlsPerObjectInList.Keys.FirstOrDefault(item => item.ToString() == itemOid);

                    if (key != null && _controlsPerObjectInList.ContainsKey(key)) _controlsPerObjectInList[key][propertyName].Focus();
                }
                catch{
                }
            }
            else if (e.Parameter != null && e.Parameter.StartsWith("rem_")){
                string itemOid = e.Parameter.Replace("rem_", "");
                if (_dataSourceList == null)
                    return;
                XPBaseObject itemToBeDeleted = _dataSourceList.Cast<XPBaseObject>().FirstOrDefault(item => item.GetMemberValue(Model.ModelClass.KeyProperty).ToString() == itemOid);
                if (itemToBeDeleted != null){
                    _collectionSource.Remove(itemToBeDeleted);
                    itemToBeDeleted.Delete();
                }
                if (_collectionSource.GetCount() == 0 && _btnAddFirst != null)
                    _btnAddFirst.Visible = true;

                BindDataSource();

                FocusCorrectControl();
            }
            ToggleHeadersVisibility();

            OnCallbackPerformed();
        }

        private void control_PreRender(object sender, EventArgs e){
            ToggleHeadersVisibility();

            if (AllowEdit)
                ValidateControls();
        }

        private void ToggleHeadersVisibility(){
            if (HideHeadersWhenNoDataWhenHeadersOnTopOnly && HeadersOnTopOnly)
                _headerRows.ForEach(t => t.Visible = _dataSourceList.Count > 0);
            if (HideHeadersWhenNoDataWhenHeadersOnTopOnly && HeadersOnTopOnly)
                _footerRows.ForEach(t => t.Visible = _dataSourceList.Count > 0);
        }

        public void ValidateControls(){
            if (_dataSourceList == null)
                return;

            foreach (XPBaseObject item in _dataSourceList){
                foreach (string prop in _controlsPerObjectInList[item.GetMemberValue(Model.ModelClass.KeyProperty)].Keys){
                    ErrorMessage error = ErrorMessages.GetMessage(prop, item);
                    if (error != null){
                        _controlsPerObjectInList[item.GetMemberValue(Model.ModelClass.KeyProperty)][prop].IsValid = false;
                        _controlsPerObjectInList[item.GetMemberValue(Model.ModelClass.KeyProperty)][prop].ErrorText =
                            error.Message;
                    }
                    else{
                        _controlsPerObjectInList[item.GetMemberValue(Model.ModelClass.KeyProperty)][prop].IsValid = true;
                        _controlsPerObjectInList[item.GetMemberValue(Model.ModelClass.KeyProperty)][prop].ErrorText = "";
                    }
                }
            }
        }

        protected override void AssignDataSourceToControl(object dataSource){
            if (_table != null){
                ClearAllCollectionsOfControls();
                _table.Rows.Clear();

                var tr = new TableRow();
                _table.Rows.Add(tr);
                var tc = new TableCell();
                tr.Cells.Add(tc);

                if (AllowEdit && ShowButtons){
                    _btnAddFirst = new ASPxButton{AutoPostBack = false, EnableClientSideAPI = true};

                    _btnAddFirst.SetClientSideEventHandler("Click",
                        "function(s, e) { " + "CallbackPanel" + Model.Id + ".PerformCallback(\"add\"); }");

                    _btnAddFirst.Text = AddButtonCaption;
                    _btnAddFirst.ID = "btnAddFirst";
                    _btnAddFirst.Visible = true;
                    tc.Controls.Add(_btnAddFirst);
                }

                _dataSourceList = ListHelper.GetList(dataSource);

                if (_dataSourceList == null)
                    return;

                if (HeadersOnTopOnly)
                    CreateHeaderRows(null).ForEach(hr => _table.Rows.Add(hr));
                if (_dataSourceList.Count > 0){
                    int rowindex = 0;
                    foreach (XPBaseObject item in _dataSourceList){
                        CreateRowForObject(item, false, rowindex);
                        rowindex++;
                    }
                }

                CreateFooterRow();
                ToggleHeadersVisibility();

                foreach (IModelColumn column in Model.Columns){
                    if (column.Index >= 0){
                        if (
                            _controlsPerObjectInList.Values.ToList()
                                .TrueForAll(
                                    d => d.ContainsKey(column.PropertyName) && !d[column.PropertyName].Visible)){
                            if (_headerCellsPerColumn.ContainsKey(column.PropertyName))
                                _headerCellsPerColumn[column.PropertyName].Visible = false;
                            if (_footerCellsPerColumn.ContainsKey(column.PropertyName))
                                _footerCellsPerColumn[column.PropertyName].Visible = false;
                        }
                    }
                }
            }
        }

        private void CreateFooterRow(){
            var hrows = new List<TableRow>();
            int i = 0;
            TableRow thr = null;
            foreach (IModelColumn column in Model.Columns){
                if (column.Index >= 0){
                    if (i%ColumnsPerRow == 0){
                        thr = new TableRow{CssClass = "footerrow"};
                        hrows.Add(thr);
                    }
                    i++;

                    var thc = new TableCell{CssClass = "Caption"};
                    if (thr != null) thr.Cells.Add(thc);

                    _footerCellsPerColumn.Add(column.PropertyName, thc);

                    var summary = (IModelColumnSummaryItemEditabledTabEnabled)column.Summary.FirstOrDefault();
                    if (summary == null)
                        thc.Text = "";
                    else{
                        //TODO: implement all summary types
                        switch (summary.SummaryType){
                            case SummaryType.Average:
                                break;
                            case SummaryType.Count:
                                break;
                            case SummaryType.Custom:
                                break;
                            case SummaryType.Max:
                                break;
                            case SummaryType.Min:
                                break;
                            case SummaryType.None:
                                break;
                            case SummaryType.Sum:
                                decimal sum = _dataSourceList.Cast<XPBaseObject>().Sum(item => Convert.ToDecimal(item.GetMemberValue(column.PropertyName)));
                                if (!string.IsNullOrEmpty(column.DisplayFormat))
                                    thc.Text = summary.EditableTabEnabledEditorSummaryItem.SummaryCaption +
                                               string.Format(column.DisplayFormat, sum);
                                else if (column.ModelMember.Type == typeof (decimal))
                                    thc.Text = summary.EditableTabEnabledEditorSummaryItem.SummaryCaption + sum.ToString("C2");
                                else
                                    thc.Text = summary.EditableTabEnabledEditorSummaryItem.SummaryCaption + sum.ToString("F0");
                                break;
                        }
                    }
                }
            }
            if (thr == null){
                thr = new TableRow{CssClass = "footerrow"};
            }

            if (i%ColumnsPerRow != 0){
                var thcButtons = new TableCell();
                thr.Cells.Add(thcButtons);
                thcButtons.ColumnSpan = ColumnsPerRow - i%ColumnsPerRow;
            }

            hrows.Add(thr);

            foreach (TableRow row in hrows){
                _table.Rows.Add(row);
            }
            _footerRows.AddRange(hrows);
        }

        private void ClearAllCollectionsOfControls(){
            _controls = new List<ASPxEdit>();
            _controlsPerObjectInList = new Dictionary<object, Dictionary<string, ASPxEdit>>();
            _baseObjectsHandledByControl = new Dictionary<ASPxEdit, Tuple<XPBaseObject, string>>();
            _rowsPerObject = new Dictionary<object, List<TableRow>>();
            _headerRows = new List<TableRow>();
            _footerRows = new List<TableRow>();
            _propertiesAlreadyChanged = new Dictionary<string, List<XPBaseObject>>();
            _headerCellsPerColumn = new Dictionary<string, TableCell>();
            _footerCellsPerColumn = new Dictionary<string, TableCell>();
        }

        private void CreateRowForObject(XPBaseObject item, bool focusFirstControl, int rowindex){
            _rowsPerObject[item.GetMemberValue(Model.ModelClass.KeyProperty)] = new List<TableRow>();
            var headerRowsForItem = new List<TableRow>();
            if (!HeadersOnTopOnly){
                headerRowsForItem = CreateHeaderRows(item, rowindex);
                _rowsPerObject[item.GetMemberValue(Model.ModelClass.KeyProperty)].AddRange(headerRowsForItem);
            }

            if (_btnAddFirst != null)
                _btnAddFirst.Visible = false;

            TableRow tr = null;

            var controlsOfRowPerColumn = new Dictionary<string, ASPxEdit>();

            int i = 0;
            int headerrowsadded = 0;
            bool firstControlFocused = false;
            foreach (IModelColumn column in Model.Columns){
                if (column.Index >= 0){
                    if (i%ColumnsPerRow == 0){
                        if (headerRowsForItem.Count > headerrowsadded){
                            _table.Rows.Add(headerRowsForItem[headerrowsadded]);
                            headerrowsadded++;
                        }
                        tr = new TableRow{CssClass = rowindex%2 == 0 ? "evenrow" : "oddrow"};
                        _table.Rows.Add(tr);
                        _rowsPerObject[item.GetMemberValue(Model.ModelClass.KeyProperty)].Add(tr);
                    }
                    i++;

                    var tc = new TableCell();
                    if (tr != null) tr.Cells.Add(tc);

                    if (!AllowEdit){
                        object itemGetMemberValue = item.GetMemberValue(column.PropertyName);
                        if (itemGetMemberValue == null)
                            tc.Text = "";
                        else if (itemGetMemberValue is XPBaseObject){
                            ITypeInfo typeInfo =
                                XafTypesInfo.Instance.FindTypeInfo(
                                    (itemGetMemberValue as XPBaseObject).ClassInfo.ClassType);
                            if (typeInfo.DefaultMember != null){
                                object defMembVal =
                                    (itemGetMemberValue as XPBaseObject).GetMemberValue(typeInfo.DefaultMember.Name);
                                tc.Text = defMembVal != null ? defMembVal.ToString() : "";
                            }
                            else
                                tc.Text = itemGetMemberValue.ToString();
                        }
                        else if (itemGetMemberValue is Enum){
                            {
                                string caption =
                                    new EnumDescriptor(column.ModelMember.MemberInfo.MemberType).GetCaption(
                                        itemGetMemberValue);
                                tc.Text = string.IsNullOrEmpty(column.DisplayFormat)
                                    ? caption
                                    : string.Format(column.DisplayFormat, caption);
                            }
                        }
                        else{
                            if (!string.IsNullOrEmpty(column.DisplayFormat)){
                                tc.Text = string.Format(column.DisplayFormat, itemGetMemberValue);
                            }
                            else if (itemGetMemberValue is decimal){
                                tc.Text = string.Format("{0:C2}", itemGetMemberValue);
                            }
                            else
                                tc.Text = itemGetMemberValue.ToString();
                        }

                        tc.CssClass = "StaticText";
                    }
                    else{
                        ASPxEdit cellControl = GetControlForColumn(column, item);
                        if (!column.AllowEdit)
                            cellControl.Enabled = false;

                        //handle control width
                        int tableColumns = Math.Max(Model.Columns.Count(c => c.Index >= 0), ColumnsPerRow);
                        if (cellControl is ASPxCheckBox){
                            tc.Width = new Unit(1, UnitType.Percentage);
                        }
                        else if (tableColumns>0){
                            var value = 100/tableColumns;
                            tc.Width = new Unit(value, UnitType.Percentage);
                        }

                        cellControl.ID = column.PropertyName + "_control_" +
                                         item.GetMemberValue(Model.ModelClass.KeyProperty)
                                             .ToString()
                                             .Replace("-", "_minus_");
                        cellControl.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
                        cellControl.ValidationSettings.Display = Display.Dynamic;
                        cellControl.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Left;
                        cellControl.ValidationSettings.ErrorImage.Url =
                            ImageLoader.Instance.GetImageInfo("ximage").ImageUrl;

                        controlsOfRowPerColumn.Add(column.PropertyName, cellControl);
                        _baseObjectsHandledByControl[cellControl] = new Tuple<XPBaseObject, string>(item,
                            column.PropertyName);

                        tc.Controls.Add(cellControl);

                        if (focusFirstControl && !firstControlFocused){
                            cellControl.Focus();
                            firstControlFocused = true;
                        }
                    }
                    OnCustomizeAppearance(new CustomizeAppearanceEventArgs(column.PropertyName,
                        new WebControlAppearanceAdapter(tc, tc), item));
                }
            }

            if (tr == null){
                tr = new TableRow{CssClass = rowindex%2 == 0 ? "evenrow" : "oddrow"};
                _table.Rows.Add(tr);
                _rowsPerObject[item.GetMemberValue(Model.ModelClass.KeyProperty)].Add(tr);
            }

            var tcButtons = new TableCell{Wrap = false, Width = new Unit(1, UnitType.Percentage)};

            if (i%ColumnsPerRow != 0){
                tcButtons.ColumnSpan = ColumnsPerRow - i%ColumnsPerRow;
            }
            else{
                tr = new TableRow{CssClass = rowindex%2 == 0 ? "evenrow" : "oddrow"};
                _table.Rows.Add(tr);
                _rowsPerObject[item.GetMemberValue(Model.ModelClass.KeyProperty)].Add(tr);
                //a new row will be created and the whole row will be the buttons
                tcButtons.ColumnSpan = ColumnsPerRow;
            }

            tr.Cells.Add(tcButtons);

            var tblButtons = new Table();
            tcButtons.Controls.Add(tblButtons);
            var btnsrow = new TableRow();
            tblButtons.Rows.Add(btnsrow);

            if (AllowEdit && ShowButtons){
                var btnRemove = new ASPxButton{AutoPostBack = false, EnableClientSideAPI = true};

                btnRemove.SetClientSideEventHandler("Click",
                    "function(s, e) { " + "CallbackPanel" + Model.Id + ".PerformCallback(\"rem_" +
                    item.GetMemberValue(Model.ModelClass.KeyProperty) + "\"); }");
                btnRemove.Text = DeleteButtonCaption;
                btnRemove.ID = "btnRemove_" + item.GetMemberValue(Model.ModelClass.KeyProperty);

                var t = new TableCell();
                btnsrow.Cells.Add(t);
                t.Controls.Add(btnRemove);
            }

            if (AllowEdit && ShowButtons){
                var btnAdd = new ASPxButton{AutoPostBack = false, EnableClientSideAPI = true};

                btnAdd.SetClientSideEventHandler("Click",
                    "function(s, e) { " + "CallbackPanel" + Model.Id + ".PerformCallback(\"add\"); }");
                btnAdd.Text = AddButtonCaption;
                btnAdd.ID = "btnAdd_" + item.GetMemberValue(Model.ModelClass.KeyProperty);

                var t = new TableCell();
                btnsrow.Cells.Add(t);
                t.Controls.Add(btnAdd);
            }
            _controlsPerObjectInList[item.GetMemberValue(Model.ModelClass.KeyProperty)] = controlsOfRowPerColumn;
        }


        private List<TableRow> CreateHeaderRows(XPBaseObject forItem, int rowindex = -1){
            var hrows = new List<TableRow>();
            int i = 0;
            TableRow thr = null;
            foreach (IModelColumn column in Model.Columns){
                if (column.Index >= 0){
                    if (i%ColumnsPerRow == 0){
                        thr = new TableRow();
                        if (rowindex < 0)
                            thr.CssClass = "headerrow";
                        else
                            thr.CssClass = rowindex%2 == 0 ? "evenrow" : "oddrow";
                        hrows.Add(thr);
                    }
                    i++;

                    var thc = new TableCell{CssClass = "Caption"};
                    if (thr != null) thr.Cells.Add(thc);

                    thc.Text = column.Caption;

                    if (forItem == null)
                        _headerCellsPerColumn.Add(column.PropertyName, thc);
                    else
                        OnCustomizeAppearance(new CustomizeAppearanceEventArgs(column.PropertyName,
                            new WebControlAppearanceAdapter(thc, thc), forItem));
                }
            }
            if (thr == null){
                thr = new TableRow();
                if (rowindex < 0)
                    thr.CssClass = "headerrow";
                else
                    thr.CssClass = rowindex%2 == 0 ? "evenrow" : "oddrow";
            }

            if (i%ColumnsPerRow != 0){
                var thcButtons = new TableCell();
                thr.Cells.Add(thcButtons);
                thcButtons.ColumnSpan = ColumnsPerRow - i%ColumnsPerRow;
            }

            hrows.Add(thr);
            _headerRows.AddRange(hrows);
            return hrows;
        }


        private void FocusCorrectControl(){
            if (_collectionSource.GetCount() == 0 && _btnAddFirst != null){
                _btnAddFirst.Focus();
            }
            else{
                int i = 0;
                foreach (TableRow row in _table.Rows){
                    i++;
                    if (i == 1 || i == 2) //skip header line and the line after (the one with the add new button on it)
                        continue;

                    if (row.Visible && row.Cells.Count > 0 && row.Cells[0].Controls.Count > 0){
                        row.Cells[0].Controls[0].Focus();
                        break;
                    }
                }
            }
        }

        private ASPxEdit GetControlForColumn(IModelColumn column, XPBaseObject item){
            object value = item.GetMemberValue(column.PropertyName);
            ASPxEdit c;
            if (typeof (XPBaseObject).IsAssignableFrom(column.ModelMember.MemberInfo.MemberType)){
                c = RenderHelper.CreateASPxComboBox();
                var helper = new WebLookupEditorHelper(_application, _objectSpace,
                    column.ModelMember.MemberInfo.MemberTypeInfo, column);

                ((ASPxComboBox) c).ClientSideEvents.KeyUp =
                    "function(s, e) { if(e.htmlEvent.keyCode == 46){ s.SetSelectedIndex(-1); } }";
                ((ASPxComboBox) c).ValueType = column.ModelMember.MemberInfo.MemberTypeInfo.KeyMember.MemberType;
                ((ASPxComboBox) c).SelectedIndexChanged += DetailItemControlValueChanged;
                c.Style.Add("min-width", "120px");
                c.Width = new Unit(100, UnitType.Percentage);

                FillEditorValues(value, ((ASPxComboBox) c), helper, item, column.ModelMember);
                if (column.ModelMember.MemberInfo.FindAttribute<ImmediatePostDataAttribute>() != null)
                    ((ASPxComboBox) c).ClientSideEvents.SelectedIndexChanged = "function(s, e) { " + "CallbackPanel" +
                                                                               Model.Id + ".PerformCallback(\"changed_" +
                                                                               column.PropertyName + "_" +
                                                                               item.GetMemberValue(
                                                                                   Model.ModelClass.KeyProperty) +
                                                                               "\"); }";
            }
            else if (typeof (Enum).IsAssignableFrom(column.ModelMember.MemberInfo.MemberType)){
                c = RenderHelper.CreateASPxComboBox();
                ((ASPxComboBox) c).ClientSideEvents.KeyUp =
                    "function(s, e) { if(e.htmlEvent.keyCode == 46){ s.SetSelectedIndex(-1); } }";
                var descriptor = new EnumDescriptor(column.ModelMember.MemberInfo.MemberType);
                var source = (Enum.GetValues(column.ModelMember.MemberInfo.MemberType).Cast<object>()
                    .Select(v => new Tuple<object, string>(v, descriptor.GetCaption(v)))).ToList();
                c.DataSource = source;
                ((ASPxComboBox) c).ValueField = "Item1";
                ((ASPxComboBox) c).TextField = "Item2";
                ((ASPxComboBox) c).ValueType = column.ModelMember.MemberInfo.MemberType;
                ((ASPxComboBox) c).SelectedIndexChanged += DetailItemControlValueChanged;
                c.Style.Add("min-width", "120px");
                c.Width = new Unit(100, UnitType.Percentage);
                c.Load += c_Load;
                if (column.ModelMember.MemberInfo.FindAttribute<ImmediatePostDataAttribute>() != null)
                    ((ASPxComboBox) c).ClientSideEvents.SelectedIndexChanged = "function(s, e) { " + "CallbackPanel" +
                                                                               Model.Id + ".PerformCallback(\"changed_" +
                                                                               column.PropertyName + "_" +
                                                                               item.GetMemberValue(
                                                                                   Model.ModelClass.KeyProperty) +
                                                                               "\"); }";
            }
            else{
                switch (column.ModelMember.MemberInfo.MemberType.ToString()){
                    case "System.Boolean":
                    case "System.bool":
                        c = RenderHelper.CreateASPxCheckBox();
                        ((ASPxCheckBox) c).CheckedChanged += DetailItemControlValueChanged;
                        c.Style.Add("max-width", "20px");
                        if (column.ModelMember.MemberInfo.FindAttribute<ImmediatePostDataAttribute>() != null)
                            ((ASPxCheckBox) c).ClientSideEvents.CheckedChanged = "function(s, e) { " + "CallbackPanel" +
                                                                                 Model.Id +
                                                                                 ".PerformCallback(\"changed_" +
                                                                                 column.PropertyName + "_" +
                                                                                 item.GetMemberValue(
                                                                                     Model.ModelClass.KeyProperty) +
                                                                                 "\"); }";
                        break;
                    case "System.String":
                    case "System.string":
                        c = RenderHelper.CreateASPxTextBox();
                        ((ASPxTextBox) c).MaxLength = 100;
                        if (column.ModelMember.Size > 0)
                            ((ASPxTextBox) c).MaxLength = column.ModelMember.Size;
                        ((ASPxTextBox) c).TextChanged += DetailItemControlValueChanged;
                        if (column.ModelMember.MemberInfo.FindAttribute<ImmediatePostDataAttribute>() != null)
                            ((ASPxTextBox) c).ClientSideEvents.TextChanged = "function(s, e) { " + "CallbackPanel" +
                                                                             Model.Id + ".PerformCallback(\"changed_" +
                                                                             column.PropertyName + "_" +
                                                                             item.GetMemberValue(
                                                                                 Model.ModelClass.KeyProperty) +
                                                                             "\"); }";
                        c.Style.Add("min-width", "130px");
                        break;
                    case "System.Int32":
                    case "System.int":
                    case "System.Int64":
                    case "System.long":
                        c = RenderHelper.CreateASPxSpinEdit();
                        ((ASPxSpinEdit) c).NumberType = SpinEditNumberType.Integer;
                        ((ASPxSpinEdit) c).DecimalPlaces = 0;
                        c.ValueChanged += DetailItemControlValueChanged;
                        if (column.ModelMember.MemberInfo.FindAttribute<ImmediatePostDataAttribute>() != null)
                            ((ASPxSpinEdit) c).ClientSideEvents.NumberChanged = "function(s, e) { " + "CallbackPanel" +
                                                                                Model.Id + ".PerformCallback(\"changed_" +
                                                                                column.PropertyName + "_" +
                                                                                item.GetMemberValue(
                                                                                    Model.ModelClass.KeyProperty) +
                                                                                "\"); }";
                        c.Style.Add("min-width", "100px");
                        break;
                    case "System.Double":
                    case "System.double":
                    case "System.Decimal":
                    case "System.decimal":
                    case "System.Nullable`1[System.Decimal]":
                        c = RenderHelper.CreateASPxSpinEdit();
                        ((ASPxSpinEdit) c).NumberType = SpinEditNumberType.Float;
                        string format = column.ModelMember.DisplayFormat;
                        if (format == "{0:C}") format = null;
                        ((ASPxSpinEdit) c).DisplayFormatString = format;
                        if (string.IsNullOrEmpty(format))
                            ((ASPxSpinEdit) c).DecimalPlaces = 2;
                        c.ValueChanged += DetailItemControlValueChanged;
                        if (column.ModelMember.MemberInfo.FindAttribute<ImmediatePostDataAttribute>() != null)
                            ((ASPxSpinEdit) c).ClientSideEvents.NumberChanged = "function(s, e) { " + "CallbackPanel" +
                                                                                Model.Id + ".PerformCallback(\"changed_" +
                                                                                column.PropertyName + "_" +
                                                                                item.GetMemberValue(
                                                                                    Model.ModelClass.KeyProperty) +
                                                                                "\"); }";
                        c.Style.Add("min-width", "100px");
                        break;
                    case "System.DateTime":
                        c = RenderHelper.CreateASPxDateEdit();
                        ((ASPxDateEdit) c).DateChanged += DetailItemControlValueChanged;
                        if (column.ModelMember.MemberInfo.FindAttribute<ImmediatePostDataAttribute>() != null)
                            ((ASPxDateEdit) c).ClientSideEvents.DateChanged = "function(s, e) { " + "CallbackPanel" +
                                                                              Model.Id + ".PerformCallback(\"changed_" +
                                                                              column.PropertyName + "_" +
                                                                              item.GetMemberValue(
                                                                                  Model.ModelClass.KeyProperty) +
                                                                              "\"); }";
                        c.Style.Add("min-width", "90px");

                        break;
                    default:
                        c = RenderHelper.CreateASPxTextBox();
                        ((ASPxTextBox) c).MaxLength = 100;
                        ((ASPxTextBox) c).TextChanged += DetailItemControlValueChanged;
                        if (column.ModelMember.MemberInfo.FindAttribute<ImmediatePostDataAttribute>() != null)
                            ((ASPxTextBox) c).ClientSideEvents.TextChanged = "function(s, e) { " + "CallbackPanel" +
                                                                             Model.Id + ".PerformCallback(\"changed_" +
                                                                             column.PropertyName + "_" +
                                                                             item.GetMemberValue(
                                                                                 Model.ModelClass.KeyProperty) +
                                                                             "\"); }";
                        c.Style.Add("min-width", "130px");
                        break;
                }
                c.Width = new Unit(100, UnitType.Percentage);
            }

            SetValueToControl(value, c);
            _controls.Add(c);

            OnControlCreated(column.PropertyName, c, item);

            return c;
        }

        private void c_Load(object sender, EventArgs e){
            ((ASPxComboBox) sender).DataBind();
            if (((ASPxComboBox) sender).Items.FindByValue(((ASPxComboBox) sender).Value) == null){
                ((ASPxComboBox) sender).Value = null;
                ((ASPxComboBox) sender).Text = "";
            }
        }


        private static void SetValueToControl(object value, ASPxEdit c){
            c.Value = value;
        }

        private void DetailItemControlValueChanged(object sender, EventArgs e){
            Tuple<XPBaseObject, string> tuple = _baseObjectsHandledByControl[(ASPxEdit) sender ];
            if (!_propertiesAlreadyChanged.ContainsKey(tuple.Item2) ||
                !_propertiesAlreadyChanged[tuple.Item2].Contains(tuple.Item1)){
                tuple.Item1.Changed += Item1_Changed;
                _objectSpace.SetModified(tuple.Item1,
                    Model.Columns.First(c => c.PropertyName == tuple.Item2).ModelMember.MemberInfo);
                DoSave(tuple.Item1.GetMemberValue(Model.ModelClass.KeyProperty), tuple.Item2);
                tuple.Item1.Changed -= Item1_Changed;
            }
        }

        private void Item1_Changed(object sender, ObjectChangeEventArgs e){
            if (!_propertiesAlreadyChanged.ContainsKey(e.PropertyName))
                _propertiesAlreadyChanged[e.PropertyName] = new List<XPBaseObject>();
            if (!_propertiesAlreadyChanged[e.PropertyName].Contains(sender as XPBaseObject))
                _propertiesAlreadyChanged[e.PropertyName].Add(sender as XPBaseObject);
        }


        private void FillEditorValues(object currentSelectedObject, ASPxComboBox control, WebLookupEditorHelper helper,
            object currentObject, IModelMember member){
            control.Items.Clear();
            control.Items.Add(WebPropertyEditor.EmptyValue, null);
            control.SelectedIndex = 0;
            if (currentObject != null){
                CollectionSourceBase dataSource = helper.CreateCollectionSource(currentObject);
                if (dataSource != null){
                    helper.ReloadCollectionSource(dataSource, currentObject);
                }
                else{
                    throw new NullReferenceException("currentSelectedObject");
                }
                var list = new ArrayList();
                if (dataSource.List != null){
                    foreach (object t in dataSource.List){
                        list.Add(t);
                    }
                }
                else{
                    if (currentSelectedObject != null){
                        control.Items.Add(
                            helper.GetEscapedDisplayText(currentSelectedObject, WebPropertyEditor.EmptyValue,
                                member.DisplayFormat), -1);
                        control.SelectedIndex = 1;
                    }
                    return;
                }
                if (currentSelectedObject != null && (list.IndexOf(currentSelectedObject) == -1)){
                    list.Add(currentSelectedObject);
                }
                list.Sort(new DisplayValueComparer(helper, WebPropertyEditor.EmptyValue));
                foreach (object obj in list){
                    control.Items.Add(
                        helper.GetEscapedDisplayText(obj, WebPropertyEditor.EmptyValue, member.DisplayFormat),
                        ((XPBaseObject) obj).GetMemberValue(helper.LookupObjectTypeInfo.KeyMember.Name));
                        // helper.GetObjectKey(obj));
                    if (currentSelectedObject == obj){
                        if (obj != null) control.SelectedIndex = list.IndexOf(obj) + 1;
                    }
                }
            }
        }

        public override void Refresh(){
        }

        public override IList GetSelectedObjects(){
            var selectedObjects = new List<object>();
            return selectedObjects;
        }

        public override void SaveModel(){
        }

        public void BindDataSource(){
            if (_table != null){
                _table.Rows.Clear(); //clear rows in order to recreate the control 
            }
            AssignDataSourceToControl(_collectionSource.Collection);
        }

        private void SetMemberValueOfItem(XPBaseObject item, IModelColumn column, ASPxEdit cellControl){
            if (cellControl is ASPxSpinEdit && ((ASPxSpinEdit) cellControl).NumberType == SpinEditNumberType.Integer)
                item.SetMemberValue(column.PropertyName,
                    column.ModelMember.MemberInfo.MemberType == typeof (int)
                        ? Convert.ToInt32(cellControl.Value)
                        : Convert.ToInt64(cellControl.Value));
            else{
                if (typeof (XPBaseObject).IsAssignableFrom(column.ModelMember.MemberInfo.MemberType) &&
                    !typeof (IFileData).IsAssignableFrom(column.ModelMember.MemberInfo.MemberType)){
                    object o = _objectSpace.GetObjectByKey(column.ModelMember.MemberInfo.MemberType, cellControl.Value);
                    item.SetMemberValue(column.PropertyName, o);
                }
                else{
                    item.SetMemberValue(column.PropertyName, cellControl.Value);
                }
            }
        }

        private void DoSave(object oid, string propertyName){
            XPBaseObject currentObject = _dataSourceList.Cast<XPBaseObject>().FirstOrDefault(item => item.GetMemberValue(Model.ModelClass.KeyProperty).Equals(oid));

            if (currentObject == null || !_controlsPerObjectInList.ContainsKey(oid)) return;

            ASPxEdit cellControl = _controlsPerObjectInList[oid][propertyName];
            SetMemberValueOfItem(currentObject, Model.Columns.FirstOrDefault(c => c.PropertyName == propertyName),
                cellControl);
        }


        protected virtual void OnCustomizeAppearance(CustomizeAppearanceEventArgs args){
            if (CustomizeAppearance != null){
                CustomizeAppearance(this, args);
            }
        }

        #region Configuration

        private const string AddButtonCaption = "New";
        private const string DeleteButtonCaption = "Delete";

        public override bool SupportsDataAccessMode(CollectionSourceDataAccessMode dataAccessMode){
            return false;
        }

        private int ColumnsPerRow{
            get { return ((IModelListViewEditableTabEnabledEditor)Model).EditableTabEnabledEditor.ColumnsPerRow; }
        }


        private bool HeadersOnTopOnly{
            get{ return ((IModelListViewEditableTabEnabledEditor) Model).EditableTabEnabledEditor.HeadersOnTopOnly;}
        }

        private bool HideHeadersWhenNoDataWhenHeadersOnTopOnly{
            get { return true; }
        }

        private bool ShowButtons{
            get { return ((IModelListViewEditableTabEnabledEditor)Model).EditableTabEnabledEditor.ShowButtons; }
        }

        #endregion
    }

    internal class DisplayValueComparer : IComparer{
        private readonly string _emptyValue;
        private readonly LookupEditorHelper _helper;

        public DisplayValueComparer(LookupEditorHelper helper, string emptyValue){
            _emptyValue = emptyValue;
            _helper = helper;
        }

        #region IComparer Members

        public int Compare(object x, object y){
            return String.Compare(_helper.GetDisplayText(x, _emptyValue, null), _helper.GetDisplayText(y, _emptyValue, null), StringComparison.Ordinal);
        }

        #endregion
    }
}