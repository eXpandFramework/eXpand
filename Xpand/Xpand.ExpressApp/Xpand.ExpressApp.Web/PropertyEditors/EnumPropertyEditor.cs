using System;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Web;
using Xpand.Persistent.Base.General.CustomAttributes;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(Enum), EditorAliases.EnumPropertyEditor, false)]
    public class ASPxEnumPropertyEditor : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxEnumPropertyEditor,IComplexViewItem,IEnumPropertyEditor {
        private IObjectSpace _objectSpace;

        public ASPxEnumPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
            CurrentObjectChanged+=OnCurrentObjectChanged;
        }

        private void OnCurrentObjectChanged(object sender, EventArgs e) {
            SetupDataSource(Editor);
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            base.BreakLinksToControl(unwireEventsOnly);
            CurrentObjectChanged-=OnCurrentObjectChanged;
            _objectSpace.Committed-=ObjectSpaceOnCommitted;
        }

        protected override void SetupControl(WebControl control) {
            base.SetupControl(control);
            SetupDataSource(control);
        }

        private void SetupDataSource(WebControl control){
            if (control != null&&ViewEditMode==ViewEditMode.Edit){
                var listEditItemCollection = ((ASPxComboBox) control).Items;
                var startitems = listEditItemCollection.ToArray();
                MemberInfo.SetupEnumPropertyDataSource(CurrentObject,_objectSpace,  startitems, listEditItemCollection, listItem => listItem.Value);
            }
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _objectSpace = objectSpace;
            _objectSpace.Committed+=ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs e) {
            SetupDataSource(Editor);
        }
    }
}