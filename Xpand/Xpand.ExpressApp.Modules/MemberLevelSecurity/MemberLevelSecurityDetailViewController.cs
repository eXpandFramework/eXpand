using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.MemberLevelSecurity {
    public class MemberLevelSecurityDetailViewController : ViewController<DetailView> {
        readonly Dictionary<string, ControlHelper> controlStorage = new Dictionary<string, ControlHelper>();

        void Initialize(string name) {
            if (!controlStorage.ContainsKey(name))
                controlStorage.Add(name, new ControlHelper());
            InitLayoutControltem(name);
            InitControls(name);
        }

        protected override void OnActivated() {
            base.OnActivated();
            View.ControlsCreated += View_OnControlsCreated;
            View.CurrentObjectChanged += View_CurrentObjectChanged;
        }


        void View_OnControlsCreated(object sender, EventArgs e) {
            protectMembers();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.CurrentObjectChanged -= View_CurrentObjectChanged;
        }


        void View_CurrentObjectChanged(object sender, EventArgs e) {
            protectMembers();
        }

        void protectMembers() {
            if (SecuritySystem.CurrentUser != null) {
                ReplaceEditorControl(View.GetItems<PropertyEditor>().Select(editor => editor.PropertyName));
            }
        }



        void ReplaceEditorControl(IEnumerable<string> propertyNames) {
            var xpBaseObject = View.CurrentObject as XPBaseObject;
            if (xpBaseObject != null) {
                foreach (string name in propertyNames) {
                    Initialize(name);
                    bool canRead = GetCanRead(name);
                    ReplaceEditorControl(!canRead ? controlStorage[name].ProtectedControl : controlStorage[name].DefaultControl, controlStorage[name]);
                }
            }
        }

        bool GetCanRead(string name) {
            bool canRead = DataManipulationRight.CanRead(View.ObjectTypeInfo.Type, name, View.CurrentObject, null);
            bool fit = ((MemberLevelObjectAccessComparer)ObjectAccessComparerBase.CurrentComparer).Fit(View.CurrentObject, MemberOperation.Read);
            if (fit)
                return canRead;
            return true;
        }

        void ReplaceEditorControl(object newControl, ControlHelper controlHelper) {
            if (controlHelper.LayoutControlItem != null) {
                Control oldControl = controlHelper.LayoutControlItem.Control;
                if (ReferenceEquals(newControl, oldControl))
                    return;

                bool enabled = newControl.Enabled;

                if (controlHelper.LayoutControlItem.Owner != null)
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
            if (controlStorage[name].LayoutControlItem == null) {
                int hash = FindControlHashByPropertyName(name);
                if (hash != 0)
                    controlStorage[name].LayoutControlItem = FindLayoutControlItemByControlHash(hash, name);
            }
        }

        void InitControls(string name) {
            if (controlStorage[name].DefaultControl == null) {
                var item = View.FindItem(name) as PropertyEditor;
                if (item != null) {
                    controlStorage[name].DefaultControl = (Control)item.Control;
                    var protectedContentEdit = new ProtectedContentEdit();
                    ((RepositoryItemProtectedContentTextEdit)protectedContentEdit.Properties).ProtectedContentText =
                        Application.Model.ProtectedContentText;
                    controlStorage[name].ProtectedControl = protectedContentEdit;
                }
            }
        }



        int FindControlHashByPropertyName(string name) {
            var item = View.FindItem(name);
            if (item != null)
                return item.Control.GetHashCode();
            return 0;
        }

        LayoutControlItem FindLayoutControlItemByControlHash(int hash, string name) {
            foreach (object obj in (((LayoutControl)(View.Control))).Items)
                if (obj is LayoutControlItem) {
                    var item = (LayoutControlItem)obj;
                    if (item.Control != null && item.Control.GetHashCode() == hash) {
                        controlStorage[name].LayoutControlItem = item;
                        return controlStorage[name].LayoutControlItem;
                    }
                }
            return null;
        }
        #region Nested type: ControlHelper
        public class ControlHelper {
            public Control DefaultControl { get; set; }


            public LayoutControlItem LayoutControlItem { get; set; }

            public Control ProtectedControl { get; set; }
        }
        #endregion
    }
}