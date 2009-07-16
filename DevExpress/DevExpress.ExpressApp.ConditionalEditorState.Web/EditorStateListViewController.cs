using System;
using DevExpress.ExpressApp.ConditionalEditorState.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web.ASPxGridView;
using DevExpress.Web.ASPxGridView.Rendering;
using System.Web.UI;

namespace DevExpress.ExpressApp.ConditionalEditorState.Web {
    /// <summary>
    /// A ListView controller for the Web Forms platform that provides the capability to customize the view's editors.
    /// </summary>
    public class EditorStateListViewController : EditorStateListViewControllerBase {
        public new ASPxGridView Control {
            get {
                return controlCore as ASPxGridView;
            }
        }
        protected override void OnViewControlsCreated(object sender, EventArgs e) {
            base.OnViewControlsCreated(sender, e);
            if (IsReady) {
                ASPxGridView gv = ((ASPxGridListEditor)ListView.Editor).Grid;
                controlCore = gv;
                gv.PreRender += OnControlPreRender;
                gv.HtmlRowPrepared += OnGridHtmlRowPrepared;
                gv.HtmlEditFormCreated += OnGridHtmlEditFormCreated;
            }
        }
        protected override void ResourcesReleasing() {
            if (IsReady) {
                ASPxGridView gv = ((ASPxGridListEditor)ListView.Editor).Grid;
                gv.PreRender -= OnControlPreRender;
                gv.HtmlRowPrepared -= OnGridHtmlRowPrepared;
                gv.HtmlEditFormCreated -= OnGridHtmlEditFormCreated;
            }
            base.ResourcesReleasing();
        }
        private void OnGridHtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e) {
            DisableEnableEditFormEditors(e);
        }
        private void OnGridHtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e) {
            DisableEnableInlineEditors(e);
        }
        private void OnControlPreRender(object sender, EventArgs e) {
            InvalidateRules(true);
        }
        public override bool IsReady {
            get {
                return base.IsReady && (ListView.Editor is ASPxGridListEditor);
            }
        }
        protected override void HideShowColumn(string property, bool hidden) {
            GridViewDataColumn column = GetColumn(property) as GridViewDataColumn;
            if (column != null) {
                if (column.Visible == !hidden) {
                    return;
                } else {
                    column.ShowInCustomizationForm = !hidden;
                    column.Visible = !hidden;
                }
            } else {
                Tracing.Tracer.LogWarning(string.Format(EditorStateLocalizer.Active.GetLocalizedString("CannotFindInfoForProperty"), typeof(GridViewDataColumn).Name, property));
            }
        }
        protected virtual void DisableEnableInlineEditors(ASPxGridViewTableRowEventArgs e) {
            if (!Control.IsEditing || e.KeyValue == null) return;

            Type objectType = ListView.ObjectTypeInfo.Type;
            object obj = View.ObjectSpace.GetObjectByKey(objectType, e.KeyValue);

            if (e.RowType == GridViewRowType.InlineEdit) {
                for (int i = 0; i < e.Row.Cells.Count; i++) {
                    if (e.Row.Cells[i] is GridViewTableEditorCellBase) {
                        GridViewTableEditorCellBase editCell = ((GridViewTableEditorCellBase)(e.Row.Cells[i]));
                        string fieldName = ((GridViewDataColumn)editCell.Column).FieldName;
                        if (!String.IsNullOrEmpty(fieldName)) {
                            DisableEnableCell(editCell, objectType, obj, fieldName);
                        }
                    }
                }
            }
        }
        private void DisableEnableEditFormEditors(ASPxGridViewEditFormEventArgs e) {
            Type objectType = ListView.ObjectTypeInfo.Type;
            object obj = View.ObjectSpace.GetObject(Control.GetRow(Control.EditingRowVisibleIndex));

            if (!Control.IsEditing || obj == null) return;

            Accept(e.EditForm,
                delegate(System.Web.UI.Control c) {
                    if (c is GridViewTableEditFormEditorCell) {
                        GridViewTableEditorCellBase editCell = (GridViewTableEditorCellBase)c;
                        string fieldName = ((GridViewDataColumn)editCell.Column).FieldName;
                        if (!String.IsNullOrEmpty(fieldName)) {
                            DisableEnableCell(editCell, objectType, obj, fieldName);
                        }
                    }
                }
            );
        }
        private void Accept(Control container, Action<Control> action) {
            if (action != null) {
                action(container);
            }
            foreach (Control c in container.Controls) {
                Accept(c, action);
            }
        }
        protected virtual void DisableEnableCell(GridViewTableEditorCellBase editCell, Type objectType, object obj, string fieldName) {
            foreach (EditorStateRule rule in EditorStateRuleManager.Instance[objectType]) {
                if ((rule.ViewType == ViewType.ListView) ||
                    rule.ViewType == ViewType.Any) {

                    foreach (string property in rule.Properties) {
                        EditorStateInfo info = EditorStateRuleManager.CalculateEditorStateInfo(obj, property, rule);
                        if (info != null && info.Rule.ObjectType == objectType) {
                            IMemberInfo mi = EditorStateRuleManager.FindMember(objectType, property);
                            string displayableFieldName = GetColumnFieldName(property);
                            if (fieldName == displayableFieldName) {
                                EditorStateInfoCustomizingEventArgs args = new EditorStateInfoCustomizingEventArgs(info, false);
                                RaiseEditorStateCustomizing(args);
                                if (!args.Cancel) {
                                    switch (info.EditorState) {
                                        case EditorState.Disabled: {
                                                editCell.Enabled = !info.Active;
                                                break;
                                            }
                                        case EditorState.Default:
                                        case EditorState.Hidden:
                                        default: break;
                                    }
                                    RaiseEditorStateCustomized(new EditorStateInfoCustomizedEventArgs(info));
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        protected override string GetColumnFieldName(string property) {
            ColumnsNodeWrapper cw = new ListViewInfoNodeWrapper(ListView.Info).Columns;
            if (cw == null) return string.Empty;
            ColumnInfoNodeWrapper columnInfo = cw.FindColumnInfo(property);
            if (columnInfo == null) return string.Empty;
            if (!string.IsNullOrEmpty(columnInfo.LookupPropertyName)) {
                return columnInfo.PropertyName + "." + columnInfo.LookupPropertyName;
            } else {
                IMemberInfo mi = ReflectionHelper.FindDisplayableMemberDescriptor(ListView.ObjectTypeInfo, columnInfo.PropertyName);
                string displayablePropertyName = mi != null ? mi.Name : columnInfo.PropertyName;
                if (!string.IsNullOrEmpty(displayablePropertyName)) {
                    return displayablePropertyName;
                }
                return columnInfo.PropertyName;
            }
        }
        protected override object GetColumn(string property) {
            string fieldName = GetColumnFieldName(property);
            foreach (object c in Control.Columns) {
                GridViewDataColumn column = c as GridViewDataColumn;
                if (column != null && column.FieldName == fieldName) {
                    return column;
                }
            }
            return null;
        }
    }
}