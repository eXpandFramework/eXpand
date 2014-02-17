using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Web.ASPxCallbackPanel;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxEditors;
using DevExpress.Xpo;
using Xpand.ExpressApp.Web.ListEditors.EditableTabEnabledListEditor;

namespace Xpand.ExpressApp.Web.ListEditors.TwoDimensionListEditor {
    public delegate Color ColorPattern(object item);
    public interface ITwoDimensionItem {
        IComparable HorizontalDimension { get; }
        IComparable VerticalDimension { get; }
    }

    [ListEditor(typeof(ITwoDimensionItem), true)]
    public class TwoDimensionListEditor : ListEditor, IComplexListEditor, IProcessCallbackComplete, IXafCallbackHandler,
        ISupportAppearanceCustomization {
        private readonly string _uniqueId = "TimeTableListEditor_CallbackHandlerId";
        private readonly Dictionary<ASPxEditBase, string> _buttonConfirmations = new Dictionary<ASPxEditBase, string>();
        private readonly Dictionary<ASPxEditBase, string> _buttonScripts = new Dictionary<ASPxEditBase, string>();
        private readonly Dictionary<ASPxCheckBox, object> _checkboxObjectIds = new Dictionary<ASPxCheckBox, object>();
        private readonly Dictionary<ASPxEditBase, string> _checkboxScripts = new Dictionary<ASPxEditBase, string>();
        private XafCallbackManager _callbackManager;
        private ASPxCallbackPanel _callbackPanel;
        private TimeTableContextMenuTemplate _contextMenu;
        private IList _dataSourceList;
        private List<object> _selectedObjects = new List<object>();
        private Table _table;

        #region Configuration

        //public ColorPattern ColorPattern { get; set; }

        public ColorPattern HorizontalDimensionColorPattern { get; set; }

        public ColorPattern VerticalDimensionColorPattern { get; set; }

        public List<IComparable> DefaultHorizontalDimensions { get; set; }

        public List<IComparable> DefaultVerticalDimensions { get; set; }

        public override bool IsServerModeSupported {
            get { return false; }
        }

        public bool ViewMode { get; set; }

        internal bool ShowCheckboxes {
            get{
                return !ViewMode && ((IModelListViewTwoDimensionListEditor) Model).TwoDimensionListEditor.ShowCheckboxes;
            }
        }

        internal bool FastMode {
            get {
                return !ViewMode && ((IModelListViewTwoDimensionListEditor)Model).TwoDimensionListEditor.FastMode;
            }
        }

        #endregion

        public TwoDimensionListEditor(IModelListView info)
            : base(info) {
            _contextMenu = new TimeTableContextMenuTemplate(this);
            _uniqueId = "TimeTableListEditor_CallbackHandlerId" + Guid.NewGuid();
            DefaultHorizontalDimensions = new List<IComparable>();
            DefaultVerticalDimensions = new List<IComparable>();
        }

        public override object FocusedObject { get; set; }

        public override SelectionType SelectionType {
            get {
                if (ShowCheckboxes)
                    return SelectionType.MultipleSelection;
                return SelectionType.TemporarySelection;
            }
        }

        public override IContextMenuTemplate ContextMenuTemplate {
            get { return _contextMenu; }
        }

        public void Setup(CollectionSourceBase cs, XafApplication app) {
        }

        public void ProcessCallbackComplete() {
            BindDataSource();
        }

        public event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;

        public void ProcessAction(string parameter) {
            bool mustRenderControlAgain = FastMode;
            string[] parameters = parameter.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            string actionId = parameters[1];
            if (ProcessActions(actionId, parameters, ref mustRenderControlAgain)) return;
            if (mustRenderControlAgain)
                RenderControl();
        }

        private bool ProcessActions(string actionId, string[] parameters, ref bool mustRenderControlAgain) {
            if (actionId == "selectall") {
                ProcessSelectAllAction(parameters);
            }
            else {
                string itemOid = parameters[2];

                if (_dataSourceList == null)
                    return true;
                var itemToBeEdited =
                    _dataSourceList.Cast<XPBaseObject>()
                        .FirstOrDefault(item => item.GetMemberValue(Model.ModelClass.KeyProperty).ToString() == itemOid);

                if (actionId == "select") {
                    ProccessSelectAction(parameters, itemOid);
                }
                else if (actionId == "selecthorizontal") {
                    ProccessSelectHorizontalActrion(parameters);
                }
                else if (actionId == "selectvertical") {
                    ProccessSelectVerticlaAction(parameters);
                }
                else {
                    mustRenderControlAgain = ProccessOtherAction(actionId, itemToBeEdited);
                }
            }
            return false;
        }

        private bool ProccessOtherAction(string actionId, XPBaseObject itemToBeEdited) {
            ActionBase actionToBeExecuted = null;
            foreach (ActionBase action in _contextMenu.Actions) {
                if (action.Id == actionId) {
                    actionToBeExecuted = action;
                }
            }
            ExecuteActionOnObject(itemToBeEdited, actionToBeExecuted);
            return false;
        }

        private void ProcessSelectAllAction(string[] parameters) {
            bool check = bool.Parse(parameters[2]);
            foreach (ASPxCheckBox chk in _checkboxObjectIds.Keys) {
                chk.Checked = check;
            }
            DoGetSelected();
        }

        private void ProccessSelectVerticlaAction(string[] parameters) {
            bool check = bool.Parse(parameters[3]);
            string itemIdentifier = parameters[2];
            foreach (ITwoDimensionItem item in _dataSourceList) {
                var bo = (item as XPBaseObject);
                if (GetIdentifier(item.VerticalDimension) == itemIdentifier) {
                    if (bo != null) {
                        string entityOid = bo.GetMemberValue(bo.ClassInfo.KeyProperty.Name).ToString();
                        foreach (ASPxCheckBox chk in _checkboxObjectIds.Keys) {
                            if (_checkboxObjectIds[chk].ToString() == entityOid) {
                                chk.Checked = check;
                                break;
                            }
                        }
                    }
                }
            }
            DoGetSelected();
        }

        private void ProccessSelectHorizontalActrion(string[] parameters) {
            bool check = bool.Parse(parameters[3]);
            string itemIdentifier = parameters[2];
            foreach (ITwoDimensionItem item in _dataSourceList) {
                var bo = (item as XPBaseObject);
                if (GetIdentifier(item.HorizontalDimension) == itemIdentifier) {
                    if (bo != null) {
                        string entityOid = bo.GetMemberValue(bo.ClassInfo.KeyProperty.Name).ToString();
                        foreach (ASPxCheckBox chk in _checkboxObjectIds.Keys) {
                            if (_checkboxObjectIds[chk].ToString() == entityOid) {
                                chk.Checked = check;
                                break;
                            }
                        }
                    }
                }
            }
            DoGetSelected();
        }

        private void ProccessSelectAction(string[] parameters, string itemOid) {
            bool check = bool.Parse(parameters[3]);
            foreach (ASPxCheckBox chk in _checkboxObjectIds.Keys) {
                if (_checkboxObjectIds[chk].ToString() == itemOid) {
                    chk.Checked = check;
                    break;
                }
            }

            DoGetSelected();
        }

        private ICallbackManagerHolder GetCallbackManager() {
            return ((ICallbackManagerHolder)WebWindow.CurrentRequestPage);
        }

        protected override object CreateControlsCore() {
            _table = new Table {
                BorderWidth = 0,
                Width = new Unit(100, UnitType.Percentage),
                CellPadding = 0,
                CellSpacing = 0,
                ID = "TimeTableListEditor_Table"
            };

            _callbackManager = GetCallbackManager().CallbackManager;
            _callbackManager.RegisterHandler(_uniqueId, this);

            _table.Load += table_Load;
            if (FastMode) {
                _callbackPanel = new ASPxCallbackPanel {
                    Width = new Unit(100, UnitType.Percentage),
                    ClientInstanceName = "CallbackPanel" + Model.Id
                };
                _callbackPanel.Callback += callbackPanel_Callback;
                _callbackPanel.Controls.Add(_table);
                return _callbackPanel;
            }
            return _table;
        }

        private void callbackPanel_Callback(object sender, CallbackEventArgsBase e) {
            ProcessAction(e.Parameter);
        }

        private void table_Load(object sender, EventArgs e) {
            foreach (ASPxEditBase btn in _buttonScripts.Keys) {
                AddCorrectCallbackScriptToButton(btn);
            }
            foreach (ASPxEditBase chk in _checkboxScripts.Keys) {
                AddCorrectCallbackScriptToCheckbox(chk);
            }
        }

        private void AddCorrectCallbackScriptToCheckbox(ASPxEditBase chk) {
            if (FastMode)
                chk.SetClientSideEventHandler("CheckedChanged",
                    "function(s, e) { CallbackPanel" + Model.Id + ".PerformCallback(" + _checkboxScripts[chk] + "); }");
            else
                chk.SetClientSideEventHandler("CheckedChanged",
                    "function(s, e) { " + _callbackManager.GetScript(_uniqueId, _checkboxScripts[chk]) + " }");
        }

        private void AddCorrectCallbackScriptToButton(ASPxEditBase btn) {
            if (string.IsNullOrEmpty(_buttonConfirmations[btn]))
                btn.SetClientSideEventHandler("Click",
                    "function(s, e) { " + _callbackManager.GetScript(_uniqueId, _buttonScripts[btn]) + " }");
            else
                btn.SetClientSideEventHandler("Click",
                    "function(s, e) { " +
                    _callbackManager.GetScript(_uniqueId, _buttonScripts[btn], _buttonConfirmations[btn]) + " }");
        }

        protected void RemoveFalseByContext(BoolList checkedList) {
            string[] contexts = ShowCheckboxes
                ? new[] { ActionBase.RequireSingleObjectContext }
                : new[] { ActionBase.RequireSingleObjectContext, ActionBase.RequireMultipleObjectsContext };
            foreach (string item in contexts) {
                if (checkedList.GetKeys().Contains(item))
                    checkedList.RemoveItem(item);
            }
        }


        private void ExecuteActionOnObject(XPBaseObject itemToBeEdited, ActionBase actionToBeExecuted) {
            List<object> temp = _selectedObjects;
            BoolList temp2 = actionToBeExecuted.Enabled;
            object temp3 = FocusedObject;
            try {
                _selectedObjects = new List<object>();
                if (itemToBeEdited != null)
                    _selectedObjects.Add(itemToBeEdited);

                FocusedObject = itemToBeEdited;

                OnSelectionChanged();
                OnFocusedObjectChanged();

                RemoveFalseByContext(actionToBeExecuted.Enabled);

                var action = actionToBeExecuted as PopupWindowShowAction;
                if (action != null) {
                    WebApplication.Instance.PopupWindowManager.RegisterStartupPopupWindowShowActionScript(
                        (WebControl)Control, action);
                }
                else if (actionToBeExecuted is SimpleAction) {
                    (actionToBeExecuted as SimpleAction).DoExecute();
                }
            }
            finally {
                _selectedObjects = temp;
                FocusedObject = temp3;
                OnSelectionChanged();
                OnFocusedObjectChanged();
                foreach (string key in temp2.GetKeys()) {
                    actionToBeExecuted.Enabled.SetItemValue(key, temp2[key]);
                }
            }
            if (_contextMenu != null)
                _contextMenu.CreateControls();
        }

        public override void BreakLinksToControls() {
            if (_table != null)
                _table.Load -= table_Load;
            _table = null;
            _callbackManager = null;
            if (_callbackPanel != null)
                _callbackPanel.Callback -= callbackPanel_Callback;
            _callbackPanel = null;

            _buttonScripts.Clear();
            _buttonConfirmations.Clear();
            _checkboxScripts.Clear();
            _checkboxObjectIds.Clear();
            if (_contextMenu != null) {
                _contextMenu.BreakLinksToControls();
            }

            base.BreakLinksToControls();
        }

        public override void Dispose() {
            if (_contextMenu != null) {
                _contextMenu.Dispose();
                _contextMenu = null;
            }
            _selectedObjects.Clear();
            DefaultHorizontalDimensions.Clear();
            DefaultVerticalDimensions.Clear();
            base.Dispose();
        }


        protected override void AssignDataSourceToControl(object dataSource) {
            _dataSourceList = ListHelper.GetList(dataSource);
            RenderControl();
        }

        private void RenderControl() {
            if (_table != null) {
                _buttonScripts.Clear();
                _buttonConfirmations.Clear();
                _checkboxScripts.Clear();
                _checkboxObjectIds.Clear();

                _table.Rows.Clear();

                ITypeInfo listTypeInfo = Model.ModelClass.TypeInfo;

                var schedulelines = new Dictionary<IComparable, Dictionary<IComparable, List<ITwoDimensionItem>>>();
                var horizontalDimensionItems = new List<IComparable>();
                foreach (ITwoDimensionItem item in _dataSourceList) {
                    if (!schedulelines.ContainsKey(item.VerticalDimension))
                        schedulelines[item.VerticalDimension] = new Dictionary<IComparable, List<ITwoDimensionItem>>();
                    if (!schedulelines[item.VerticalDimension].ContainsKey(item.HorizontalDimension))
                        schedulelines[item.VerticalDimension][item.HorizontalDimension] = new List<ITwoDimensionItem>();
                    schedulelines[item.VerticalDimension][item.HorizontalDimension].Add(item);
                    if (!horizontalDimensionItems.Contains(item.HorizontalDimension))
                        horizontalDimensionItems.Add(item.HorizontalDimension);
                }
                var tr = new TableRow();
                _table.Rows.Add(tr);
                var tc = new TableCell();
                tr.Cells.Add(tc);
                tc.BackColor = Color.Gainsboro;
                tc.Attributes["style"] = "border:solid 1px gray; color:black";
                tc.CssClass = "StaticText";

                ASPxCheckBox chkAll = null;
                if (ShowCheckboxes) {
                    var t = new Table();
                    tc.Controls.Add(t);
                    var trr = new TableRow();
                    t.Rows.Add(trr);
                    var tcc = new TableCell();
                    trr.Cells.Add(tcc);
                    chkAll = new ASPxCheckBox { ID = "chkAll" };
                    tcc.Controls.Add(chkAll);
                }

                if (DefaultHorizontalDimensions != null)
                    DefaultHorizontalDimensions.Where(t => !horizontalDimensionItems.Contains(t))
                        .ToList()
                        .ForEach(horizontalDimensionItems.Add);
                horizontalDimensionItems.Sort();
                bool allSelected = true;
                foreach (IComparable item in horizontalDimensionItems) {
                    tc = new TableCell { BackColor = Color.Gainsboro };
                    tc.Attributes["style"] = "border:solid 1px gray; color:black";
                    tc.CssClass = "StaticText";

                    if (HorizontalDimensionColorPattern != null)
                        tc.BackColor = HorizontalDimensionColorPattern(item);

                    var inner = new Table { CellPadding = 3, CellSpacing = 0, BorderWidth = 0 };
                    tc.Controls.Add(inner);

                    tr.Cells.Add(tc);

                    TableCell innertc;

                    var innertr = new TableRow();
                    inner.Rows.Add(innertr);

                    if (ShowCheckboxes) {
                        var chk = new ASPxCheckBox();
                        bool atLeastOneEntityExists = false;
                        bool atLeastOneEntityUnchecked = false;
                        foreach (IComparable vkey in schedulelines.Keys) {
                            if (schedulelines[vkey].ContainsKey(item)) {
                                atLeastOneEntityExists = true;
                                if (schedulelines[vkey][item].Any(ent => !_selectedObjects.Contains(ent))) {
                                    atLeastOneEntityUnchecked = true;
                                }
                                if (atLeastOneEntityUnchecked)
                                    break;
                            }
                        }
                        bool rowselected = atLeastOneEntityExists && !atLeastOneEntityUnchecked;
                        chk.Checked = rowselected;
                        if (!rowselected)
                            allSelected = false;

                        chk.EnableClientSideAPI = true;
                        chk.ID = "chkHor" + GetIdentifier(item);

                        innertc = new TableCell();
                        innertr.Cells.Add(innertc);
                        innertc.Controls.Add(chk);
                        _checkboxScripts[chk] = "\"action_selecthorizontal_" + GetIdentifier(item) + "_" + (!chk.Checked) +
                                               "\"";
                        AddCorrectCallbackScriptToCheckbox(chk);
                    }

                    innertc = new TableCell();
                    innertr.Cells.Add(innertc);
                    innertc.Text = GetCaption(item, true);
                }

                if (ShowCheckboxes) {
                    chkAll.Checked = allSelected && _dataSourceList.Count > 0;
                    chkAll.EnableClientSideAPI = true;
                    _checkboxScripts[chkAll] = "\"action_selectall_" + (!chkAll.Checked) + "\"";
                    AddCorrectCallbackScriptToCheckbox(chkAll);
                }

                foreach (IModelColumn column in Model.Columns) {
                    if (column.Index >= 0) {
                        var summary = (IModelColumnSummaryItemEditabledTabEnabled)column.Summary.FirstOrDefault();
                        if (summary != null){
                            var summary2 = ((IModelColumnSummaryItemTwoDimensionListEditor)summary).TwoDimensionListEditor;
                            if (summary != null && summary2 != null &&
                                (summary2.SummaryAppearance == DimensionsEnum.Both ||
                                 summary2.SummaryAppearance == DimensionsEnum.Horizontal)) {
                                tc = new TableCell();
                                tc.Attributes["style"] = "padding:3px 3px 3px 3px; border:solid 1px gray; color:black";
                                tc.CssClass = "StaticText";
                                tc.BackColor = Color.Silver;
                                tr.Cells.Add(tc);
                                tc.Text = CaptionHelper.GetMemberCaption(listTypeInfo, column.PropertyName) + " " +
                                          summary.EditableTabEnabledEditorSummaryItem.SummaryCaption;
                            }
                        }
                    }
                }


                Dictionary<IComparable, Dictionary<IComparable, List<ITwoDimensionItem>>>.KeyCollection
                    verticalDimensionKeys = schedulelines.Keys;
                List<IComparable> verticalDimensionItems = verticalDimensionKeys.ToList();
                if (DefaultVerticalDimensions != null)
                    DefaultVerticalDimensions.Where(t => !verticalDimensionItems.Contains(t))
                        .ToList()
                        .ForEach(verticalDimensionItems.Add);

                verticalDimensionItems.Sort();
                foreach (IComparable vitem in verticalDimensionItems) {
                    tr = new TableRow();
                    _table.Rows.Add(tr);
                    tc = new TableCell { BackColor = Color.Gainsboro };
                    tc.Attributes["style"] = "border:solid 1px gray; color:black";
                    tc.CssClass = "StaticText";
                    if (VerticalDimensionColorPattern != null)
                        tc.BackColor = VerticalDimensionColorPattern(vitem);

                    var header = new Table { CellPadding = 3, CellSpacing = 0, BorderWidth = 0 };
                    tc.Controls.Add(header);

                    tr.Cells.Add(tc);

                    TableCell hinnertc;
                    var hinnertr = new TableRow();
                    header.Rows.Add(hinnertr);

                    if (ShowCheckboxes) {
                        var chk = new ASPxCheckBox();
                        bool atLeastOneEntityExists = false;
                        bool atLeastOneEntityUnchecked = false;

                        if (schedulelines.ContainsKey(vitem)) {
                            foreach (IComparable hkey in schedulelines[vitem].Keys) {
                                atLeastOneEntityExists = true;
                                if (schedulelines[vitem][hkey].Any(ent => !_selectedObjects.Contains(ent))) {
                                    atLeastOneEntityUnchecked = true;
                                }
                                if (atLeastOneEntityUnchecked)
                                    break;
                            }
                        }
                        chk.Checked = atLeastOneEntityExists && !atLeastOneEntityUnchecked;
                        chk.ID = "chkVer" + GetIdentifier(vitem);

                        chk.EnableClientSideAPI = true;
                        hinnertc = new TableCell();
                        hinnertr.Cells.Add(hinnertc);
                        hinnertc.Controls.Add(chk);
                        _checkboxScripts[chk] = "\"action_selectvertical_" + GetIdentifier(vitem) + "_" + (!chk.Checked) +
                                               "\"";
                        AddCorrectCallbackScriptToCheckbox(chk);
                    }


                    hinnertc = new TableCell();
                    hinnertr.Cells.Add(hinnertc);

                    hinnertc.Text = GetCaption(vitem, false);

                    foreach (IComparable hitem in horizontalDimensionItems) {
                        tc = new TableCell();
                        tr.Cells.Add(tc);


                        tc.Attributes["style"] = "border:solid 1px gray; color:black";
                        tc.CssClass = "StaticText";
                        if (schedulelines.ContainsKey(vitem) && schedulelines[vitem].ContainsKey(hitem)) {
                            var inner = new Table {
                                CellPadding = 3,
                                CellSpacing = 0,
                                BorderWidth = 0,
                                Width = new Unit(100, UnitType.Percentage),
                                Height = new Unit(100, UnitType.Percentage)
                            };
                            tc.Controls.Add(inner);

                            foreach (ITwoDimensionItem item in schedulelines[vitem][hitem]) {
                                TableCell innertc;
                                var innertr = new TableRow();


                                inner.Rows.Add(innertr);

                                if (ShowCheckboxes) {
                                    object entityId = (item as XPBaseObject).GetMemberValue(Model.ModelClass.KeyProperty);
                                    var chk = new ASPxCheckBox();
                                    if (_selectedObjects.Contains(item))
                                        chk.Checked = true;

                                    chk.ID = "chkItem" +
                                             (item as XPBaseObject).GetMemberValue(Model.ModelClass.KeyProperty);

                                    chk.EnableClientSideAPI = true;
                                    innertc = new TableCell();
                                    innertr.Cells.Add(innertc);
                                    innertc.Controls.Add(chk);
                                    _checkboxScripts[chk] = "\"action_select_" +
                                                           (item as XPBaseObject).GetMemberValue(
                                                               Model.ModelClass.KeyProperty) + "_" + (!chk.Checked) +
                                                           "\"";
                                    _checkboxObjectIds[chk] = entityId;
                                    AddCorrectCallbackScriptToCheckbox(chk);
                                }

                                OnCustomizeAppearance(new CustomizeAppearanceEventArgs("ALLCOLUMNS",
                                    new WebControlAppearanceAdapter(inner, inner), item));

                                var innerInner = new Table();
                                innertc = new TableCell();
                                innertr.Cells.Add(innertc);
                                innertc.Controls.Add(innerInner);

                                //////Release to display default property instead of all columns/////////
                                //TableRow innerInnerTr = new TableRow();
                                //innerInner.Rows.Add(innerInnerTr);
                                //TableCell innerInnerTc = new TableCell();
                                //innerInnerTr.Cells.Add(innerInnerTc);
                                //Literal lit = new Literal();
                                //lit.Text = listTypeInfo.DefaultMember == null ? item.ToString() : (item as XPBaseObject).GetMemberValue(listTypeInfo.DefaultMember.Name).ToString();
                                //innerInnerTc.Controls.Add(lit);

                                foreach (IModelColumn column in Model.Columns) {
                                    var innerInnerTr = new TableRow();
                                    innerInner.Rows.Add(innerInnerTr);
                                    var innerInnerTc = new TableCell();
                                    innerInnerTr.Cells.Add(innerInnerTc);
                                    var lit = new Literal {
                                        Text = (!string.IsNullOrEmpty(column.DisplayFormat))
                                            ? string.Format(column.DisplayFormat,
                                                (item as XPBaseObject).GetMemberValue(column.PropertyName))
                                            : (item as XPBaseObject).GetMemberValue(column.PropertyName).ToString()
                                    };
                                    innerInnerTc.Controls.Add(lit);
                                    OnCustomizeAppearance(new CustomizeAppearanceEventArgs(column.PropertyName,
                                        new WebControlAppearanceAdapter(innerInnerTc, innerInnerTc), item));
                                }

                                foreach (ActionBase action in _contextMenu.Actions) {
                                    if (action.Enabled || IsFalseByContextRequiredOrCriteria(action.Enabled)) {
                                        ASPxEditBase btn;
                                        if (action.HasImage) {
                                            btn = new ASPxImage();
                                            (btn as ASPxImage).ImageUrl =
                                                ImageLoader.Instance.GetImageInfo(action.ImageName, true).ImageUrl;
                                            (btn as ASPxImage).Cursor = "pointer";
                                            btn.ToolTip = action.Caption;
                                        }
                                        else {
                                            btn = new ASPxHyperLink();
                                            (btn as ASPxHyperLink).Cursor = "pointer";
                                            (btn as ASPxHyperLink).Text = action.Caption;
                                        }
                                        _buttonConfirmations[btn] = action.ConfirmationMessage;
                                        btn.EnableClientSideAPI = true;
                                        _buttonScripts[btn] = "\"action_" + action.Id + "_" +
                                                             (item as XPBaseObject).GetMemberValue(
                                                                 Model.ModelClass.KeyProperty) + "\"";
                                        innertc = new TableCell();
                                        innertr.Cells.Add(innertc);
                                        innertc.Controls.Add(btn);
                                        if (!string.IsNullOrEmpty(action.TargetObjectsCriteria))
                                            btn.Visible =
                                                (bool)(item as XPBaseObject).Evaluate(action.TargetObjectsCriteria);

                                        AddCorrectCallbackScriptToButton(btn);
                                    }
                                }
                            }
                        }
                    }

                    var items = new List<ITwoDimensionItem>();
                    if (schedulelines.ContainsKey(vitem)) {
                        foreach (var item in schedulelines[vitem].Values) {
                            items.AddRange(item);
                        }
                    }
                    foreach (IModelColumn column in Model.Columns) {
                        if (column.Index >= 0) {
                            var footerSummaries =CreateFooterSummaries(items, column);
                            foreach (var summary in footerSummaries.Keys) {
                                var summary2 = ((IModelColumnSummaryItemTwoDimensionListEditor)summary).TwoDimensionListEditor;
                                if (summary != null && summary2 != null &&
                                    (summary2.SummaryAppearance == DimensionsEnum.Both ||
                                     summary2.SummaryAppearance == DimensionsEnum.Horizontal)) {
                                    tc = new TableCell();
                                    tc.Attributes["style"] =
                                        "padding:3px 3px 3px 3px;border:solid 1px gray; color:black";
                                    tc.CssClass = "StaticText";
                                    tc.BackColor = Color.Silver;
                                    tr.Cells.Add(tc);
                                    tc.Text = footerSummaries[summary];
                                }
                            }
                        }
                    }
                }

                foreach (IModelColumn column in Model.Columns) {
                    if (column.Index >= 0) {
                        foreach (var modelColumnSummaryItem in column.Summary.ToList()) {
                            var summary = (IModelColumnSummaryItemEditabledTabEnabled)modelColumnSummaryItem;
                            var summary2 = ((IModelColumnSummaryItemTwoDimensionListEditor)summary).TwoDimensionListEditor;
                            if (summary != null && summary2 != null &&
                                (summary2.SummaryAppearance == DimensionsEnum.Both ||
                                 summary2.SummaryAppearance == DimensionsEnum.Vertical)) {
                                tr = new TableRow { BackColor = Color.Silver };
                                _table.Rows.Add(tr);
                                tc = new TableCell();
                                tc.Attributes["style"] = "padding:3px 3px 3px 3px;border:solid 1px gray; color:black";
                                tc.CssClass = "StaticText";
                                tr.Cells.Add(tc);
                                tc.Text = CaptionHelper.GetMemberCaption(listTypeInfo, column.PropertyName) + " " +
                                          summary.EditableTabEnabledEditorSummaryItem.SummaryCaption;

                                foreach (IComparable hitem in horizontalDimensionItems) {
                                    var items = verticalDimensionItems.Where(vitem => schedulelines.ContainsKey(vitem) && schedulelines[vitem].ContainsKey(hitem)).SelectMany(vitem => schedulelines[vitem][hitem]).ToList();
                                    var footerSummaries =CreateFooterSummaries(items, column);
                                    foreach (var key in footerSummaries.Keys) {
                                        tc = new TableCell();
                                        tc.Attributes["style"] = (ShowCheckboxes
                                            ? "padding:3px 3px 3px 30px;"
                                            : "padding:3px 3px 3px 3px;") + "border:solid 1px gray; color:black";
                                        tc.CssClass = "StaticText";
                                        tr.Cells.Add(tc);
                                        tc.Text = footerSummaries[key];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<IModelColumnSummaryItemEditabledTabEnabled, string> CreateFooterSummaries(IList objects,
            IModelColumn column) {
                var retval = new Dictionary<IModelColumnSummaryItemEditabledTabEnabled, string>();
            foreach (var modelColumnSummaryItem in column.Summary.ToList()) {
                var summary = (IModelColumnSummaryItemEditabledTabEnabled)modelColumnSummaryItem;
                if (summary != null) {
                    string summaryText = "";
                    switch (summary.SummaryType) {
                        case SummaryType.Average:
                            var avg = (from XPBaseObject item in objects where item != null && item.GetMemberValue(column.PropertyName) != null select Convert.ToDecimal(item.GetMemberValue(column.PropertyName))).ToList();
                            decimal avgval = avg.Average();
                            if (!string.IsNullOrEmpty(column.DisplayFormat))
                                summaryText = string.Format(column.DisplayFormat, avgval);
                            else if (column.ModelMember.Type == typeof(decimal))
                                summaryText = avgval.ToString("C2");
                            else
                                summaryText = avgval.ToString("F0");
                            break;
                        case SummaryType.Count:
                            summaryText = objects.Count.ToString("F0");
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
                            decimal sum = (from XPBaseObject item in objects where item != null && item.GetMemberValue(column.PropertyName) != null select Convert.ToDecimal(item.GetMemberValue(column.PropertyName))).Sum();
                            if (!string.IsNullOrEmpty(column.DisplayFormat))
                                summaryText = string.Format(column.DisplayFormat, sum);
                            else if (column.ModelMember.Type == typeof(decimal))
                                summaryText = sum.ToString("C2");
                            else
                                summaryText = sum.ToString("F0");
                            break;
                    }
                    retval[summary] = summaryText;
                }
            }
            return retval;
        }


        private string GetCaption(IComparable item, bool isHorizontalDimension) {
            if (item == null)
                return "";
            if (item is XPBaseObject) {
                var obj = (item as XPBaseObject);

                ITypeInfo relativeItemInfo = XafTypesInfo.Instance.FindTypeInfo(obj.GetType());
                return relativeItemInfo.DefaultMember == null
                    ? obj.ToString()
                    : obj.GetMemberValue(relativeItemInfo.DefaultMember.Name).ToString();
            }

            IMemberInfo dimensionMember = isHorizontalDimension
                ? ((Model.ModelClass.TypeInfo).FindMember("HorizontalDimension"))
                : ((Model.ModelClass.TypeInfo).FindMember("VerticalDimension"));
            var modelDefaultAttribute = dimensionMember.FindAttribute<ModelDefaultAttribute>();
            if (modelDefaultAttribute != null && modelDefaultAttribute.PropertyName == "DisplayFormat" &&
                !string.IsNullOrEmpty((modelDefaultAttribute.PropertyValue))) {
                return string.Format(modelDefaultAttribute.PropertyValue, item);
            }
            return item.ToString();
        }

        private string GetIdentifier(IComparable item) {
            if (item == null)
                return "";
            if (item is XPBaseObject) {
                var obj = (item as XPBaseObject);

                ITypeInfo relativeItemInfo = XafTypesInfo.Instance.FindTypeInfo(obj.GetType());
                return obj.GetMemberValue(relativeItemInfo.KeyMember.Name).ToString();
            }
            return item.ToString();
        }

        protected bool IsFalseByContextRequiredOrCriteria(BoolList checkedList) {
            string[] contextRequiredKeys ={
                ActionBase.RequireSingleObjectContext,
                ActionBase.RequireMultipleObjectsContext, ActionsCriteriaViewController.EnabledByCriteriaKey
            };
            bool result = !checkedList;
            if (result) {
                if (checkedList.GetKeys().Any(activeKey => !checkedList[activeKey] && (Array.IndexOf(contextRequiredKeys, activeKey) == -1))) {
                    result = false;
                }
            }
            return result;
        }


        public override void Refresh() {
            //BindDataSource();
        }


        public override IList GetSelectedObjects() {
            return _selectedObjects;
        }

        private void DoGetSelected() {
            _selectedObjects.Clear();
            foreach (ASPxCheckBox chk in _checkboxObjectIds.Keys) {
                object itemId = _checkboxObjectIds[chk];
                XPBaseObject o =
                    _dataSourceList.OfType<XPBaseObject>()
                        .FirstOrDefault(t => t.GetMemberValue(t.ClassInfo.KeyProperty.Name).Equals(itemId));
                if (o != null && !o.IsDeleted && chk.Checked) {
                    if (!_selectedObjects.Contains(o))
                        _selectedObjects.Add(o);
                }
            }

            FocusedObject = _selectedObjects.Count == 0 ? null : _selectedObjects[0];

            OnFocusedObjectChanged();

            OnSelectionChanged();
        }

        public override void SaveModel() {
        }

        internal void BindDataSource() {
            if (_table != null) {
                _table.Rows.Clear(); //clear rows in order to recreate the control 
                RenderControl();
                _contextMenu.CreateControls();

                DoGetSelected();
            }
        }


        internal void ClearSelection() {
            _selectedObjects.Clear();
        }

        protected virtual void OnCustomizeAppearance(CustomizeAppearanceEventArgs args) {
            if (CustomizeAppearance != null) {
                CustomizeAppearance(this, args);
            }
        }
    }

    public class TimeTableContextMenuTemplate : IContextMenuTemplate {
        private static readonly string[] _contextRequiredKeys ={
            ActionBase.RequireSingleObjectContext,
            ActionBase.RequireMultipleObjectsContext, ActionsCriteriaViewController.EnabledByCriteriaKey
        };

        private readonly List<WebContextMenuActionContainer> _containers = new List<WebContextMenuActionContainer>();
        public List<ActionBase> Actions = new List<ActionBase>();
        private TwoDimensionListEditor _editor;
        private ActionBase _processSelectedItemAction;

        public TimeTableContextMenuTemplate(TwoDimensionListEditor editor) {
            _editor = editor;
        }


        public event EventHandler<BoundItemCreatingEventArgs> BoundItemCreating;

        public void CreateActionItems(IFrameTemplate parent, ListView context,
            ICollection<IActionContainer> contextContainers) {
            foreach (WebContextMenuActionContainer container in _containers) {
                container.Dispose();
            }
            _containers.Clear();
            foreach (IActionContainer sourceContainer in contextContainers) {
                var container = new WebContextMenuActionContainer { ContainerId = sourceContainer.ContainerId };
                foreach (ActionBase action in sourceContainer.Actions) {
                    if (action.Id == ListViewProcessCurrentObjectController.ListViewShowObjectActionId &&
                        _processSelectedItemAction == null) {
                        _processSelectedItemAction = action;
                    }
                    container.Register(action);
                }
                _containers.Add(container);
            }
            CreateControls();
        }

        public void Dispose() {
            _editor = null;
        }

        internal void BreakLinksToControls() {
        }

        internal void CreateControls() {
            Actions.Clear();
            foreach (WebContextMenuActionContainer container in _containers) {
                foreach (ActionBase action in container.Actions) {
                    if (IsActionVisibleInContextMenu(action)) {
                        Actions.Add(action);
                    }
                }
            }
        }

        protected bool IsActionVisibleInContextMenu(ActionBase action) {
            return IsActionVisibleInContextMenu(_processSelectedItemAction, action);
        }

        protected virtual bool IsActionVisibleInContextMenu(ActionBase defaultAction, ActionBase action) {
            if ((action != null) &&
                (action.Active || IsFalseByContextRequiredOrCriteria(action.Active)) &&
                defaultAction != action
                &&
                (
                    action.SelectionDependencyType == SelectionDependencyType.RequireSingleObject ||
                    (action.SelectionDependencyType == SelectionDependencyType.RequireMultipleObjects &&
                     (_editor.SelectionType & SelectionType.TemporarySelection) == SelectionType.TemporarySelection))
                ) {
                return true;
            }
            return false;
        }

        protected bool IsFalseByContextRequiredOrCriteria(BoolList checkedList) {
            bool result = !checkedList;
            if (result) {
                if (checkedList.GetKeys().Any(activeKey => !checkedList[activeKey] && (Array.IndexOf(_contextRequiredKeys, activeKey) == -1))) {
                    result = false;
                }
            }
            return result;
        }
    }

    public class ViewModeAppliedAtTwoDimensionListEditorController : ViewController<DetailView> {
        private List<TwoDimensionListEditor> _editors;

        protected override void OnActivated() {
            _editors = new List<TwoDimensionListEditor>();
            foreach (ListPropertyEditor item in View.GetItems<ListPropertyEditor>()) {
                if (item != null && item.ListView != null && item.ListView.Editor != null &&
                    item.ListView.Editor is TwoDimensionListEditor) {
                    var listeditor = item.ListView.Editor as TwoDimensionListEditor;
                    if (!_editors.Contains(listeditor))
                        _editors.Add(listeditor);
                    SetViewMode(listeditor);
                }
            }
            View.ViewEditModeChanged += View_ViewEditModeChanged;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.ViewEditModeChanged -= View_ViewEditModeChanged;
            if (_editors != null) {
                _editors.Clear();
                _editors = null;
            }
        }

        private void View_ViewEditModeChanged(object sender, EventArgs e) {
            if (_editors == null)
                return;
            foreach (TwoDimensionListEditor editor in _editors) {
                SetViewMode(editor);
            }
        }


        private void SetViewMode(TwoDimensionListEditor listeditor) {
            listeditor.ViewMode = View.ViewEditMode == ViewEditMode.View;
        }
    }
}