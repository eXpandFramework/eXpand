using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;
using View = DevExpress.ExpressApp.View;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    public enum SearchModeType {
        StartsWith,
        Contains
    }

    public interface IFastSearchMemberViewItem : IModelNode {
        [DefaultValue(3),
         Description("Cantidad Minima de caracteres antes de realizar la busqueda")]
        int FilterMinLength { get; set; }

        [DefaultValue(false),
         Description("If set, a popup appears when the user enters the first character")]
        bool ImmediatePopup { get; set; }

        [DefaultValue(0x3e8)]
        int IncrementalFilteringDelay { get; set; }

        [DefaultValue(0)]
        SearchModeType IncrementalFilteringMode { get; set; }

        [Description("If set, a filter row appears at the top of the popup")]
        bool ShowAutoFilterRow { get; set; }
    }

    public interface IModelMemberViewItemFastSearch {
        [ModelBrowsable(typeof(WinLookUpPropertyEditorVisibilityCalculator))]
        IFastSearchMemberViewItem FastSearchMemberViewItem { get; }
    }

    public class WinLookUpPropertyEditorVisibilityCalculator:IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return typeof (FastSearchPropertyEditor).IsAssignableFrom(((IModelMemberViewItem) node).PropertyEditorType);
        }
    }

    [PropertyEditor(typeof(IXPObject), EditorAliases.FastSearchPropertyEditor,false)]
    public class FastSearchPropertyEditor : DXPropertyEditor, IComplexViewItem, IDependentPropertyEditor, ISupportViewShowing {
        LookupEditorHelper _helper;
        LookUpGridEditEx _lookup;
        View _lookupObjectView;

        public FastSearchPropertyEditor(Type objectType, IModelMemberViewItem item)
            : base(objectType, item) {
        }

        public LookupEditorHelper Helper {
            get { return _helper; }
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            if (_helper == null) {
                _helper = new LookupEditorHelper(application, objectSpace, MemberInfo.MemberTypeInfo, Model) {
                    SmallCollectionItemCount = 0x989680
                };
            }
            _helper.SetObjectSpace(objectSpace);
            _helper.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
        }

        protected virtual void AddNewObject() {
            var svp = new ShowViewParameters();
            IObjectSpace newObjectViewObjectSpace = _helper.Application.CreateObjectSpace(_helper.LookupObjectTypeInfo.Type);
            object newObject =newObjectViewObjectSpace.CreateObject(_helper.LookupObjectTypeInfo.Type);
            _lookupObjectView = _helper.Application.CreateDetailView(newObjectViewObjectSpace,newObject, true);
            svp.CreatedView = _lookupObjectView;
            newObjectViewObjectSpace.Committed += newObjectViewObjectSpace_Committed;
            newObjectViewObjectSpace.Disposed += newObjectViewObjectSpace_Disposed;
            svp.TargetWindow = TargetWindow.NewModalWindow;
            svp.Context = TemplateContext.PopupWindow;
            svp.Controllers.Add(_helper.Application.CreateController<DialogController>());
            _helper.Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
        }

        protected virtual void ClearCurrentObject() {
            _lookup.EditValue = null;
        }

        protected override object CreateControlCore() {
            _lookup = new LookUpGridEditEx();
            _lookup.QueryPopUp += Editor_QueryPopUp;
            return _lookup;
        }

        public new LookUpGridEditEx Control {
            get { return (LookUpGridEditEx)base.Control; }
        }

        protected override RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemGridLookUpEditEx();
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) {
                    if ((_lookup != null) && (_lookup.Properties != null)) {
                        _lookup.Properties.Enter -= properties_Enter;
                    }
                    if ((_helper != null) && (_helper.ObjectSpace != null)) {
                        _helper.ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
                    }
                }
            } finally {
                base.Dispose(disposing);
            }
        }

        protected virtual void InitializeDataSource() {
            if (((_lookup != null) && (_lookup.Properties != null)) && (_lookup.Properties.Helper != null)) {
                _lookup.Properties.DataSource =_lookup.Properties.Helper.CreateCollectionSource(_lookup.FindEditingObject()).List;
            }
        }

        void newObjectViewObjectSpace_Committed(object sender, EventArgs e) {
            _lookup.EditValue =_helper.ObjectSpace.GetObject(_lookupObjectView.CurrentObject);
            if (_lookup.Properties.DataSource != null) {
                ((IList)_lookup.Properties.DataSource).Add(_lookup.EditValue);
            }
        }

        void newObjectViewObjectSpace_Disposed(object sender, EventArgs e) {
            var os = (IObjectSpace)sender;
            os.Disposed -= newObjectViewObjectSpace_Disposed;
            os.Committed -= newObjectViewObjectSpace_Committed;
        }

        void ObjectSpace_Reloaded(object sender, EventArgs e) {
            InitializeDataSource();
        }

        protected virtual void OpenCurrentObject() {
            if (_lookup.EditValue==null)
                return;
            var showViewParameters = new ShowViewParameters();
            IObjectSpace openObjectViewObjectSpace = _helper.Application.CreateObjectSpace(_lookup.EditValue.GetType());
            object targetObject =openObjectViewObjectSpace.GetObject(_lookup.EditValue);
            if (targetObject != null) {
                openObjectViewObjectSpace.Committed += openObjectViewObjectSpace_Committed;
                openObjectViewObjectSpace.Disposed += openObjectViewObjectSpace_Disposed;
                _lookupObjectView = _helper.Application.CreateDetailView(openObjectViewObjectSpace,targetObject,true);
                showViewParameters.CreatedView = _lookupObjectView;
                showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                _helper.Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(null, null));
            }
        }

        void openObjectViewObjectSpace_Committed(object sender, EventArgs e) {
            if (_lookupObjectView != null) {
                _lookup.EditValue =_helper.ObjectSpace.GetObject(_lookupObjectView.CurrentObject);
            }
        }

        void openObjectViewObjectSpace_Disposed(object sender, EventArgs e) {
            var os = (IObjectSpace)sender;
            os.Disposed -= openObjectViewObjectSpace_Disposed;
            os.Committed -= openObjectViewObjectSpace_Committed;
        }

        void properties_ButtonClick(object sender, ButtonPressedEventArgs e) {
            switch (Convert.ToString(e.Button.Tag)) {
                case "MinusButtonTag":
                    ClearCurrentObject();
                    break;

                case "AddButtonTag":
                    AddNewObject();
                    break;

                case "DetailButtonTag":
                    OpenCurrentObject();
                    break;
            }
        }

        void properties_Enter(object sender, EventArgs e) {
            _lookup = (LookUpGridEditEx)sender;
            InitializeDataSource();
        }

        public override void Refresh() {
            base.Refresh();
            if (_lookup != null) {
                _lookup.UpdateDisplayText();
            }
        }

        private void OnViewShowingNotification() {
            if (viewShowingNotification != null) {
                viewShowingNotification(this, EventArgs.Empty);
            }
        }
        public event CancelEventHandler QueryPopUp;

        private void Editor_QueryPopUp(object sender, CancelEventArgs e) {
            if (QueryPopUp != null) {
                QueryPopUp(this, e);
            }
            OnViewShowingNotification();
        }

        private event EventHandler<EventArgs> viewShowingNotification;
        event EventHandler<EventArgs> ISupportViewShowing.ViewShowingNotification {
            add { viewShowingNotification += value; }
            remove { viewShowingNotification -= value; }
        }

        IList<string> IDependentPropertyEditor.MasterProperties {
            get { return _helper.MasterProperties; }
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var properties = (RepositoryItemGridLookUpEditEx)item;
            properties.Init(DisplayFormat, _helper);
            var settings = ((IModelMemberViewItemFastSearch)Model);
            
            if (settings.FastSearchMemberViewItem.IncrementalFilteringMode == SearchModeType.Contains) {
                properties.PopupFilterMode = PopupFilterMode.Contains;
            }
            properties.ImmediatePopup = settings.FastSearchMemberViewItem.ImmediatePopup;
            properties.View.OptionsView.ShowAutoFilterRow = settings.FastSearchMemberViewItem.ShowAutoFilterRow;
            properties.EditValueChangedDelay = settings.FastSearchMemberViewItem.IncrementalFilteringDelay;
            
            properties.ReadOnly = !AllowEdit.ResultValue;
            properties.Enter += properties_Enter;
            properties.ButtonClick += properties_ButtonClick;
            CreateButtons(properties);
        }

        void CreateButtons(RepositoryItemGridLookUpEditEx properties) {
            properties.ButtonsStyle = BorderStyles.HotFlat;
            var editButton = CreatelButton("Action_Edit", "tooltipDetail", "DetailButtonTag");
            editButton.Enabled = _lookup.EditValue != null;


            string info;
            editButton.Visible = DataManipulationRight.CanEdit(MemberInfo.MemberType, null, null,null,null)&&DataManipulationRight.CanEdit(ObjectType, propertyName, CurrentObject,null,_helper.ObjectSpace);
            properties.Buttons.Add(editButton);
            var newButton = CreatelButton("MenuBar_New", "tooltipNew", "AddButtonTag");
            newButton.Visible = DataManipulationRight.CanCreate(null, MemberInfo.MemberType, null, out info);
            properties.Buttons.Add(newButton);
            var clearButton = CreatelButton("Action_Clear", "tooltipClear", "MinusButtonTag");
            clearButton.Enabled = editButton.Enabled;
            if (!editButton.Visible) {
                properties.ReadOnly = true;
                clearButton.Visible = false;
                newButton.Visible = false;
            }
            properties.Buttons.Add(clearButton);
            _lookup.EditValueChanged += (sender, args) => {
                editButton.Enabled = _lookup.EditValue != null && AllowEdit.ResultValue;
                clearButton.Enabled = editButton.Enabled;
            };
        }

        EditorButton CreatelButton(string imageName, string tooltip, string tag) {
            var detailButton = new EditorButton{
                ImageLocation = ImageLocation.MiddleCenter,
                Kind = ButtonPredefines.Glyph,
                Image = ImageLoader.Instance.GetImageInfo(imageName).Image,
                ToolTip = CaptionHelper.GetLocalizedText("Texts", tooltip),
                Tag = tag,
                Enabled = AllowEdit.ResultValue
            };
            return detailButton;
        }
    }


    [ToolboxItem(false)]
    public class LookUpGridEditEx : GridLookUpEdit, IGridInplaceEdit {
        static readonly List<WeakReference> __ENCList = new List<WeakReference>();

        [AccessedThroughProperty("fPropertiesView")]
        GridView _fPropertiesView;
        object _gridEditingObject;

        static LookUpGridEditEx() {
            RepositoryItemGridLookUpEditEx.Register();
        }

        public LookUpGridEditEx() {
            __ENCAddToList(this);
            DataBindings.CollectionChanged += DataBindings_CollectionChanged;
        }

        public override string EditorTypeName {
            get { return "LookUpEditEx"; }
        }

        public override object EditValue {
            get { return base.EditValue; }
            set {
                if (((value != DBNull.Value) && (value != null)) &&
                    !Properties.Helper.LookupObjectType.IsInstanceOfType(value)) {
                    base.EditValue = null;
                } else {
                    base.EditValue = value;
                }
            }
        }


        internal virtual GridView fPropertiesView {
            [DebuggerNonUserCode]
            get { return _fPropertiesView; }
            [MethodImpl(MethodImplOptions.Synchronized), DebuggerNonUserCode]
            set { _fPropertiesView = value; }
        }


        public new RepositoryItemGridLookUpEditEx Properties {
            get { return (RepositoryItemGridLookUpEditEx)base.Properties; }
        }

        ControlBindingsCollection IGridInplaceEdit.DataBindings {
            get { return DataBindings; }
        }

        object IGridInplaceEdit.GridEditingObject {
            get { return _gridEditingObject; }
            set {
                if (_gridEditingObject != value) {
                    _gridEditingObject = value;
                    OnEditingObjectChanged();
                }
            }
        }

        [DebuggerNonUserCode]
        static void __ENCAddToList(object value) {
            List<WeakReference> list = __ENCList;
            lock (list) {
                if (__ENCList.Count == __ENCList.Capacity) {
                    int index = 0;
                    int num3 = __ENCList.Count - 1;
                    for (int i = 0; i <= num3; i++) {
                        WeakReference reference = __ENCList[i];
                        if (reference.IsAlive) {
                            if (i != index) {
                                __ENCList[index] = __ENCList[i];
                            }
                            index++;
                        }
                    }
                    __ENCList.RemoveRange(index, __ENCList.Count - index);
                    __ENCList.Capacity = __ENCList.Count;
                }
                __ENCList.Add(new WeakReference(value));
            }
        }

        void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e) {
            OnEditingObjectChanged();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                DataBindings.CollectionChanged -= DataBindings_CollectionChanged;
            }
            base.Dispose(disposing);
        }

        public object FindEditingObject() {
            return BindingHelper.FindEditingObject(this);
        }


        void OnEditingObjectChanged() {
            if ((FindEditingObject() == null) && (EditValue != null)) {
                EditValue = null;
            }
        }

        public new void UpdateDisplayText() {
            base.UpdateDisplayText();
            Refresh();
        }
    }


    public class RepositoryItemGridLookUpEditEx : RepositoryItemGridLookUpEdit, ILookupEditRepositoryItem {
        // Fields
        internal const string EditorName = "LookUpEditEx";
        LookupEditorHelper m_helper;

        // Methods
        static RepositoryItemGridLookUpEditEx() {
            Register();
        }

        public override string EditorTypeName {
            get { return "LookUpEditEx"; }
        }

        public LookupEditorHelper Helper {
            get { return m_helper; }
        }



        public new LookUpGridEditEx OwnerEdit {
            get { return (LookUpGridEditEx)base.OwnerEdit; }
        }

        string ILookupEditRepositoryItem.DisplayMember {
            get { return ((m_helper.DisplayMember != null) ? m_helper.DisplayMember.Name : string.Empty); }
        }

        Type ILookupEditRepositoryItem.LookupObjectType {
            get { return m_helper.LookupObjectType; }
        }

        public override void Assign(RepositoryItem item) {
            var source = (RepositoryItemGridLookUpEditEx)item;
            try {
                base.Assign(source);
            } catch (Exception) {
            }
            m_helper = source.Helper;
        }

        public override string GetDisplayText(FormatInfo format, object editValue) {
            string result = base.GetDisplayText(format, editValue);
            if ((string.IsNullOrEmpty(result) && (editValue != null)) && (m_helper != null)) {
                result = m_helper.GetDisplayText(editValue, NullText, format.FormatString);
            }
            return result;
        }

        public void Init(string displayFormat__1, LookupEditorHelper helper) {
            m_helper = helper;
            m_helper.SmallCollectionItemCount = 0x186a0;
            BeginUpdate();
            DisplayFormat.FormatString = displayFormat__1;
            DisplayFormat.FormatType = FormatType.Custom;
            EditFormat.FormatString = displayFormat__1;
            EditFormat.FormatType = FormatType.Custom;
            TextEditStyle = TextEditStyles.Standard;
            ExportMode = ExportMode.DisplayText;
            DisplayMember = ((ILookupEditRepositoryItem)this).DisplayMember;
            ValueMember = null;
            NullText = CaptionHelper.NullValueText;
            AllowNullInput = DefaultBoolean.True;
            View.OptionsBehavior.AutoPopulateColumns = false;
            if (helper.LookupListViewModel == null) {
                Trace.TraceWarning("helper is nothing for :");
            } else {
                View.Columns.Clear();
                View.OptionsView.ShowColumnHeaders = true;
                foreach (IModelColumn col in helper.LookupListViewModel.Columns) {
                    GridColumn info = View.Columns.AddField(col.PropertyName);
                    info.Caption = col.Caption;
                    if (col.Index.HasValue) {
                        info.VisibleIndex = col.Index.Value;
                    }
                    info.Width = col.Width;
                    info.ToolTip = col.ToolTip;
                    info.Fixed = FixedStyle.Left;
                    info.SortOrder = col.SortOrder;
                    info.SortIndex = col.SortIndex;
                    info.Visible = true;
                }
            }
            EndUpdate();
        }

        public static void Register() {
            if (!EditorRegistrationInfo.Default.Editors.Contains("LookUpEditEx")) {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("LookUpEditEx", typeof(LookUpGridEditEx),
                                                                               typeof(RepositoryItemGridLookUpEditEx),
                                                                               typeof(LookUpEditViewInfo),
                                                                               new ButtonEditPainter(), true,
                                                                               EditImageIndexes.LookUpEdit,
                                                                               typeof(PopupEditAccessible)));
            }
        }
    }
}