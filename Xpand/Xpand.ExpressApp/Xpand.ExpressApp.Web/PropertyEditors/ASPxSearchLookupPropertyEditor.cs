using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using Xpand.Extensions.TypeExtensions;
using Xpand.Utils.Helpers;
using CallbackEventArgs = DevExpress.ExpressApp.Web.Editors.ASPx.CallbackEventArgs;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;
using PopupWindow = DevExpress.ExpressApp.Web.PopupWindow;

namespace Xpand.ExpressApp.Web.PropertyEditors{
    [PropertyEditor(typeof(object), EditorAliases.ASPxSearchLookupPropertyEditor, false)]
    public class ASPxSearchLookupPropertyEditor : ASPxObjectPropertyEditorBase, IDependentPropertyEditor,
        ISupportViewShowing, IFrameContainer{
        private static int _windowWidth = 800;
        private static int _windowHeight = 480;
        private WebApplication _application;
        private string _editorId;
        private NestedFrame _frame;
        private ListView _listView;
        
        private PopupWindowShowAction _newObjectWindowAction;
        private IObjectSpace _objectSpace;
        private ASPxSearchDropDownEdit _searchDropDownEdit;
        private PopupWindowShowAction _showFindSelectWindowAction;

        public ASPxSearchLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model){
            NullText = "";
        }

        public WebLookupEditorHelper WebLookupEditorHelper { get; private set; }

        protected CollectionSourceBase DataSource => _listView?.CollectionSource;

        public int WindowWidth{
            get => _windowWidth;
            set => _windowWidth = value;
        }

        public int WindowHeight{
            get => _windowHeight;
            set => _windowHeight = value;
        }

        internal WebLookupEditorHelper Helper => WebLookupEditorHelper;

        #region IDependentPropertyEditor Members

        IList<string> IDependentPropertyEditor.MasterProperties => WebLookupEditorHelper.MasterProperties;

        #endregion

        #region ISupportViewShowing Members

        event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification{
            add => ViewShowingNotification += value;
            remove => ViewShowingNotification -= value;
        }

        #endregion

        protected override void SetImmediatePostDataScript(string script){
            _searchDropDownEdit.DropDown.ClientSideEvents.SelectedIndexChanged = script;
        }

        protected override void SetImmediatePostDataCompanionScript(string script){
            _searchDropDownEdit.DropDown.SetClientSideEventHandler("GotFocus", script);
        }

        private void UpdateDropDownLookupControlAddButton(ASPxSearchDropDownEdit control){
            control.AddingEnabled = false;
            if (CurrentObject != null){
                RecreateListView(true);
                control.AddingEnabled = AllowEdit &&
                                        DataManipulationRight.CanCreate(_listView,
                                            WebLookupEditorHelper.LookupObjectType,
                                            _listView.CollectionSource, out _);
                if (control.AddingEnabled)
                    control.AddingEnabled = _listView.AllowNew;
            }
        }

        private void action_FindWindowParamsCustomizing(object sender, CustomizePopupWindowParamsEventArgs args){
            OnViewShowingNotification();
            args.View = WebLookupEditorHelper.CreateListView(CurrentObject);
            var controller = WebLookupEditorHelper.Application.CreateController<FindLookupDialogController>();
            controller.Initialize(WebLookupEditorHelper);
            args.DialogController = controller;
        }

        private void UpdateFindButtonScript(ASPxSearchDropDownEdit findEdit, PopupWindowManager popupWindowManager){
            if ((findEdit != null) && (popupWindowManager != null)){
                var immediatePostDataScript = Model.ImmediatePostData && (WebWindow.CurrentRequestPage is ICallbackManagerHolder callbackManagerHolder)
                    ? callbackManagerHolder.CallbackManager.GetScript().Replace("'", @"\\\\\\""")
                    : string.Empty;
                var processFindResultFunc = "xafFindLookupProcessFindObject('" + findEdit.UniqueID + "', '" +
                                            findEdit.Hidden.ClientID + "', window.findLookupResult, '" +
                                            immediatePostDataScript + "');";
                var showInFindPopup = DeviceDetector.Instance.GetDeviceCategory() == DeviceCategory.Desktop;
                var showPopupWindowScript = popupWindowManager.GetShowPopupWindowScript(_showFindSelectWindowAction,
                    HttpUtility.JavaScriptStringEncode(processFindResultFunc), findEdit.ClientID, false, true, true,
                    showInFindPopup,
                    "function() { window.buttonEditAlreadyClicked = false; window.canInitiateImmediatePostData = true;}");
                findEdit.SetFindButtonClientScript(showPopupWindowScript);
            }
        }

