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
using View = DevExpress.ExpressApp.View;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    public enum SearchModeType {
        StartsWith,
        Contains
    }


    public interface IModelColumnFastSearchItem {
        // Properties
        [DefaultValue(3), Category("Behavior"),
         Description("Cantidad Minima de caracteres antes de realizar la busqueda")]
        int FilterMinLength { get; set; }

        [DefaultValue(false), Category("LookupGrid"),
         Description("If set, a popup appears when the user enters the first character")]
        bool ImmediatePopup { get; set; }

        [DefaultValue(0x3e8), Category("Behavior")]
        int IncrementalFilteringDelay { get; set; }

        [Category("Behavior"), DefaultValue(0)]
        SearchModeType IncrementalFilteringMode { get; set; }

        [Description("If set, a filter row appears at the top of the popup"), Category("LookupGrid")]
        bool ShowAutoFilterRow { get; set; }
    }


    [PropertyEditor(typeof(IXPObject), false)]
    public class WinLookUpPropertyEditor : DXPropertyEditor, IComplexPropertyEditor {
        // Fields
        LookupEditorHelper _helper;
        LookUpGridEditEx _lookup;
        View lookupObjectView;

        // Methods
        public WinLookUpPropertyEditor(Type objectType, IModelMemberViewItem item)
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
            IObjectSpace newObjectViewObjectSpace = _helper.Application.CreateObjectSpace();
            object newObject =
                RuntimeHelpers.GetObjectValue(newObjectViewObjectSpace.CreateObject(_helper.LookupObjectTypeInfo.Type));
            lookupObjectView = _helper.Application.CreateDetailView(newObjectViewObjectSpace,
                                                                    RuntimeHelpers.GetObjectValue(newObject), true);
            svp.CreatedView = lookupObjectView;
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
            return _lookup;
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
                _lookup.Properties.DataSource =
                    _lookup.Properties.Helper.CreateCollectionSource(
                        RuntimeHelpers.GetObjectValue(_lookup.FindEditingObject())).List;
            }
        }

        void newObjectViewObjectSpace_Committed(object sender, EventArgs e) {
            _lookup.EditValue =
                RuntimeHelpers.GetObjectValue(
                    _helper.ObjectSpace.GetObject(RuntimeHelpers.GetObjectValue(lookupObjectView.CurrentObject)));
            if (_lookup.Properties.DataSource != null) {
                ((IList)_lookup.Properties.DataSource).Add(RuntimeHelpers.GetObjectValue(_lookup.EditValue));
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
            var svp = new ShowViewParameters();
            IObjectSpace openObjectViewObjectSpace = _helper.Application.CreateObjectSpace();
            object targetObject =
                RuntimeHelpers.GetObjectValue(
                    openObjectViewObjectSpace.GetObject(RuntimeHelpers.GetObjectValue(_lookup.EditValue)));
            if (targetObject != null) {
                openObjectViewObjectSpace.Committed += openObjectViewObjectSpace_Committed;
                openObjectViewObjectSpace.Disposed += openObjectViewObjectSpace_Disposed;
                lookupObjectView = _helper.Application.CreateDetailView(openObjectViewObjectSpace,
                                                                        RuntimeHelpers.GetObjectValue(targetObject),
                                                                        true);
                svp.CreatedView = lookupObjectView;
                svp.TargetWindow = TargetWindow.NewModalWindow;
                _helper.Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
            }
        }

        void openObjectViewObjectSpace_Committed(object sender, EventArgs e) {
            if (lookupObjectView != null) {
                _lookup.EditValue =
                    RuntimeHelpers.GetObjectValue(
                        _helper.ObjectSpace.GetObject(RuntimeHelpers.GetObjectValue(lookupObjectView.CurrentObject)));
            }
        }

        void openObjectViewObjectSpace_Disposed(object sender, EventArgs e) {
            var os = (IObjectSpace)sender;
            os.Disposed -= openObjectViewObjectSpace_Disposed;
            os.Committed -= openObjectViewObjectSpace_Committed;
        }

        void properties_ButtonClick(object sender, ButtonPressedEventArgs e) {
            switch (Convert.ToString(RuntimeHelpers.GetObjectValue(e.Button.Tag))) {
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

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            var properties = (RepositoryItemGridLookUpEditEx)item;
            properties.Init(DisplayFormat, _helper);
            var settings = Model as IModelColumnFastSearchItem;
            if (settings != null) {
                if (settings.IncrementalFilteringMode == SearchModeType.Contains) {
                    properties.PopupFilterMode = PopupFilterMode.Contains;
                }
                properties.ImmediatePopup = settings.ImmediatePopup;
                properties.View.OptionsView.ShowAutoFilterRow = settings.ShowAutoFilterRow;
                properties.EditValueChangedDelay = settings.IncrementalFilteringDelay;
            }
            properties.ReadOnly = !AllowEdit.ResultValue;
            properties.Enter += properties_Enter;
            properties.ButtonClick += properties_ButtonClick;
            properties.ButtonsStyle = BorderStyles.HotFlat;
            var detailButton = new EditorButton {
                ImageLocation = ImageLocation.MiddleCenter,
                Kind = ButtonPredefines.Glyph,
                Image = ImageLoader.Instance.GetImageInfo("Action_Edit").Image,
                ToolTip = CaptionHelper.GetLocalizedText("Texts", "tooltipDetail"),
                Tag = "DetailButtonTag",
                Enabled = AllowEdit.ResultValue
            };
            properties.Buttons.Add(detailButton);
            var newButton = new EditorButton {
                ImageLocation = ImageLocation.MiddleCenter,
                Kind = ButtonPredefines.Glyph,
                Image = ImageLoader.Instance.GetImageInfo("MenuBar_New").Image,
                ToolTip = CaptionHelper.GetLocalizedText("Texts", "tooltipNew"),
                Tag = "AddButtonTag",
                Enabled = AllowEdit.ResultValue
            };
            properties.Buttons.Add(newButton);
            var clearButton = new EditorButton {
                ImageLocation = ImageLocation.MiddleCenter,
                Kind = ButtonPredefines.Glyph,
                Image = ImageLoader.Instance.GetImageInfo("Action_Clear").Image,
                ToolTip = CaptionHelper.GetLocalizedText("Texts", "tooltipClear"),
                Tag = "MinusButtonTag",
                Enabled = AllowEdit.ResultValue
            };
            properties.Buttons.Add(clearButton);
        }

        // Properties
    }


    [ToolboxItem(false)]
    public class LookUpGridEditEx : GridLookUpEdit, IGridInplaceEdit {
        // Fields
        static readonly List<WeakReference> __ENCList = new List<WeakReference>();

        [AccessedThroughProperty("fPropertiesView")]
        GridView _fPropertiesView;
        object gridEditingObject;

        // Methods
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
                    !Properties.Helper.LookupObjectType.IsInstanceOfType(RuntimeHelpers.GetObjectValue(value))) {
                    base.EditValue = null;
                } else {
                    base.EditValue = RuntimeHelpers.GetObjectValue(value);
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
            get { return gridEditingObject; }
            set {
                if (gridEditingObject != value) {
                    gridEditingObject = RuntimeHelpers.GetObjectValue(value);
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
                __ENCList.Add(new WeakReference(RuntimeHelpers.GetObjectValue(value)));
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
            string result = base.GetDisplayText(format, RuntimeHelpers.GetObjectValue(editValue));
            if ((string.IsNullOrEmpty(result) && (editValue != null)) && (m_helper != null)) {
                result = m_helper.GetDisplayText(RuntimeHelpers.GetObjectValue(editValue), NullText, format.FormatString);
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