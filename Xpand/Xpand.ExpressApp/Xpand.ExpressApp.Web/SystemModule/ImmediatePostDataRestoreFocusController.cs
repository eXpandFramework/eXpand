using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using Xpand.ExpressApp.Web.PropertyEditors;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.Web.SystemModule {
    public interface IModelMemberImmediatePostDataRestoreFocus {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [DefaultValue(true)]
        bool ImmediatePostDataRestoreFocus { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberImmediatePostDataRestoreFocus), "ModelMember")]
    public interface IModelPropertyEditorImmediatePostDataRestoreFocus : IModelMemberImmediatePostDataRestoreFocus {
    }
    public class ImmediatePostDataRestoreFocusController : ViewController<DetailView>,IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            foreach (WebPropertyEditor item in View.GetItems<WebPropertyEditor>().Where(editor => editor.ImmediatePostData&&((IModelPropertyEditorImmediatePostDataRestoreFocus) editor.Model).ImmediatePostDataRestoreFocus)){
                item.ControlCreated += (s, e) => {
                    if (View.ViewEditMode==ViewEditMode.Edit)
                        AddClientSideFunctionality(s);
                };
            }
        }

        protected void AddClientSideFunctionality(object element){
            var editor = element as ASPxLookupPropertyEditor;
            if (editor != null) {
                AddClientSideFunctionalityCore(editor.DropDownEdit.DropDown);
                AddClientSideFunctionalityCore(editor.FindEdit.Editor);
            }
            else if (element is ASPxSearchLookupPropertyEditor){
                var propertyEditor = ((ASPxSearchLookupPropertyEditor) element);
                AddClientSideFunctionalityCore(propertyEditor.SearchDropDownEdit.DropDown);
            }
            else if (element is WebPropertyEditor){
                AddClientSideFunctionalityCore(((WebPropertyEditor) element).Editor as ASPxWebControl);
            }
        }

        private void AddClientSideFunctionalityCore(ASPxWebControl dxControl) {
            if (dxControl != null) {
                EventHandler loadEventHandler = (s, e) => {
                    ASPxWebControl control = (ASPxWebControl)s;
                    AddClientSideEventHandlerSafe(control, "GotFocus", "window.lastFocusedEditorId = s.inputElement.id;");
                    AddClientSideEventHandlerSafe(control, "Init", @"            
                        if (window.lastFocusedEditorId === s.inputElement.id) {
                            window.setTimeout(function () {
                                var element = document.getElementById(s.inputElement.id);
                                element.focus();
                                element.selectionStart = element.selectionEnd = 10000;
                            }, 500);
                        }");
                };
                EventHandler disposedEventHandler = null;
                disposedEventHandler = (s, e) => {
                    ASPxWebControl control = (ASPxWebControl)s;
                    control.Disposed -= disposedEventHandler;
                    control.Load -= loadEventHandler;
                };
                dxControl.Disposed += disposedEventHandler;
                dxControl.Load += loadEventHandler;
            }
        }
        private static void AddClientSideEventHandlerSafe(ASPxWebControl control, string eventName, string handler) {
            string format = @"function(s,e){{{0}}}";
            string existingHandler = control.GetClientSideEventHandler(eventName);
            if (string.IsNullOrEmpty(existingHandler)) {
                control.SetClientSideEventHandler(eventName, string.Format(format, handler));
            }
            else {
                existingHandler = $"{existingHandler.Substring(0, existingHandler.LastIndexOf('}'))}{handler}\r\n}}";
                control.SetClientSideEventHandler(eventName, existingHandler);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelMember,IModelMemberImmediatePostDataRestoreFocus>();
            extenders.Add<IModelPropertyEditor,IModelPropertyEditorImmediatePostDataRestoreFocus>();
        }
    }
}