        private ASPxSearchDropDownEdit CreateSearchDropDownEditControl(){
            if (_showFindSelectWindowAction == null){
                _showFindSelectWindowAction = new PopupWindowShowAction(null,
                    MemberInfo.Name + "_ASPxLookupEditor_ShowFindWindow", PredefinedCategory.Unspecified);
                _showFindSelectWindowAction.Execute += actionFind_OnExecute;
                _showFindSelectWindowAction.CustomizePopupWindowParams += action_FindWindowParamsCustomizing;
                _showFindSelectWindowAction.Application = application;
                _showFindSelectWindowAction.AcceptButtonCaption = string.Empty;
                _showFindSelectWindowAction.Data[EditorActionRelationKey] = this;
            }
            var searchDropDownEdit = new ASPxSearchDropDownEdit{Width = Unit.Percentage(100)};
            searchDropDownEdit.DropDown.SelectedIndexChanged += DropDownOnSelectedIndexChanged;

            searchDropDownEdit.Init += DropDownEditOnInit;
            searchDropDownEdit.PreRender += DropDownEditOnPreRender;
            searchDropDownEdit.Callback += DropDownEditOnCallback;
            searchDropDownEdit.ValueChanged += DropDownEditOnValueChanged;
            searchDropDownEdit.Setup(Helper);
            searchDropDownEdit.ReadOnly = !AllowEdit;
            DisplayFormat = DisplayFormat;
            UpdateDropDownLookup(searchDropDownEdit);
            return searchDropDownEdit;
        }


        private void actionFind_OnExecute(object sender, PopupWindowShowActionExecuteEventArgs args){
            var objectKey = WebLookupEditorHelper.GetObjectKey(((ListView) args.PopupWindow.View).CurrentObject);
            ((PopupWindow) args.PopupWindow).ClosureScript =
                "if(window.dialogOpener) window.dialogOpener.findLookupResult = '" + EscapeObjectKey(objectKey) + "';";
        }

        private string EscapeObjectKey(string key){
            return key.Replace("'", "\\'");
        }

        private void DropDownEditOnCallback(object sender, CallbackEventArgs e){
            var argument = e.Argument;
            if (argument == "clear") {
                _searchDropDownEdit.DropDown.Text = Helper.GetDisplayText(null, NullText, DisplayFormat);
            }
            else if (argument.StartsWith("found")){
                argument = argument.Substring(5);
                object editValue = GetObjectByKey(argument);
                _searchDropDownEdit.DropDown.Text = Helper.GetDisplayText(editValue, NullText, DisplayFormat);
                _searchDropDownEdit.DropDown.JSProperties[ASPxSearchDropDownEdit.CpLookup] = editValue == null;
                _searchDropDownEdit.DropDown.JSProperties[ASPxSearchDropDownEdit.CpIsEmpty] = editValue == null;
                FillSearchDropDownValues(editValue);
                return;
            }
            FillSearchDropDownValues(GetObjectByKey($"{Helper.LookupObjectTypeInfo}({argument})"));
        }

