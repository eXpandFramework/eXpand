using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Utils;
using DevExpress.Web.ASPxCallback;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxGridView;
using DevExpress.Xpo;
using CallbackEventArgs = DevExpress.Web.ASPxCallback.CallbackEventArgs;

namespace Xpand.ExpressApp.Web.ListEditors{
    [ListEditor(typeof (object), false)]
    public class MultiEditASPxGridListEditor : XpandASPxGridListEditor {
        private const String CallbackArgumentFormat = "function (s, e) {{ {0}.PerformCallback(\"{1}|{2}|\" + {3}); }}";
        private ASPxCallback _callback;

        public MultiEditASPxGridListEditor(IModelListView model)
            : base(model){
        }

        protected override object CreateControlsCore(){
            var panel = new Panel();
            _callback = new ASPxCallback{
                ID = ObjectTypeInfo.Type.Name + "aspxCallback1",
                ClientInstanceName = ObjectTypeInfo.Type.Name + "_callback1"
            };
            _callback.Callback += callback_Callback;
            panel.Controls.Add(_callback);
            var grid = (ASPxGridView) base.CreateControlsCore();
            grid.HtmlDataCellPrepared += grid_HtmlDataCellPrepared;
            panel.Controls.Add(grid);
            return panel;
        }

        private void grid_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e){
            if (IsColumnSupported((IModelColumn) e.DataColumn.Model())){
                e.Cell.Attributes["onclick"] = RenderHelper.EventCancelBubbleCommand;
            }
        }

        protected override ITemplate CreateDataItemTemplate(IModelColumn columnInfo){
            if (IsColumnSupported(columnInfo)){
                var editModeTemplate =
                    (EditModeDataItemTemplate) CreateDefaultColumnTemplate(columnInfo, this, ViewEditMode.Edit);
                editModeTemplate.PropertyEditor.ImmediatePostData = false;
                editModeTemplate.CustomCreateCellControl += editModeTemplate_CustomCreateCellControl;
                return editModeTemplate;
            }
            return base.CreateDataItemTemplate(columnInfo);
        }

        private void editModeTemplate_CustomCreateCellControl(object sender, CustomCreateCellControlEventArgs e){
            if (e.PropertyEditor.Editor is ASPxWebControl){
                e.PropertyEditor.Editor.Init += (s, args) => Editor_Init(s, e.PropertyEditor.Editor);
            }
            else if (e.PropertyEditor is ASPxLookupPropertyEditor){
                var editor = e.PropertyEditor as ASPxLookupPropertyEditor;
                editor.DropDownEdit.DropDown.Init += (s, args) => Editor_Init(s, e.PropertyEditor.Editor);
            }
        }

        private void Editor_Init(object sender, WebControl baseEditor){
            var editor = (ASPxWebControl) sender;
            var container = baseEditor.NamingContainer as GridViewDataItemTemplateContainer;
            if (container != null){
                var columnInfo = container.Column;
                if (columnInfo != null)
                    editor.SetClientSideEventHandler("ValueChanged", String.Format(CallbackArgumentFormat,
                        _callback.ClientInstanceName, container.KeyValue, columnInfo.Model().PropertyName,
                        editor is ASPxDateEdit ? "s.GetText()" : "s.GetValue()"));
            }
        }

        private void callback_Callback(object source, CallbackEventArgs e){
            String[] p = e.Parameter.Split('|');
            Object key = TypeDescriptor.GetConverter(ObjectTypeInfo.KeyMember.MemberType).ConvertFromString(p[0]);
            IMemberInfo member = ObjectTypeInfo.FindMember(p[1]);
            Object value = null;
            if (typeof (IXPSimpleObject).IsAssignableFrom(member.MemberType)){
                Type memberKeyType = XafTypesInfo.Instance.FindTypeInfo(member.MemberType).KeyMember.MemberType;
                int index1 = p[2].LastIndexOf("(", StringComparison.Ordinal);
                int index2 = p[2].LastIndexOf(")", StringComparison.Ordinal);
                if (index1 > 0 && index2 > index1){
                    string memberKeyText = p[2].Substring(index1 + 1, index2 - index1 - 1);
                    value = ObjectSpace.GetObjectByKey(member.MemberType,
                        Convert.ChangeType(memberKeyText, memberKeyType));
                }
            }
            else{
                value = TypeDescriptor.GetConverter(member.MemberType).ConvertFromString(p[2]);
            }
            object obj = ObjectSpace.GetObjectByKey(ObjectTypeInfo.Type, key);
            member.SetValue(obj, value);
            ObjectSpace.CommitChanges();
        }

        private IEnumerable<Type> supportedPropertyEditorTypes(){
            return new[]{
                typeof (ASPxStringPropertyEditor),
                typeof (ASPxIntPropertyEditor),
                typeof (ASPxBooleanPropertyEditor),
                typeof (ASPxEnumPropertyEditor),
                typeof (ASPxDateTimePropertyEditor),
                typeof (ASPxLookupPropertyEditor)
            };
        }

        protected virtual bool IsColumnSupported(IModelColumn model){
            if (model.GroupIndex >= 0){
                return false;
            }
            return supportedPropertyEditorTypes().Any(type => type.IsAssignableFrom(model.PropertyEditorType));
        }

        protected override ColumnWrapper AddColumnCore(IModelColumn columnInfo){
            var columnWrapper = (ASPxGridViewColumnWrapper) base.AddColumnCore(columnInfo);
            if (IsColumnSupported((IModelColumn) columnWrapper.Column.Model())){
                columnWrapper.Column.Settings.AllowSort = DefaultBoolean.False;
                columnWrapper.Column.Settings.AllowGroup = DefaultBoolean.False;
            }
            return columnWrapper;
        }
    }
}