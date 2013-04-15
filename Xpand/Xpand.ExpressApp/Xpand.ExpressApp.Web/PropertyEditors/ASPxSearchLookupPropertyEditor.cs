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
using DevExpress.Web.ASPxEditors;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(object), false)]
    public class ASPxSearchLookupPropertyEditor : ASPxObjectPropertyEditorBase, IDependentPropertyEditor,
                                                  ISupportViewShowing, IFrameContainer {
        static int windowWidth = 800;
        static int windowHeight = 480;
        readonly List<IObjectSpace> createdObjectSpaces = new List<IObjectSpace>();
        string editorId;
        internal NestedFrame frame;
        WebLookupEditorHelper helper;
        ListView listView;
        object newObject;
        IObjectSpace newObjectSpace;
        NewObjectViewController newObjectViewController;
        PopupWindowShowAction newObjectWindowAction;
        ASPxSearchDropDownEdit searchDropDownEdit;
        WebApplication _application;
        IObjectSpace _objectSpace;

        public ASPxSearchLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            skipEditModeDataBind = true;
        }

        public WebLookupEditorHelper WebLookupEditorHelper {
            get { return helper; }
        }

        protected CollectionSourceBase DataSource {
            get {
                if (listView != null) {
                    return listView.CollectionSource;
                }
                return null;
            }
        }

        public static int WindowWidth {
            get { return windowWidth; }
            set { windowWidth = value; }
        }

        public static int WindowHeight {
            get { return windowHeight; }
            set { windowHeight = value; }
        }

        internal LookupEditorHelper Helper {
            get { return helper; }
        }
        #region IDependentPropertyEditor Members
        IList<string> IDependentPropertyEditor.MasterProperties {
            get { return helper.MasterProperties; }
        }
        #endregion
        #region IFrameContainer Members
        public Frame Frame {
            get {
                InitializeFrame();
                return frame;
            }
        }

        public void InitializeFrame() {
            if (frame == null) {
                frame = helper.Application.CreateNestedFrame(this, TemplateContext.LookupControl);
                newObjectViewController = frame.GetController<NewObjectViewController>();
                if (newObjectViewController != null) {
                    newObjectViewController.ObjectCreating += newObjectViewController_ObjectCreating;
                    newObjectViewController.ObjectCreated += newObjectViewController_ObjectCreated;
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
            searchDropDownEdit.DropDown.ClientSideEvents.SelectedIndexChanged = script;
        }

        protected override void SetImmediatePostDataCompanionScript(string script) {
            searchDropDownEdit.DropDown.SetClientSideEventHandler("GotFocus", script);
        }

        void UpdateDropDownLookupControlAddButton(ASPxSearchDropDownEdit control) {
            control.AddingEnabled = false;
            if (CurrentObject != null) {
                string diagnosticInfo;
                RecreateListView(true);
                control.AddingEnabled = AllowEdit &&
                                        DataManipulationRight.CanCreate(listView, helper.LookupObjectType,
                                                                        listView.CollectionSource, out diagnosticInfo);
                if (control.AddingEnabled) {
                    if (newObjectViewController != null) {
                        control.AddingEnabled = newObjectViewController.NewObjectAction.Active &&
                                                newObjectViewController.NewObjectAction.Enabled;
                    }
                }
            }
        }

        ASPxSearchDropDownEdit CreateSearchDropDownEditControl() {
            var result = new ASPxSearchDropDownEdit(helper, EmptyValue, DisplayFormat) { Width = Unit.Percentage(100) };
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
                if (newObjectViewController != null) {
                    supportNewObjectCreating.NewActionCaption = newObjectViewController.NewObjectAction.Caption;
                }
                UpdateDropDownLookupControlAddButton(supportNewObjectCreating);
                if (_application != null) {
                    supportNewObjectCreating.SetClientNewButtonScript(
                        _application.PopupWindowManager.GenerateModalOpeningScript(editor, newObjectWindowAction,
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
            return helper.GetObjectByKey(CurrentObject, key);
        }

        void newObjectViewController_ObjectCreating(object sender, ObjectCreatingEventArgs e) {
            e.ShowDetailView = false;
            // B196715
            if (e.ObjectSpace is INestedObjectSpace) {
                e.ObjectSpace = _application.CreateObjectSpace();
            }
        }

        void newObjectViewController_ObjectCreated(object sender, ObjectCreatedEventArgs e) {
            newObject = e.CreatedObject;
            newObjectSpace = e.ObjectSpace;
            createdObjectSpaces.Add(newObjectSpace);
        }

        void newObjectWindowAction_OnCustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs args) {
            if (!DataSource.AllowAdd) {
                throw new InvalidOperationException();
            }
            if (newObjectViewController != null) {
                //TODO MINAKOV rewrite
                OnViewShowingNotification(); //CaptionHelper.GetLocalizedText("DialogButtons", "Add"));
                newObjectViewController.NewObjectAction.DoExecute(newObjectViewController.NewObjectAction.Items[0]);
                args.View = _application.CreateDetailView(newObjectSpace, newObject, listView);
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
            DataSource.Add(helper.ObjectSpace.GetObject(detailView.CurrentObject));
            ((PopupWindow)args.PopupWindow).ClosureScript =
                "if(window.dialogOpener != null) window.dialogOpener.ddLookupResult = '" +
                detailView.ObjectSpace.GetKeyValueAsString(detailView.CurrentObject) + "';";
        }

        void RecreateListView(bool ifNotCreatedOnly) {
            if (ViewEditMode == ViewEditMode.Edit && (!ifNotCreatedOnly || listView == null)) {
                listView = null;
                if (CurrentObject != null) {
                    listView = helper.CreateListView(CurrentObject);
                }
                Frame.SetView(listView);
            }
        }

        void OnViewShowingNotification() {
            if (viewShowingNotification != null) {
                viewShowingNotification(this, EventArgs.Empty);
            }
        }

        protected override void ApplyReadOnly() {
            if (searchDropDownEdit != null) {
                searchDropDownEdit.ReadOnly = !AllowEdit;
            }
        }

        protected override WebControl CreateEditModeControlCore() {
            if (newObjectWindowAction == null) {
                newObjectWindowAction = new PopupWindowShowAction(null, "New", PredefinedCategory.Unspecified.ToString());
                newObjectWindowAction.Execute += newObjectWindowAction_OnExecute;
                newObjectWindowAction.CustomizePopupWindowParams += newObjectWindowAction_OnCustomizePopupWindowParams;
                newObjectWindowAction.Application = helper.Application;
            }

            var panel = new Panel();
            // Use Panel instead of ASPxPanel cause it doesn't affect editor ClientID            
            searchDropDownEdit = CreateSearchDropDownEditControl();
            panel.Controls.Add(searchDropDownEdit);
            return panel;
        }

        protected override object GetControlValueCore() {
            if (ViewEditMode == ViewEditMode.Edit && Editor != null) {
                ASPxComboBox dropDownControl = searchDropDownEdit.DropDown;
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
            UpdateDropDownLookup(searchDropDownEdit);
        }

        protected override string GetPropertyDisplayValue() {
            return helper.GetDisplayText(MemberInfo.GetValue(CurrentObject), EmptyValue, DisplayFormat);
        }

        void FillSearchDropDownValues(object item) {
            searchDropDownEdit.DropDown.Items.Clear();
            if (item != null) {
                searchDropDownEdit.DropDown.DataSource = new List<object>(new[] { item });
                searchDropDownEdit.DropDown.DataBind();
            }
            searchDropDownEdit.DropDown.SelectedIndex = searchDropDownEdit.DropDown.Items.Count - 1;
        }

        protected override void ReadEditModeValueCore() {
            if (searchDropDownEdit != null) {
                FillSearchDropDownValues(PropertyValue);
            }
        }

        public void SetValueToControl(object obj) {
            if (searchDropDownEdit != null) {
                ASPxComboBox controlBox = searchDropDownEdit.DropDown;
                foreach (ListEditItem item in controlBox.Items) {
                    var val = item.Value as string;
                    if (val == helper.GetObjectKey(obj)) {
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
            if (searchDropDownEdit != null) {
                return searchDropDownEdit.DropDown;
            }
            return base.GetActiveControl();
        }

        protected override string GetEditorClientId() {
            return searchDropDownEdit.ClientID;
        }

        void UpdateControlId() {
            searchDropDownEdit.ID = editorId;
        }

        protected override void SetEditorId(string controlId) {
            editorId = controlId;
            UpdateControlId();
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) {
                    if (newObjectWindowAction != null) {
                        newObjectWindowAction.Execute -= newObjectWindowAction_OnExecute;
                        newObjectWindowAction.CustomizePopupWindowParams -=
                            newObjectWindowAction_OnCustomizePopupWindowParams;
                        DisposeAction(newObjectWindowAction);
                        newObjectWindowAction = null;
                    }
                    if (newObjectViewController != null) {
                        newObjectViewController.ObjectCreating -= newObjectViewController_ObjectCreating;
                        newObjectViewController.ObjectCreated -= newObjectViewController_ObjectCreated;
                        newObjectViewController = null;
                    }
                    if (frame != null) {
                        frame.SetView(null);
                        frame.Dispose();
                        frame = null;
                    }
                    if (listView != null) {
                        listView.Dispose();
                        listView = null;
                    }
                    foreach (IObjectSpace createdObjectSpace in createdObjectSpaces) {
                        if (!createdObjectSpace.IsDisposed) {
                            createdObjectSpace.Dispose();
                        }
                    }
                    createdObjectSpaces.Clear();
                    newObject = null;
                    newObjectSpace = null;
                }
            } finally {
                base.Dispose(disposing);
            }
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            if (searchDropDownEdit != null) {
                searchDropDownEdit.DropDown.SelectedIndexChanged -= dropDownLookup_SelectedIndexChanged;
                searchDropDownEdit.Init -= dropDownLookup_Init;
                searchDropDownEdit.PreRender -= dropDownLookup_PreRender;
                searchDropDownEdit.Callback -= result_Callback;
            }
            if (!unwireEventsOnly) {
                searchDropDownEdit = null;
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        public void SetControlValue(object val) {
            object selectedObject = GetControlValueCore();
            if (((selectedObject == null && val == null) || (selectedObject != val)) && (CurrentObject != null)) {
                OnValueStoring(helper.GetDisplayText(val, EmptyValue, DisplayFormat));
                MemberInfo.SetValue(CurrentObject, helper.ObjectSpace.GetObject(val));
                OnValueStored();
                ReadValue();
            }
        }

        public override void Setup(IObjectSpace space, XafApplication xafApplication) {
            base.Setup(space, xafApplication);
            _application = application;
            _objectSpace = objectSpace;
            helper = new WebLookupEditorHelper(xafApplication, space, MemberInfo.MemberTypeInfo, Model);
        }

        event EventHandler<EventArgs> viewShowingNotification;

        internal string GetSearchActionName() {
            return Frame.GetController<FilterController>().FullTextFilterAction.Caption;
        }
    }

    public class ASPxSearchDropDownEdit : Table, INamingContainer, ICallbackEventHandler {
        const int NUMBER_CHAR_SEARCH = 1;
        readonly EditButton clearButton;
        readonly ASPxComboBox dropDown;
        readonly EditButton newButton;
        bool addingEnabled;
        string clearButtonScript;
        bool isPrerendered;
        string newButtonScript;

        public ASPxSearchDropDownEdit(WebLookupEditorHelper helper, string emptyValue, string displayFormat) {
            Helper = helper;
            EmptyValue = emptyValue;
            DisplayFormat = displayFormat;

            dropDown = RenderHelper.CreateASPxComboBox();
            dropDown.ID = "DD";
            dropDown.Width = Unit.Percentage(100);
            dropDown.CssClass = "xafLookupEditor";

            // the following properties would be nice to be read from the model
            dropDown.IncrementalFilteringMode = IncrementalFilteringMode.StartsWith;
            dropDown.FilterMinLength = 3;

            dropDown.DropDownButton.Visible = false;
            dropDown.EnableCallbackMode = true;
            dropDown.CallbackPageSize = 10;
            dropDown.ItemRequestedByValue += dropDown_ItemRequestedByValue;
            dropDown.ItemsRequestedByFilterCondition += dropDown_ItemsRequestedByFilterCondition;

            dropDown.TextField = Helper.DisplayMember.Name;
            dropDown.ValueField = Helper.LookupObjectTypeInfo.KeyMember.Name;
            dropDown.Columns.Add(Helper.LookupObjectTypeInfo.DefaultMember.BindingName);
            /*if (Helper.LookupObjectTypeInfo.Type.FullName == "MainDemo.Module.BusinessObjects.Contact")
            {
                dropDown.Columns.Add("FullName", "FullName", 300);
                dropDown.Columns.Add("SpouseName", "SpouseName", 300);
                dropDown.TextFormatString = "{0} {1}";
            }*/

            newButton = dropDown.Buttons.Add();
            clearButton = dropDown.Buttons.Add();
            ASPxImageHelper.SetImageProperties(newButton.Image, "Action_New_12x12");
            ASPxImageHelper.SetImageProperties(clearButton.Image, "Editor_Clear");

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
            Rows[0].Cells[0].Controls.Add(dropDown);
        }


        public ASPxComboBox DropDown {
            get { return dropDown; }
        }

        public bool ReadOnly {
            get { return dropDown.ReadOnly; }
            set {
                dropDown.ReadOnly = value;
                dropDown.Enabled = !value;
            }
        }

        public BaseObjectSpace ObjectSpace {
            get { return (BaseObjectSpace)Helper.ObjectSpace; }
        }

        public WebLookupEditorHelper Helper { get; set; }
        public string EmptyValue { get; set; }
        public string DisplayFormat { get; set; }

        public bool AddingEnabled {
            get { return addingEnabled; }
            set {
                addingEnabled = value;
                if (newButton != null) {
                    newButton.Enabled = value;
                    newButton.Visible = value;
                }
            }
        }

        public string NewActionCaption {
            get { return newButton.Text; }
            set {
                newButton.ToolTip = value;
                if (newButton.Image.IsEmpty) {
                    newButton.Text = value;
                }
            }
        }
        #region ICallbackEventHandler Members
        public string GetCallbackResult() {
            var result = new StringBuilder();
            foreach (ListEditItem item in dropDown.Items) {
                result.AppendFormat("{0}<{1}{2}|", HttpUtility.HtmlAttributeEncode(item.Text), item.Value,
                                    dropDown.SelectedItem == item ? "<" : "");
            }
            if (result.Length > 0) {
                result.Remove(result.Length - 1, 1);
            }
            return string.Format("{0}><{1}", dropDown.ClientID, result);
        }

        public void RaiseCallbackEvent(string eventArgument) {
            if (Callback != null) {
                Callback(this, new CallbackEventArgs(eventArgument));
            }
        }
        #endregion
        void UpdateClientButtonsScript() {
            dropDown.ClientSideEvents.ButtonClick = @"function(s, e) {
                if(e.buttonIndex == 0) {" +
                                                    newButtonScript +
                                                    @"}
                if(e.buttonIndex == 1) {" +
                                                    clearButtonScript +
                                                    @"}             
            }";
        }

        void dropDown_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e) {
            if (String.IsNullOrEmpty(e.Filter) || e.Filter.Length < NUMBER_CHAR_SEARCH)
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
            isPrerendered = true;
            if (!ReadOnly) {
                clearButtonScript =
                    @"var processOnServer = false;
						var dropDownControl = aspxGetControlCollection().Get('" +
                    dropDown.ClientID +
                    @"');
						if(dropDownControl) {
							dropDownControl.ClearItems();                            
							processOnServer = dropDownControl.RaiseValueChangedEvent();
						}
						e.processOnServer = processOnServer;";
                UpdateClientButtonsScript();
            } else {
                clearButton.Visible = false;
                newButton.Visible = false;
            }
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer) {
            if (!isPrerendered) {
                OnPreRender(EventArgs.Empty);
            }
            base.Render(writer);
        }

        public void SetClientNewButtonScript(string value) {
            newButtonScript = value;
            UpdateClientButtonsScript();
        }

        public string GetProcessNewObjFunction() {
            return "xafDropDownLookupProcessNewObject('" + UniqueID + "')";
        }

        public event EventHandler<CallbackEventArgs> Callback;
    }
}