        private void UpdateDropDownLookup(WebControl editor){
            if (editor is ASPxSearchDropDownEdit dropDownEdit){
                
                dropDownEdit.NewActionCaption = Model.Application.ActionDesign.Actions["New"].Caption;
                UpdateDropDownLookupControlAddButton(dropDownEdit);
                if (_application != null){
                    var callBackFuncName = HttpUtility.JavaScriptStringEncode(dropDownEdit.GetProcessNewObjFunction());
                    var script = application.PopupWindowManager.GetShowPopupWindowScript(_newObjectWindowAction,
                        callBackFuncName, editor.ClientID, false, _newObjectWindowAction.IsSizeable);
                    dropDownEdit.SetNewButtonScript(script);

                    UpdateFindButtonScript(dropDownEdit, application.PopupWindowManager);
                }
            }
        }
        
        private void DropDownOnSelectedIndexChanged(object source, EventArgs e){
            EditValueChangedHandler(source, EventArgs.Empty);
        }

        private void DropDownEditOnInit(object sender, EventArgs e){
            UpdateDropDownLookup((WebControl) sender);
        }

        private void DropDownEditOnPreRender(object sender, EventArgs e){
            UpdateDropDownLookup((WebControl) sender);
        }

        private object GetObjectByKey(string key){
            return WebLookupEditorHelper.GetObjectByKey(CurrentObject, key);
        }



        private void newObjectWindowAction_OnCustomizePopupWindowParams(object sender,
            CustomizePopupWindowParamsEventArgs args){
            if (!DataSource.AllowAdd) throw new InvalidOperationException();
            OnViewShowingNotification();
            var nestedObjectSpace = _application.CreateObjectSpace(Helper.LookupObjectType);
            var newObject = nestedObjectSpace.CreateObject(Helper.LookupObjectType);
            args.View = _application.CreateDetailView(nestedObjectSpace, newObject, _listView);
        }

        private void newObjectWindowAction_OnExecute(object sender, PopupWindowShowActionExecuteEventArgs args){
            if (!DataSource.AllowAdd) throw new InvalidOperationException();
            if (_objectSpace != args.PopupWindow.View.ObjectSpace) args.PopupWindow.View.ObjectSpace.CommitChanges();
            var detailView = (DetailView) args.PopupWindow.View;
            DataSource.Add(WebLookupEditorHelper.ObjectSpace.GetObject(detailView.CurrentObject));
            ((PopupWindow) args.PopupWindow).ClosureScript =
                "if(window.dialogOpener != null) window.dialogOpener.ddLookupResult = '" +
                detailView.ObjectSpace.GetKeyValueAsString(detailView.CurrentObject) + "';";
        }

        private void RecreateListView(bool ifNotCreatedOnly){
            if ((ViewEditMode == ViewEditMode.Edit) && (!ifNotCreatedOnly || (_listView == null))){
                _listView = null;
                if (CurrentObject != null) _listView = WebLookupEditorHelper.CreateListView(CurrentObject);
            }
        }

        private void OnViewShowingNotification(){
            ViewShowingNotification?.Invoke(this, EventArgs.Empty);
        }

        protected override void ApplyReadOnly(){
            if (_searchDropDownEdit != null) _searchDropDownEdit.ReadOnly = !AllowEdit;
        }

        public ASPxSearchDropDownEdit SearchDropDownEdit => _searchDropDownEdit;

        protected override WebControl CreateEditModeControlCore(){
            if (_newObjectWindowAction == null){
                _newObjectWindowAction = new PopupWindowShowAction(null, "New",
                    PredefinedCategory.Unspecified.ToString());
                _newObjectWindowAction.Execute += newObjectWindowAction_OnExecute;
                _newObjectWindowAction.CustomizePopupWindowParams += newObjectWindowAction_OnCustomizePopupWindowParams;
                _newObjectWindowAction.Application = WebLookupEditorHelper.Application;
            }

            
            _searchDropDownEdit = CreateSearchDropDownEditControl();
            
            return _searchDropDownEdit;
        }

