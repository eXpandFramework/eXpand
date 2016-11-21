using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Utils;
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
                var propertyEditors = View.GetItems<WebPropertyEditor>().Where(editor => !editor.Model.IsPassword&&((IModelPropertyEditorUpperCase) editor.Model).UpperCase);
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
            foreach (var editor in ((WebPropertyEditor) sender).GetEditors()){
                ClientSideEventsHelper.AssignClientHandlerSafe(editor, eventName, @"
                function (s,e){
                    console.log(s.GetText());
                    s.inputElement.value=s.inputElement.value.toUpperCase();
                }", Guid.NewGuid().ToString());
            }
        }


        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions, IModelOptionsUpperCase>();
            extenders.Add<IModelMember, IModelMemberUpperCase>();
            extenders.Add<IModelClass, IModelClassUpperCase>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorUpperCase>();
        }
    }
}
