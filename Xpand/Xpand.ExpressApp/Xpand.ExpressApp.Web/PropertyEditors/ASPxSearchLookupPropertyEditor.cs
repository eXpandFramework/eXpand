using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(object), EditorAliases.ASPxSearchLookupPropertyEditor, false)]
    public class ASPxSearchLookupPropertyEditor : ASPxObjectPropertyEditorBase, IDependentPropertyEditor,
                                                  ISupportViewShowing, IFrameContainer {
        static int _windowWidth = 800;
        static int _windowHeight = 480;
        readonly List<IObjectSpace> _createdObjectSpaces = new List<IObjectSpace>();
        string _editorId;
        NestedFrame _frame;
        WebLookupEditorHelper _helper;
        ListView _listView;
        object _newObject;
        IObjectSpace _newObjectSpace;
        NewObjectViewController _newObjectViewController;
        PopupWindowShowAction _newObjectWindowAction;
        ASPxSearchDropDownEdit _searchDropDownEdit;
        WebApplication _application;
        IObjectSpace _objectSpace;

        public ASPxSearchLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            skipEditModeDataBind = true;
        }

        public WebLookupEditorHelper WebLookupEditorHelper {
            get { return _helper; }
        }

        protected CollectionSourceBase DataSource {
            get {
                if (_listView != null) {
                    return _listView.CollectionSource;
                }
                return null;
            }
        }

        public static int WindowWidth {
            get { return _windowWidth; }
            set { _windowWidth = value; }
        }

        public static int WindowHeight {
            get { return _windowHeight; }
            set { _windowHeight = value; }
        }

        internal LookupEditorHelper Helper {
            get { return _helper; }
        }
        #region IDependentPropertyEditor Members
        IList<string> IDependentPropertyEditor.MasterProperties {
            get { return _helper.MasterProperties; }
        }
        #endregion
        #region IFrameContainer Members
        public Frame Frame {
            get {
                InitializeFrame();
                return _frame;
            }
        }

        public void InitializeFrame() {
            if (_frame == null) {
                _frame = _helper.Application.CreateNestedFrame(this, TemplateContext.LookupControl);
                _newObjectViewController = _frame.GetController<NewObjectViewController>();
                if (_newObjectViewController != null) {
                    _newObjectViewController.ObjectCreating += newObjectViewController_ObjectCreating;
                    _newObjectViewController.ObjectCreated += newObjectViewController_ObjectCreated;
                }
            }
        }
        #endregion
        #region ISupportViewShowing Members
        event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification {
            add { viewShowingNotification += value; }
            remove { viewShowingNotification -= value; }
        }
        #endregion
        protected override void SetImmediatePostDataScript(string script) {
            _searchDropDownEdit.DropDown.ClientSideEvents.SelectedIndexChanged = script;
        }

        protected override void SetImmediatePostDataCompanionScript(string script) {
            _searchDropDownEdit.DropDown.SetClientSideEventHandler("GotFocus", script);
        }

        void UpdateDropDownLookupControlAddButton(ASPxSearchDropDownEdit control) {
            control.AddingEnabled = false;
            if (CurrentObject != null) {
                string diagnosticInfo;
                RecreateListView(true);
                control.AddingEnabled = AllowEdit &&
                                        DataManipulationRight.CanCreate(_listView, _helper.LookupObjectType,
                                                                        _listView.CollectionSource, out diagnosticInfo);
                if (control.AddingEnabled) {
                    if (_newObjectViewController != null) {
                        control.AddingEnabled = _newObjectViewController.NewObjectAction.Active &&
                                                _newObjectViewController.NewObjectAction.Enabled;
                    }
                }
            }
        }

        ASPxSearchDropDownEdit CreateSearchDropDownEditControl() {
            var result = new ASPxSearchDropDownEdit(_helper, EmptyValue, DisplayFormat) { Width = Unit.Percentage(100) };
            result.DropDown.SelectedIndexChanged += dropDownLookup_SelectedIndexChanged;
            result.Init += dropDownLookup_Init;
            result.PreRender += dropDownLookup_PreRender;
            result.Callback += result_Callback;
            result.ReadOnly = !AllowEdit;
            UpdateDropDownLookup(result);
            return result;
        }

        void result_Callback(object sender, CallbackEventArgs e) {
            FillSearchDropDownValues(GetObjectByKey(String.Format("{0}({1})", Helper.LookupObjectTypeInfo, e.Argument)));
        }

        void UpdateDropDownLookup(WebControl editor) {
            var supportNewObjectCreating = editor as ASPxSearchDropDownEdit;
            if (supportNewObjectCreating != null) {
                if (_newObjectViewController != null) {
                    supportNewObjectCreating.NewActionCaption = _newObjectViewController.NewObjectAction.Caption;
                }
                UpdateDropDownLookupControlAddButton(supportNewObjectCreating);
                if (_application != null) {
                    supportNewObjectCreating.SetClientNewButtonScript(
                        _application.PopupWindowManager.GenerateModalOpeningScript(editor, _newObjectWindowAction,
                                                                                  WindowWidth, WindowHeight, false,
                                                                                  supportNewObjectCreating.
                                                                                      GetProcessNewObjFunction()));
                }
            }
        }

        void dropDownLookup_SelectedIndexChanged(object source, EventArgs e) {
            EditValueChangedHandler(source, EventArgs.Empty);
        }

        void dropDownLookup_Init(object sender, EventArgs e) {
            UpdateDropDownLookup((WebControl)sender);
        }

        void dropDownLookup_PreRender(object sender, EventArgs e) {
            UpdateDropDownLookup((WebControl)sender);
        }

        object GetObjectByKey(string key) {
            return _helper.GetObjectByKey(CurrentObject, key);
        }

        void newObjectViewController_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
            e.ShowDetailView = false;
            // B196715
            if (e.ObjectSpace is INestedObjectSpace) {
                e.ObjectSpace = _application.CreateObjectSpace();
            }
        }

        void newObjectViewController_ObjectCreated(object sender, ObjectCreatedEventArgs e) {
            _newObject = e.CreatedObject;
            _newObjectSpace = e.ObjectSpace;
            _createdObjectSpaces.Add(_newObjectSpace);
        }

        void newObjectWindowAction_OnCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
            if (!DataSource.AllowAdd) {
                throw new InvalidOperationException();
            }
            if (_newObjectViewController != null) {
                //TODO MINAKOV rewrite
                OnViewShowingNotification(); //CaptionHelper.GetLocalizedText("DialogButtons", "Add"));
                _newObjectViewController.NewObjectAction.DoExecute(_newObjectViewController.NewObjectAction.Items[0]);
                args.View = _application.CreateDetailView(_newObjectSpace, _newObject, _listView);
            }
        }

        void newObjectWindowAction_OnExecute(Object sender, PopupWindowShowActionExecuteEventArgs args) {
            if (!DataSource.AllowAdd) {
                throw new InvalidOperationException();
            }
            if (_objectSpace != args.PopupWindow.View.ObjectSpace) {
                args.PopupWindow.View.ObjectSpace.CommitChanges();
            }
            var detailView = (DetailView)args.PopupWindow.View;
            DataSource.Add(_helper.ObjectSpace.GetObject(detailView.CurrentObject));
            ((PopupWindow)args.PopupWindow).ClosureScript =
                "if(window.dialogOpener != null) window.dialogOpener.ddLookupResult = '" +
                detailView.ObjectSpace.GetKeyValueAsString(detailView.CurrentObject) + "';";
        }

        void RecreateListView(bool ifNotCreatedOnly) {
            if (ViewEditMode == ViewEditMode.Edit && (!ifNotCreatedOnly || _listView == null)) {
                _listView = null;
                if (CurrentObject != null) {
                    _listView = _helper.CreateListView(CurrentObject);
                }
                Frame.SetView(_listView);
            }
        }

        void OnViewShowingNotification() {
            if (viewShowingNotification != null) {
                viewShowingNotification(this, EventArgs.Empty);
            }
        }

        protected override void ApplyReadOnly() {
            if (_searchDropDownEdit != null) {
                _searchDropDownEdit.ReadOnly = !AllowEdit;
            }
        }

        protected override WebControl CreateEditModeControlCore() {
            if (_newObjectWindowAction == null) {
                _newObjectWindowAction = new PopupWindowShowAction(null, "New", PredefinedCategory.Unspecified.ToString());
                _newObjectWindowAction.Execute += newObjectWindowAction_OnExecute;
                _newObjectWindowAction.CustomizePopupWindowParams += newObjectWindowAction_OnCustomizePopupWindowParams;
                _newObjectWindowAction.Application = _helper.Application;
            }

            var panel = new Panel();
            // Use Panel instead of ASPxPanel cause it doesn't affect editor ClientID            
            _searchDropDownEdit = CreateSearchDropDownEditControl();
            panel.Controls.Add(_searchDropDownEdit);
            return panel;
        }

        protected override object GetControlValueCore() {
            if (ViewEditMode == ViewEditMode.Edit && Editor != null) {
                ASPxComboBox dropDownControl = _searchDropDownEdit.DropDown;
                if (dropDownControl.Value != null && dropDownControl.Value.ToString() != EmptyValue) {
                    string objectKey = String.Format("{0}({1})", Helper.LookupObjectTypeInfo, dropDownControl.Value);
                    return GetObjectByKey(objectKey);
                }
                return null;
            }
            return MemberInfo.GetValue(CurrentObject);
        }

        protected override void OnCurrentObjectChanged() {
            if (Editor != null) {
                RecreateListView(false);
            }
            base.OnCurrentObjectChanged();
            UpdateDropDownLookup(_searchDropDownEdit);
        }

        protected override string GetPropertyDisplayValue() {
            return _helper.GetDisplayText(MemberInfo.GetValue(CurrentObject), EmptyValue, DisplayFormat);
        }

        void FillSearchDropDownValues(object item) {
            _searchDropDownEdit.DropDown.Items.Clear();
            if (item != null) {
                _searchDropDownEdit.DropDown.DataSource = new List<object>(new[] { item });
                _searchDropDownEdit.DropDown.DataBind();
            }
            _searchDropDownEdit.DropDown.SelectedIndex = _searchDropDownEdit.DropDown.Items.Count - 1;
        }

        protected override void ReadEditModeValueCore() {
            if (_searchDropDownEdit != null) {
                FillSearchDropDownValues(PropertyValue);
            }
        }

        public void SetValueToControl(object obj) {
            if (_searchDropDownEdit != null) {
                ASPxComboBox controlBox = _searchDropDownEdit.DropDown;
                foreach (ListEditItem item in controlBox.Items) {
                    var val = item.Value as string;
                    if (val == _helper.GetObjectKey(obj)) {
                        controlBox.SelectedIndex = item.Index;
                        break;
                    }
                }
            }
        }

        protected override IJScriptTestControl GetInplaceViewModeEditorTestControlImpl() {
            return new JSButtonTestControl();
        }

        protected override WebControl GetActiveControl() {
            if (_searchDropDownEdit != null) {
                return _searchDropDownEdit.DropDown;
            }
            return base.GetActiveControl();
        }

        protected override string GetEditorClientId() {
            return _searchDropDownEdit.ClientID;
        }

        void UpdateControlId() {
            _searchDropDownEdit.ID = _editorId;
        }

        protected override void SetEditorId(string controlId) {
            _editorId = controlId;
            UpdateControlId();
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) {
                    if (_newObjectWindowAction != null) {
                        _newObjectWindowAction.Execute -= newObjectWindowAction_OnExecute;
                        _newObjectWindowAction.CustomizePopupWindowParams -=
                            newObjectWindowAction_OnCustomizePopupWindowParams;
                        DisposeAction(_newObjectWindowAction);
                        _newObjectWindowAction = null;
                    }
                    if (_newObjectViewController != null) {
                        _newObjectViewController.ObjectCreating -= newObjectViewController_ObjectCreating;
                        _newObjectViewController.ObjectCreated -= newObjectViewController_ObjectCreated;
                        _newObjectViewController = null;
                    }
                    if (_frame != null) {
                        _frame.SetView(null);
                        _frame.Dispose();
                        _frame = null;
                    }
                    if (_listView != null) {
                        _listView.Dispose();
                        _listView = null;
                    }
                    foreach (IObjectSpace createdObjectSpace in _createdObjectSpaces) {
                        if (!createdObjectSpace.IsDisposed) {
                            createdObjectSpace.Dispose();
                        }
                    }
                    _createdObjectSpaces.Clear();
                    _newObject = null;
                    _newObjectSpace = null;
                }
            } finally {
                base.Dispose(disposing);
            }
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            if (_searchDropDownEdit != null) {
                _searchDropDownEdit.DropDown.SelectedIndexChanged -= dropDownLookup_SelectedIndexChanged;
                _searchDropDownEdit.Init -= dropDownLookup_Init;
                _searchDropDownEdit.PreRender -= dropDownLookup_PreRender;
                _searchDropDownEdit.Callback -= result_Callback;
            }
            if (!unwireEventsOnly) {
                _searchDropDownEdit = null;
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        public void SetControlValue(object val) {
            object selectedObject = GetControlValueCore();
            if (((selectedObject == null && val == null) || (selectedObject != val)) && (CurrentObject != null)) {
                OnValueStoring(_helper.GetDisplayText(val, EmptyValue, DisplayFormat));
                MemberInfo.SetValue(CurrentObject, _helper.ObjectSpace.GetObject(val));
                OnValueStored();
                ReadValue();
            }
        }

        public override void Setup(IObjectSpace space, XafApplication xafApplication) {
            base.Setup(space, xafApplication);
            _application = application;
            _objectSpace = objectSpace;
            _helper = new WebLookupEditorHelper(xafApplication, space, MemberInfo.MemberTypeInfo, Model);
        }

        event EventHandler<EventArgs> viewShowingNotification;

        internal string GetSearchActionName() {
            return Frame.GetController<FilterController>().FullTextFilterAction.Caption;
        }
    }

    public class ASPxSearchDropDownEdit : Table, INamingContainer, ICallbackEventHandler {
        const int NumberCharSearch = 1;
        readonly EditButton _clearButton;
        readonly ASPxComboBox _dropDown;
        readonly EditButton _newButton;
        bool _addingEnabled;
        string _clearButtonScript;
        bool _isPrerendered;
        string _newButtonScript;

        public ASPxSearchDropDownEdit(WebLookupEditorHelper helper, string emptyValue, string displayFormat) {
            Helper = helper;
            EmptyValue = emptyValue;
            DisplayFormat = displayFormat;

            _dropDown = RenderHelper.CreateASPxComboBox();
            _dropDown.ID = "DD";
            _dropDown.Width = Unit.Percentage(100);
            _dropDown.CssClass = "xafLookupEditor";

            // the following properties would be nice to be read from the model
            _dropDown.IncrementalFilteringMode = IncrementalFilteringMode.StartsWith;
            _dropDown.FilterMinLength = 3;

            _dropDown.DropDownButton.Visible = false;
            _dropDown.EnableCallbackMode = true;
            _dropDown.CallbackPageSize = 10;
            _dropDown.ItemRequestedByValue += dropDown_ItemRequestedByValue;
            _dropDown.ItemsRequestedByFilterCondition += dropDown_ItemsRequestedByFilterCondition;

            _dropDown.TextField = Helper.DisplayMember.Name;
            _dropDown.ValueField = Helper.LookupObjectTypeInfo.KeyMember.Name;
            _dropDown.Columns.Add(Helper.LookupObjectTypeInfo.DefaultMember.BindingName);
            /*if (Helper.LookupObjectTypeInfo.Type.FullName == "MainDemo.Module.BusinessObjects.Contact")
            {
                dropDown.Columns.Add("FullName", "FullName", 300);
                dropDown.Columns.Add("SpouseName", "SpouseName", 300);
                dropDown.TextFormatString = "{0} {1}";
            }*/

            _newButton = _dropDown.Buttons.Add();
            _clearButton = _dropDown.Buttons.Add();
            ASPxImageHelper.SetImageProperties(_newButton.Image, "Action_New_12x12");
            ASPxImageHelper.SetImageProperties(_clearButton.Image, "Editor_Clear");

            InitTable();
        }

        void InitTable() {
            CellPadding = 0;
            CellSpacing = 0;
            Rows.Add(new TableRow());
            Rows[0].Cells.Add(new TableCell());

            Rows[0].Cells[0].Width = Unit.Percentage(100);
            Rows[0].Cells[0].Attributes["align"] = "left";
            Rows[0].Cells[0].Attributes["valign"] = "middle";
            Rows[0].Cells[0].Controls.Add(_dropDown);
        }


        public ASPxComboBox DropDown {
            get { return _dropDown; }
        }

        public bool ReadOnly {
            get { return _dropDown.ReadOnly; }
            set {
                _dropDown.ReadOnly = value;
                _dropDown.Enabled = !value;
            }
        }

        public BaseObjectSpace ObjectSpace {
            get { return (BaseObjectSpace)Helper.ObjectSpace; }
        }

        public WebLookupEditorHelper Helper { get; set; }
        public string EmptyValue { get; set; }
        public string DisplayFormat { get; set; }

        public bool AddingEnabled {
            get { return _addingEnabled; }
            set {
                _addingEnabled = value;
                if (_newButton != null) {
                    _newButton.Enabled = value;
                    _newButton.Visible = value;
                }
            }
        }

        public string NewActionCaption {
            get { return _newButton.Text; }
            set {
                _newButton.ToolTip = value;
                if (_newButton.Image.IsEmpty) {
                    _newButton.Text = value;
                }
            }
        }
        #region ICallbackEventHandler Members
        public string GetCallbackResult() {
            var result = new StringBuilder();
            foreach (ListEditItem item in _dropDown.Items) {
                result.AppendFormat("{0}<{1}{2}|", HttpUtility.HtmlAttributeEncode(item.Text), item.Value,
                                    _dropDown.SelectedItem == item ? "<" : "");
            }
            if (result.Length > 0) {
                result.Remove(result.Length - 1, 1);
            }
            return string.Format("{0}><{1}", _dropDown.ClientID, result);
        }

        public void RaiseCallbackEvent(string eventArgument) {
            if (Callback != null) {
                Callback(this, new CallbackEventArgs(eventArgument));
            }
        }
        #endregion
        void UpdateClientButtonsScript() {
            _dropDown.ClientSideEvents.ButtonClick = @"function(s, e) {
                if(e.buttonIndex == 0) {" +
                                                    _newButtonScript +
                                                    @"}
                if(e.buttonIndex == 1) {" +
                                                    _clearButtonScript +
                                                    @"}             
            }";
        }

        void dropDown_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e) {
            if (String.IsNullOrEmpty(e.Filter) || e.Filter.Length < NumberCharSearch)
                return;
            var editor = ((ASPxComboBox)source);
            editor.DataSource = GetLookupSource(e.Filter);
            editor.DataBind();
        }

        void dropDown_ItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e) {
        }

        public IList GetLookupSource(string filter) {
            var criteriaBuilder = new SearchCriteriaBuilder {
                TypeInfo = Helper.LookupObjectTypeInfo,
                SearchInStringPropertiesOnly = false,
                SearchText = filter,
                SearchMode = SearchMode.SearchInObject
            };
            criteriaBuilder.SetSearchProperties(Helper.LookupObjectTypeInfo.DefaultMember.BindingName);
            return ObjectSpace.GetObjects(Helper.LookupObjectType, criteriaBuilder.BuildCriteria());
        }

        protected override void OnPreRender(EventArgs e) {
            _isPrerendered = true;
            if (!ReadOnly) {
                _clearButtonScript =
                    @"var processOnServer = false;
						var dropDownControl = aspxGetControlCollection().Get('" +
                    _dropDown.ClientID +
                    @"');
						if(dropDownControl) {
							dropDownControl.ClearItems();                            
							processOnServer = dropDownControl.RaiseValueChangedEvent();
						}
						e.processOnServer = processOnServer;";
                UpdateClientButtonsScript();
            } else {
                _clearButton.Visible = false;
                _newButton.Visible = false;
            }
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer) {
            if (!_isPrerendered) {
                OnPreRender(EventArgs.Empty);
            }
            base.Render(writer);
        }

        public void SetClientNewButtonScript(string value) {
            _newButtonScript = value;
            UpdateClientButtonsScript();
        }

        public string GetProcessNewObjFunction() {
            return "xafDropDownLookupProcessNewObject('" + UniqueID + "')";
        }

        public event EventHandler<CallbackEventArgs> Callback;
    }
}