        protected override object GetControlValueCore(){
            if ((ViewEditMode == ViewEditMode.Edit) && (Editor != null)){
                if (!string.IsNullOrEmpty(_searchDropDownEdit.Value))
                    return GetObjectByKey(_searchDropDownEdit.Value);
                var dropDownControl = _searchDropDownEdit.DropDown;
                if (dropDownControl.Value != null && (dropDownControl.Value.CanChange(Helper.LookupObjectTypeInfo.KeyMember.MemberType) && ((string) dropDownControl.Value != NullText))){
                    var objectKey = GetObjectKey(dropDownControl);
                    return GetObjectByKey(objectKey);
                }
                return null;
            }
            return MemberInfo.GetValue(CurrentObject);
        }

        private string GetObjectKey(ASPxComboBox dropDownControl){
            return
                (string)
                (Helper.LookupObjectTypeInfo.IsPersistent
                    ? $"{Helper.LookupObjectTypeInfo}({dropDownControl.Value})"
                    : dropDownControl.Value);
        }

        protected override void OnCurrentObjectChanged(){
            if (Editor != null) RecreateListView(false);
            base.OnCurrentObjectChanged();
            UpdateDropDownLookup(_searchDropDownEdit);
        }

        protected override string GetPropertyDisplayValue(){
            return WebLookupEditorHelper.GetDisplayText(MemberInfo.GetValue(CurrentObject), CaptionHelper.NullValueText,
                DisplayFormat);
        }

        private void FillSearchDropDownValues(object item){
            _searchDropDownEdit.DropDown.Items.Clear();
            if (item != null){
                _searchDropDownEdit.DropDown.DataSource = new List<object>(new[]{item});
                _searchDropDownEdit.DropDown.DataBind();
            }
            _searchDropDownEdit.DropDown.SelectedIndex = _searchDropDownEdit.DropDown.Items.Count - 1;
        }

        protected override void ReadEditModeValueCore(){
            if (_searchDropDownEdit != null){
                FillSearchDropDownValues(PropertyValue);
                if (_searchDropDownEdit.DropDown.JSProperties.ContainsKey(ASPxSearchDropDownEdit.CpLookup)){
                    _searchDropDownEdit.Value = Helper.GetObjectKey(PropertyValue);
                    _searchDropDownEdit.DropDown.Text = GetPropertyDisplayValueForObject(PropertyValue);
                    _searchDropDownEdit.DropDown.JSProperties[ASPxSearchDropDownEdit.CpIsEmpty] = PropertyValue == null;
                }
            }
        }

        private string GetPropertyDisplayValueForObject(object propertyValue){
            return Helper.GetDisplayText(propertyValue, NullText, DisplayFormat);
        }

        public void SetValueToControl(object obj){
            if (_searchDropDownEdit != null){
                var controlBox = _searchDropDownEdit.DropDown;
                foreach (ListEditItem item in controlBox.Items){
                    var val = item.Value as string;
                    if (val == WebLookupEditorHelper.GetObjectKey(obj)){
                        controlBox.SelectedIndex = item.Index;
                        break;
                    }
                }
            }
        }

        protected override IJScriptTestControl GetInplaceViewModeEditorTestControlImpl(){
            return new JSButtonTestControl();
        }

        protected override WebControl GetActiveControl(){
            if (_searchDropDownEdit != null) return _searchDropDownEdit.DropDown;
            return base.GetActiveControl();
        }

        protected override string GetEditorClientId(){
            return _searchDropDownEdit.DropDown.ClientID;
        }


        private void UpdateControlId(){
            if (_searchDropDownEdit?.DropDown != null){
                _searchDropDownEdit.DropDown.ID = _editorId;
                if (EditorClientInfo != null && _searchDropDownEdit.DropDown != null){
                    var controlId = _searchDropDownEdit.DropDown.ID;
                    EditorClientInfo.ID = _editorId + "_" + controlId + "_EditorClientInfo";
                }
            }
        }

        protected override void SetEditorId(string controlId){
            _editorId = controlId;
            UpdateControlId();
        }

