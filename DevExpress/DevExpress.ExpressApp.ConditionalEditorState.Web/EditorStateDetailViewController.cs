using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.ConditionalEditorState;
using DevExpress.ExpressApp.ConditionalEditorState.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.Web.ASPxTabControl;
using DevExpress.Web.ASPxTabControl.Internal;
using DevExpress.Persistent.Base;

namespace DevExpress.ExpressApp.ConditionalEditorState.Web {
    /// <summary>
    /// A DetailView controller for the Web Forms platform that provides the capability to customize the view's editors.
    /// </summary>
    public class EditorStateDetailViewController : EditorStateDetailViewControllerBase {
        public new Control Control {
            get {
                return controlCore as Control;
            }
        }
        protected override void OnViewControlsCreated(object sender, EventArgs e) {
            base.OnViewControlsCreated(sender, e);
            if (IsReady) {
                Control.PreRender += OnControlPreRender;
            }
        }
        protected override void ResourcesReleasing() {
            if (IsReady) {
                Control.PreRender -= OnControlPreRender;
            }
            base.ResourcesReleasing();
        }
        private void OnControlPreRender(object sender, EventArgs e) {
            InvalidateRules(true);
        }
        protected override void OnObjectSpaceReloaded(object sender, EventArgs e) {
            //base.OnObjectSpaceReloaded(sender, e);
            isRefreshing = false;
        }
        public override bool IsReady {
            get {
                return base.IsReady && (Control != null);
            }
        }
        //Dennis: TODO S19340
        protected override void HideShowEditor(string property, bool hidden) {
            PropertyEditor editor = GetPropertyEditor(property);
            if (editor == null) return;
            Control control = editor.Control as Control;
            if (control != null) {
                if (control.Visible == !hidden) {
                    return;
                } else {
                    HideShowEditorCore(control, hidden);
                    bool isList = EditorStateRuleManager.FindMember(editor.ObjectTypeInfo.Type, editor.PropertyName).IsList;
                    if (isList && DetailView.ViewEditMode == ViewEditMode.View) {
                        ASPxPageControlEx tabControl = FindTabControl(control);
                        if (tabControl != null) {
                            TabPage page = tabControl.TabPages.FindByText(editor.Caption);
                            if (page != null) {
                                page.Visible = !hidden;
                            }
                            int count = tabControl.TabPages.GetVisibleTabPageCount();
                            if (count > 0) {
                                tabControl.Visible = true;
                                tabControl.ActiveTabIndex = 0;
                            } else {
                                tabControl.Visible = false;
                            }
                        }
                    }
                }
            } else {
                Tracing.Tracer.LogWarning(string.Format(EditorStateLocalizer.Active.GetLocalizedString("CannotFindInfoForProperty"), "editor's Control", property));
            }
            CustomizeEditor(editor);
        }
        protected void HideShowEditorCore(Control control, bool hidden) {
            control.Visible = !hidden;
            if (control.Parent is TableCell) {
                if (control.Parent.Parent is TableRow) {
                    control.Parent.Parent.Visible = !hidden;
                }
            }
        }
        private ASPxPageControlEx FindTabControl(Control control) {
            if (control == null || control is ASPxPageControlEx) {
                return (ASPxPageControlEx)control;
            } else if (control is TCControlBase) {
                return FindTabControl(((TCControlBase)control).TabControl);
            } else {
                return FindTabControl(control.Parent);
            }
        }
    }
}