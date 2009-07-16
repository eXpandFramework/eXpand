using System;
using DevExpress.ExpressApp.ConditionalEditorState.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Persistent.Base;

namespace DevExpress.ExpressApp.ConditionalEditorState.Win {
    /// <summary>
    /// A ListView controller for the Windows Forms platform that provides the capability to customize the view's editors.
    /// </summary>
    public class EditorStateListViewController : EditorStateListViewControllerBase {
        public new GridControl Control {
            get {
                return controlCore as GridControl;
            }
        }
        protected override void OnViewControlsCreated(object sender, EventArgs e) {
            base.OnViewControlsCreated(sender, e);
            if (IsReady) {
                GridView gv = ((GridListEditor)ListView.Editor).GridView;
                gv.ShowingEditor += DisableEnableEditor;
                controlCore = ((GridListEditor)ListView.Editor).Grid;
                Control.Paint += OnControlPaint;
                if (!Control.IsHandleCreated) {
                    Control.HandleCreated += OnControlHandleCreated;
                } else {
                    InvalidateRules(true);
                }
            }
        }
        protected override void ResourcesReleasing() {
            if (IsReady) {
                GridView gv = ((GridListEditor)ListView.Editor).GridView;
                gv.ShowingEditor -= DisableEnableEditor;
                Control.Paint -= OnControlPaint;
                Control.HandleCreated -= OnControlHandleCreated;
            }
            base.ResourcesReleasing();
        }
        private void OnControlPaint(object sender, System.Windows.Forms.PaintEventArgs e) {
            ForceCustomization();
        }
        private void OnControlHandleCreated(object sender, EventArgs e) {
            InvalidateRules(true);
        }
        protected override void ForceCustomizationCore(object currentObject, DevExpress.ExpressApp.ConditionalEditorState.Core.EditorStateRule rule) {
            try {
                Control.BeginUpdate();
                Control.BeginInit();
                base.ForceCustomizationCore(currentObject, rule);
            } finally {
                Control.EndInit();
                Control.EndUpdate();
            }
        }
        public override bool IsReady {
            get {
                return base.IsReady && (ListView.Editor is GridListEditor);
            }
        }
        protected override void HideShowColumn(string property, bool hidden) {
            GridView gv = (GridView)Control.MainView;
            GridColumn column = GetColumn(property) as GridColumn;
            if (column != null) {
                column.OptionsColumn.ShowInCustomizationForm = !hidden;
                if (column.Visible == !hidden) {
                    return;
                } else {
                    column.Visible = !hidden;
                }
            } else {
                Tracing.Tracer.LogWarning(string.Format(EditorStateLocalizer.Active.GetLocalizedString("CannotFindInfoForProperty"), typeof(GridColumn).Name, property));
            }
        }
        private void DisableEnableEditor(object sender, System.ComponentModel.CancelEventArgs e) {
            GridView gv = (GridView)sender;
            int rowHandle = gv.FocusedRowHandle;
            if (rowHandle == GridControl.NewItemRowHandle || rowHandle == GridControl.InvalidRowHandle || rowHandle == GridControl.AutoFilterRowHandle) return;

            Type objectType = ListView.ObjectTypeInfo.Type;
            object obj = View.ObjectSpace.GetObject(gv.GetFocusedRow());

            foreach (EditorStateRule rule in EditorStateRuleManager.Instance[objectType]) {
                if ((rule.ViewType == ViewType.ListView) ||
                        rule.ViewType == ViewType.Any) {

                    foreach (string property in rule.Properties) {
                        EditorStateInfo info = EditorStateRuleManager.CalculateEditorStateInfo(obj, property, rule);
                        if (info != null) {
                            IMemberInfo mi = EditorStateRuleManager.FindMember(objectType, property);
                            string fieldName = gv.FocusedColumn.FieldName;
                            string displayableFieldName = GetColumnFieldName(property);
                            if (fieldName == displayableFieldName) {
                                EditorStateInfoCustomizingEventArgs args = new EditorStateInfoCustomizingEventArgs(info, false);
                                RaiseEditorStateCustomizing(args);
                                if (!args.Cancel) {
                                    switch (info.EditorState) {
                                        case EditorState.Disabled: {
                                                e.Cancel = info.Active;
                                                break;
                                            }
                                        case EditorState.Default:
                                        case EditorState.Hidden:
                                        default: break;
                                    }
                                    RaiseEditorStateCustomized(new EditorStateInfoCustomizedEventArgs(info)); ;
                                }
                                return;
                            }
                        }
                    }
                }
            }
        }
        protected override object GetColumn(string property) {
            GridView gv = (GridView)Control.MainView;
            string fieldName = GetColumnFieldName(property);
            foreach (GridColumn column in gv.Columns) {
                if (column.FieldName == fieldName) {
                    return column;
                }
            }
            return null;
        }
        protected override string GetColumnFieldName(string property) {
            GridView gv = (GridView)Control.MainView;
            IMemberInfo mi = EditorStateRuleManager.FindMember(ListView.ObjectTypeInfo.Type, property);
            string fieldName = property;
            if (typeof(IXPSimpleObject).IsAssignableFrom(mi.MemberType)) {
                fieldName += "!";
            }
            return fieldName;
        }
    }
}