        protected override void Dispose(bool disposing){
            try{
                if (disposing){
                    if (_showFindSelectWindowAction != null){
                        _showFindSelectWindowAction.Execute -= actionFind_OnExecute;
                        _showFindSelectWindowAction.CustomizePopupWindowParams -= action_FindWindowParamsCustomizing;
                        DisposeAction(_showFindSelectWindowAction);
                        _showFindSelectWindowAction = null;
                    }
                    if (_newObjectWindowAction != null){
                        _newObjectWindowAction.Execute -= newObjectWindowAction_OnExecute;
                        _newObjectWindowAction.CustomizePopupWindowParams -=
                            newObjectWindowAction_OnCustomizePopupWindowParams;
                        DisposeAction(_newObjectWindowAction);
                        _newObjectWindowAction = null;
                    }
                    if (_frame != null){
                        _frame.SetView(null);
                        _frame.Dispose();
                        _frame = null;
                    }
                    if (_listView != null){
                        _listView.Dispose();
                        _listView = null;
                    }
                }
            }
            finally{
                base.Dispose(disposing);
            }
        }

        public override void BreakLinksToControl(bool unwireEventsOnly){
            if (_searchDropDownEdit != null){
                _searchDropDownEdit.DropDown.SelectedIndexChanged -= DropDownOnSelectedIndexChanged;
                _searchDropDownEdit.ValueChanged -= DropDownEditOnValueChanged;
                _searchDropDownEdit.Init -= DropDownEditOnInit;
                _searchDropDownEdit.PreRender -= DropDownEditOnPreRender;
                _searchDropDownEdit.Callback -= DropDownEditOnCallback;
            }
            if (!unwireEventsOnly) _searchDropDownEdit = null;
            base.BreakLinksToControl(unwireEventsOnly);
        }

        private void DropDownEditOnValueChanged(object sender, EventArgs eventArgs){
            EditValueChangedHandler(sender, eventArgs);
        }

        public void SetControlValue(object val){
            var selectedObject = GetControlValueCore();
            if ((((selectedObject == null) && (val == null)) || (selectedObject != val)) && (CurrentObject != null)){
                OnValueStoring(WebLookupEditorHelper.GetDisplayText(val, CaptionHelper.NullValueText, DisplayFormat));
                MemberInfo.SetValue(CurrentObject, WebLookupEditorHelper.ObjectSpace.GetObject(val));
                OnValueStored();
                ReadValue();
            }
        }

        public override void Setup(IObjectSpace space, XafApplication xafApplication){
            base.Setup(space, xafApplication);
            _application = application;
            _objectSpace = objectSpace;
            WebLookupEditorHelper = new WebLookupEditorHelper(xafApplication, space, MemberInfo.MemberTypeInfo, Model);
        }

        public event EventHandler<EventArgs> ViewShowingNotification;

        #region IFrameContainer Members

        public Frame Frame{
            get{
                InitializeFrame();
                return _frame;
            }
        }

        public void InitializeFrame(){
            if (_frame == null){
                _frame = WebLookupEditorHelper.Application.CreateNestedFrame(this, TemplateContext.LookupControl);
            }
        }

