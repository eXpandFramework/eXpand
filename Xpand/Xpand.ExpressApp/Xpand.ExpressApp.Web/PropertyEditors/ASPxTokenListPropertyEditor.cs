using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;
using RenderHelper = DevExpress.ExpressApp.Web.RenderHelper;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(IBindingList), EditorAliases.TokenList, false)]
    public class ASPxTokenListPropertyEditor : ASPxPropertyEditor, IComplexViewItem {

        private IObjectSpace _objectSpace;
        
        private WebLookupEditorHelper _helper;

        public ASPxTokenListPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
        }

        public new ASPxTokenBox Editor => (ASPxTokenBox)base.Editor;

        public new ASPxTokenBox InplaceViewModeEditor => (ASPxTokenBox)base.InplaceViewModeEditor;

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _objectSpace = objectSpace;
            _helper = new WebLookupEditorHelper(application, objectSpace, MemberInfo.ListElementTypeInfo, Model);
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            base.BreakLinksToControl(unwireEventsOnly);
            if (Editor != null)
                Editor.TokensChanged -= EditorOnTokensChanged;
        }


        protected override WebControl CreateEditModeControlCore() {
            var asPxTokenBox = new ASPxTokenBox {
                ID = "ASPxTokenBox_control",
                IncrementalFilteringMode = IncrementalFilteringMode.Contains,
                ShowDropDownOnFocus = ShowDropDownOnFocusMode.Never,
                AllowCustomTokens = false,
                Width = new Unit("100%")
            };
            RenderHelper.SetupASPxWebControl(asPxTokenBox);
            asPxTokenBox.TokensChanged += EditorOnTokensChanged;
            return asPxTokenBox;
        }

        protected override WebControl CreateViewModeControlCore() {
            return new ASPxTokenBox {
                ClientEnabled = false,
                Width = new Unit("100%")
            };
        }

        protected override void ReadValueCore() {
            base.ReadValueCore();
            var asPxTokenBox = ViewEditMode == ViewEditMode.Edit ? Editor : InplaceViewModeEditor;
            asPxTokenBox.Value = string.Join(asPxTokenBox.ValueSeparator.ToString(), ((IEnumerable)PropertyValue).Cast<object>().Select(o => MemberInfo.ListElementTypeInfo.KeyMember.GetValue(o)));
        }

        protected override void SetupControl(WebControl control) {
            base.SetupControl(control);
            var dataSource = _helper.CreateCollectionSource(CurrentObject);
            var asPxTokenBox = (ASPxTokenBox)control;
            asPxTokenBox.DataSource = dataSource.Collection;
            var listElementTypeInfo = MemberInfo.ListElementTypeInfo;
            asPxTokenBox.TextField = listElementTypeInfo.DefaultMember?.BindingName;
            asPxTokenBox.ValueField = listElementTypeInfo.KeyMember.BindingName;
            asPxTokenBox.ItemValueType = listElementTypeInfo.KeyMember.MemberType;
            asPxTokenBox.DataBind();
        }


        private bool ItemIsTagged(object value) {
            var tagged = false;
            if (!string.IsNullOrEmpty(Editor.Value?.ToString())) {
                List<Guid> values = Editor.Value.ToString().Split(',').Select(Guid.Parse).ToList();
                if (values.Contains((Guid)value))
                    tagged = true;
            }
            return tagged;
        }

        private void EditorOnTokensChanged(object sender, EventArgs e) {
            var control = Editor;

            foreach (ListEditItem item in control.Items) {
                var obj = _objectSpace.GetObjectByKey(MemberInfo.ListElementTypeInfo.Type, item.Value);
                if (ItemIsTagged(item.Value))
                    ((IBindingList) PropertyValue).Add(obj);
                else
                    ((IBindingList)PropertyValue).Remove(obj);
            }
            OnControlValueChanged();
            _objectSpace.SetModified(CurrentObject);
        }

        protected override void SetImmediatePostDataScript(string script) {
            Editor.ClientSideEvents.TokensChanged = script;
        }

        protected override bool IsMemberSetterRequired() {
            return false;
        }
    }
}