using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Web;
using Xpand.ExpressApp.Web.PropertyEditors;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelOptionsUpperCase{
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool UpperCase { get; set; }
    }

    [ModelInterfaceImplementor(typeof(IModelOptionsUpperCase), "Application.Options")]
    public interface IModelClassUpperCase : IModelOptionsUpperCase{
        
    }

    [ModelInterfaceImplementor(typeof(IModelClassUpperCase), "ModelClass")]
    public interface IModelMemberUpperCase: IModelClassUpperCase {
        
    }
    [ModelInterfaceImplementor(typeof(IModelMemberUpperCase), "ModelMember")]
    public interface IModelPropertyEditorUpperCase : IModelMemberUpperCase {
    }
    public class UpperCaseController : ViewController<DetailView> ,IModelExtender{
        protected override void OnActivated() {
            base.OnActivated();
            if (View.ViewEditMode==ViewEditMode.Edit){
                var propertyEditors = View.GetItems<PropertyEditor>().Where(editor => !editor.Model.IsPassword&&((IModelPropertyEditorUpperCase) editor.Model).UpperCase);
                foreach (var propertyEditor in propertyEditors){
                    propertyEditor.ControlCreated+=PropertyEditorOnControlCreated;
                }
            }
        }

        private void PropertyEditorOnControlCreated(object sender, EventArgs eventArgs){
            AssignClientHandler(sender, "KeyUp");
            AssignClientHandler(sender, "TextChanged");
        }

        private void AssignClientHandler(object sender, string eventName){
            var webControl = GetWebControl(sender);
            if (webControl != null)
                ClientSideEventsHelper.AssignClientHandlerSafe(webControl, eventName, @"
                function (s,e){
                    s.SetText(s.inputElement.value.toUpperCase());
                }", Guid.NewGuid().ToString());
        }

        private ASPxWebControl GetWebControl(object sender){
            var propertyEditor = ((PropertyEditor) sender);
            var webControl = propertyEditor.Control as ASPxWebControl;
            if (webControl == null){
                var lookupPropertyEditor = propertyEditor as ASPxSearchLookupPropertyEditor;
                return lookupPropertyEditor?.SearchDropDownEdit.DropDown;
            }
            return null;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions, IModelOptionsUpperCase>();
            extenders.Add<IModelMember, IModelMemberUpperCase>();
            extenders.Add<IModelClass, IModelClassUpperCase>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorUpperCase>();
        }
    }
}