        #endregion
    }

    public sealed class ASPxSearchDropDownEdit : WebControl, ICallbackEventHandler, INamingContainer {
        private const int NumberCharSearch = 1;
        public const string CpLookup = "cpLookup";
        public const string CpIsEmpty = "cpIsEmpty";
        private readonly EditButton _clearButton;
        private readonly HiddenField _hidden;
        private readonly EditButton _newButton;
        private readonly EditButton _searchButton;
        private bool _addingEnabled;
        private string _clearButtonScript;
        private string _findButtonScript;
        private bool _isClearingHiddenValue;
        private bool _isPrerendered;
        private string _newButtonScript;

        
        public ComboBoxClientSideEvents ClientSideEvents => DropDown.ClientSideEvents;

        public ASPxSearchDropDownEdit(){
            
            DropDown = RenderHelper.CreateASPxComboBox();
            DropDown.ID = "DD";
            DropDown.Width = Unit.Percentage(100);
            DropDown.FilterMinLength = 2;
            EnableMultiColumnClientSideSelection();
            DropDown.DropDownButton.Visible = false;
            DropDown.EnableCallbackMode = true;
            DropDown.CallbackPageSize = 10;
            DropDown.ItemRequestedByValue += dropDown_ItemRequestedByValue;
            DropDown.ItemsRequestedByFilterCondition += dropDown_ItemsRequestedByFilterCondition;
            _newButton = DropDown.Buttons.Add();
            _clearButton = DropDown.Buttons.Add();
            _searchButton = DropDown.Buttons.Add();
            
            ASPxImageHelper.SetImageProperties(_newButton.Image, "Action_New_12x12");
            ASPxImageHelper.SetImageProperties(_clearButton.Image, "Editor_Clear");
            ASPxImageHelper.SetImageProperties(_searchButton.Image, "Editor_Search", 16, 16);
            Controls.Add(DropDown);
            _hidden = new LookupHiddenField{ID = "HDN"};
            _hidden.ValueChanged += hidden_ValueChanged;
            Controls.Add(_hidden);
            DropDown.DropDownStyle=DropDownStyle.DropDown;
        }

        private void EnableMultiColumnClientSideSelection(){
            DropDown.ClientSideEvents.Init = @"function (s, e) {            
                var actualOnBeforeCallbackFinally = s.filterStrategy.OnBeforeCallbackFinally;
                s.filterStrategy.OnBeforeCallbackFinally = function () {
                    var lb = this.GetListBoxControl();
                    var actualTextFormatString = lb.textFormatString;
                    actualOnBeforeCallbackFinally.apply(this);
                    for(var i=0; i < lb.columnFieldNames.length; i++){
                        lb.textFormatString = ""{""+i+""}"";
                        s.filterStrategy.RefreshHighlightInItems();
                    }
                    lb.textFormatString = actualTextFormatString;
                };
            }";
        }

        public Control Hidden => _hidden;

        public string Value{
            get => _hidden.Value;
            set => _hidden.Value = value;
        }

        public ASPxComboBox DropDown { get; }

        public bool ReadOnly{
            get => DropDown.ReadOnly;
            set{
                DropDown.ReadOnly = value;
                DropDown.Enabled = !value;
            }
        }

        public BaseObjectSpace ObjectSpace => (BaseObjectSpace) Helper.ObjectSpace;

        public WebLookupEditorHelper Helper { get; set; }

        public bool AddingEnabled{
            get => _addingEnabled;
            set{
                _addingEnabled = value;
                if (_newButton != null){
                    _newButton.Enabled = value;
                    _newButton.Visible = value;
                }
            }
        }

        public string NewActionCaption{
            get => _newButton.Text;
            set{
                _newButton.ToolTip = value;
                if (_newButton.Image.IsEmpty) _newButton.Text = value;
            }
        }

        private void hidden_ValueChanged(object sender, EventArgs e){
            if (!_isClearingHiddenValue){
                ValueChanged?.Invoke(this, EventArgs.Empty);
                _isClearingHiddenValue = true;
                _hidden.Value = null;
                _isClearingHiddenValue = false;
            }
        }

        public event EventHandler ValueChanged;

        public override void Dispose(){
            try{
                if (_hidden != null) _hidden.ValueChanged -= hidden_ValueChanged;
            }
            finally{
                base.Dispose();
            }
        }

        private void UpdateClientButtonsScript(){
            DropDown.ClientSideEvents.ButtonClick = @"function(s, e) {
                if(e.buttonIndex == 0) {" + _newButtonScript + @"};
                if(e.buttonIndex == 1) {" + _clearButtonScript + @"};
                if(e.buttonIndex == 2) {" + _findButtonScript + @"};
            }";
        }

        private void dropDown_ItemsRequestedByFilterCondition(object source,
            ListEditItemsRequestedByFilterConditionEventArgs e){
            if (string.IsNullOrEmpty(e.Filter) || (e.Filter.Length < NumberCharSearch))
                return;
            var editor = (ASPxComboBox) source;
            editor.DataSource = GetLookupSource(e.Filter);
            editor.DataBind();
        }

        private void dropDown_ItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e){
        }

        public IList GetLookupSource(string filter){
            var criteriaBuilder = new SearchCriteriaBuilder{
                TypeInfo = Helper.LookupObjectTypeInfo,
                SearchInStringPropertiesOnly = false,
                SearchText = filter,
                SearchMode = SearchMode.SearchInObject
            };
            criteriaBuilder.SetSearchProperties(Helper.LookupListViewModel.Columns.GetVisibleColumns().Select(column => column.ModelMember.MemberInfo.BindingName).ToArray());
            return ObjectSpace.GetObjects(Helper.LookupObjectType, criteriaBuilder.BuildCriteria());
        }

        public EditButton SearchButton => _searchButton;

        protected override void OnPreRender(EventArgs e){
            _isPrerendered = true;
            if (!ReadOnly){
                _clearButtonScript =
                    @"var processOnServer = false;
                    document.getElementById('" + Hidden.ClientID + @"').value = ''
					var dropDownControl = ASPxClientControl.GetControlCollection().GetByName('" + DropDown.ClientID + @"');
					if(dropDownControl) {
                        dropDownControl.SetValue('');
                        dropDownControl.cpIsEmpty = true;
						dropDownControl.ClearItems();                            
						processOnServer = dropDownControl.RaiseValueChangedEvent();
                        dropDownControl.Validate();
					}
					e.processOnServer = processOnServer;";
                UpdateClientButtonsScript();
            }
            else{
                _clearButton.Visible = false;
                _newButton.Visible = false;
                _searchButton.Visible = false;
            }
            
            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer){
            if (!_isPrerendered)
                OnPreRender(EventArgs.Empty);
            Page.ClientScript.RegisterForEventValidation(DropDown.ClientID);
            base.Render(writer);
        }

        public void SetNewButtonScript(string value){
            _newButtonScript = value;
            UpdateClientButtonsScript();
        }

        public string GetProcessNewObjFunction(){
            return "xafDropDownLookupProcessNewObject('" + UniqueID + "')";
        }

        public event EventHandler<CallbackEventArgs> Callback;

        public void SetFindButtonClientScript(string script){
            _findButtonScript = script;
            UpdateClientButtonsScript();
        }

        #region ICallbackEventHandler Members

        string ICallbackEventHandler.GetCallbackResult(){
            if (DropDown.JSProperties.ContainsKey(CpLookup)){
                DropDown.JSProperties.Remove(CpLookup);
                return DropDown.ClientID + "><" + HttpUtility.HtmlAttributeEncode(DropDown.Text) + "><" + ClientSideEventsHelper.ToJSBoolean((bool)DropDown.JSProperties[CpIsEmpty]);
            }
            var result = new StringBuilder();
            foreach (ListEditItem item in DropDown.Items)
                result.AppendFormat("{0}<{1}{2}|", HttpUtility.HtmlAttributeEncode(item.Text), item.Value,DropDown.SelectedItem == item ? "<" : "");
            if (result.Length > 0) result.Remove(result.Length - 1, 1);
            return $"{DropDown.ClientID}><{result}";
        }

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument){
            Callback?.Invoke(this, new CallbackEventArgs(eventArgument));
        }

        #endregion

        public void Setup(LookupEditorHelper helper){
            Helper = (WebLookupEditorHelper) helper;
            SearchButton.Visible = helper.IsSearchEditorMode();
            if (Helper.DisplayMember == null)
                throw new NullReferenceException("DisplayMember is not set for "+Helper.LookupObjectType.FullName);
            DropDown.TextField = Helper.DisplayMember.Name;
            DropDown.ValueField = Helper.LookupObjectTypeInfo.KeyMember.Name;
            foreach (var visibleColumn in Helper.LookupListViewModel.Columns.GetVisibleColumns()){
                DropDown.Columns.Add(visibleColumn.ModelMember.MemberInfo.BindingName,visibleColumn.Caption);
            }
            
        }
    }
}