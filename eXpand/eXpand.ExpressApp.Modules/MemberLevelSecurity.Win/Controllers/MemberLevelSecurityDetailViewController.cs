using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.MemberLevelSecurity.Win.Controllers {
    public partial class MemberLevelSecurityDetailViewController : MemberLevelSecurityControllerBase {
        readonly Dictionary<string, ControlHelper> controlHelpers = new Dictionary<string, ControlHelper>();


        public MemberLevelSecurityDetailViewController() {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.DetailView;
        }


        void Initialize(string name) {
            if (!controlHelpers.ContainsKey(name))
                controlHelpers.Add(name, new ControlHelper());
            InitLayoutControltem(name);
            InitDefaultControl(name);
            InitNewControl(name);
        }

        protected override void OnActivated() {
            base.OnActivated();
            View.ControlsCreated += View_OnControlsCreated;
            View.CurrentObjectChanged += View_CurrentObjectChanged;
        }


        void View_OnControlsCreated(object sender, EventArgs e) {
            if (HasProtectPermission())
                SetEditorButtons(((DetailView) View).GetPropertyEditors(typeof (ButtonEdit)));
            else
                protectMembers();
        }

        protected override void OnDeactivating() {
            base.OnDeactivating();
            View.CurrentObjectChanged -= View_CurrentObjectChanged;
        }


        void View_CurrentObjectChanged(object sender, EventArgs e) {
            protectMembers();
        }

        void protectMembers() {
            if (SecuritySystem.CurrentUser != null) {
                ICollection<PropertyEditor> propertyEditors = ((DetailView)View).GetPropertyEditors(typeof(ButtonEdit));
                if (!HasProtectPermission())
                    ReplaceEditorControl((from editor in propertyEditors
                                          select editor.PropertyName).ToList());
                else
                    SetEditorButtons(propertyEditors);
            }
        }

        void SetEditorButtons(IEnumerable<PropertyEditor> propertyEditors) {
            if (View.CurrentObject is XPBaseObject)
                foreach (DXPropertyEditor propertyEditor in propertyEditors)
                    MemberLevelSecurityListViewViewController.SetEditorButtonKind(
                                                                                 ((ButtonEdit) propertyEditor.Control).
                                                                                     Properties,
                                                                                 (XPBaseObject) View.CurrentObject,
                                                                                 propertyEditor.PropertyName);
        }


        void ReplaceEditorControl(IEnumerable<string> propertyNames) {
            var xpBaseObject = View.CurrentObject as XPBaseObject;
            if (xpBaseObject != null) {
                foreach (string name in propertyNames) {
                    Initialize(name);
                    ReplaceEditorControl(
                                            MemberLevelSecurityListViewViewController.IsProtected(xpBaseObject, name).
                                                IsProtected
                                                ? controlHelpers[name].NewControl
                                                : controlHelpers[name].DefaultControl, controlHelpers[name]);
                }
            }
        }

        void ReplaceEditorControl(Control newControl, ControlHelper controlHelper) {
            if (controlHelper.LayoutControlItem != null) {
                Control oldControl = controlHelper.LayoutControlItem.Control;
                if (ReferenceEquals(newControl, oldControl))
                    return;

                bool enabled = newControl.Enabled;

                if (controlHelper.LayoutControlItem.Owner!=null)
                    controlHelper.LayoutControlItem.Owner.BeginUpdate();
                controlHelper.LayoutControlItem.BeginInit();

                controlHelper.LayoutControlItem.Control = newControl;
                if (oldControl != null) oldControl.Parent = null;

                controlHelper.LayoutControlItem.EndInit();
                if (controlHelper.LayoutControlItem.Owner != null)
                    controlHelper.LayoutControlItem.Owner.EndUpdate();

                controlHelper.LayoutControlItem.Control.Enabled = enabled;
            }
        }

        void InitLayoutControltem(string name) {
            if (controlHelpers[name].LayoutControlItem == null) {
                int hash = FindControlHashByPropertyName(name);
                if (hash != 0)
                    controlHelpers[name].LayoutControlItem = FindLayoutControlItemByControlHash(hash, name);
            }
        }

        void InitDefaultControl(string name) {
            if (controlHelpers[name].DefaultControl == null) {
                DetailViewItem item = ((DetailView)View).FindItem(name);
                if (item != null)
                    controlHelpers[name].DefaultControl = (Control) item.Control;
            }
        }

        void InitNewControl(string name) {
            if (controlHelpers[name].NewControl == null)
                controlHelpers[name].NewControl = new ProtectedContentEdit();
        }

        int FindControlHashByPropertyName(string name) {
            DetailViewItem item = ((DetailView)View).FindItem(name);
            if (item != null)
                return item.Control.GetHashCode();
            return 0;
        }

        LayoutControlItem FindLayoutControlItemByControlHash(int hash, string name) {
            foreach (object obj in (((LayoutControl)(View.Control))).Items)
                if (obj is LayoutControlItem) {
                    var item = (LayoutControlItem) obj;
                    if (item.Control != null && item.Control.GetHashCode() == hash) {
                        controlHelpers[name].LayoutControlItem = item;
                        return controlHelpers[name].LayoutControlItem;
                    }
                }
            return null;
        }
        #region Nested type: ControlHelper
        public class ControlHelper {
            public Control DefaultControl { get; set; }


            public LayoutControlItem LayoutControlItem { get; set; }

            public Control NewControl { get; set; }
        }
        #endregion
    }
}