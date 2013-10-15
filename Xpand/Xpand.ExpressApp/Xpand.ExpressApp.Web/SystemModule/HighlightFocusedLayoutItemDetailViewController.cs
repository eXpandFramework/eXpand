using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxClasses;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
     public class HighlightFocusedLayoutItemDetailViewController : HighlightFocusedLayoutItemDetailViewControllerBase {
        private const string ClientSideEventHandlerFunctionFormat = @"function(s,e){{{0}}}";
        protected override void OnActivated() {
            base.OnActivated();
            if(View.ViewEditMode == ViewEditMode.Edit) {
                foreach(WebPropertyEditor item in View.GetItems<WebPropertyEditor>()) {
                    if(item.Editor != null) {
                        ApplyFocusedStyle(item);
                    }
                    else {
                        item.ControlCreated += (s, e) => ApplyFocusedStyle(s);
                    }
                }
            }
        }
        protected override void ApplyFocusedStyle(object element) {
            var editor = element as ASPxLookupPropertyEditor;
            if(editor != null) {
                ApplyFocusedStyleCore(editor.DropDownEdit.DropDown);
                ApplyFocusedStyleCore(editor.FindEdit.TextBox);
            }
            else {
                var propertyEditor = element as WebPropertyEditor;
                if(propertyEditor != null) {
                    ApplyFocusedStyleCore(propertyEditor.Editor as ASPxWebControl);
                }
            }
        }

         private void ApplyFocusedStyleCore(ASPxWebControl dxControl) {
            if(dxControl != null) {
                EventHandler loadEventHandler = (s, e) => {
                    var control = (ASPxWebControl)s;
                    AddEventHandlerSafe(control, "Init", "window.initEditor(s,e);");
                    AddEventHandlerSafe(control, "GotFocus", "window.gotFocusEditor(s,e);");
                };
                EventHandler disposedEventHandler = null;
                disposedEventHandler = (s, e) => {
                    var control = (ASPxWebControl)s;
                    control.Disposed -= disposedEventHandler;
                    control.Load -= loadEventHandler;
                };
                dxControl.Disposed += disposedEventHandler;
                dxControl.Load += loadEventHandler;
            }
        }
        private static void AddEventHandlerSafe(ASPxWebControl control, string eventName, string handler) {
            string existingHandler = control.GetClientSideEventHandler(eventName);
            if(string.IsNullOrEmpty(existingHandler)) {
                control.SetClientSideEventHandler(eventName, string.Format(ClientSideEventHandlerFunctionFormat, handler));
            }
            else {
                existingHandler = String.Format("{0}{1}\r\n}}", existingHandler.Substring(0, existingHandler.LastIndexOf('}')), handler);
                control.SetClientSideEventHandler(eventName, existingHandler);
            }
        }
    }
}
