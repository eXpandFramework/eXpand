using System;
using System.Web.UI;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Web.SystemModule;
using Xpand.Extensions.StreamExtensions;

[assembly: WebResource("Xpand.ExpressApp.Web.SystemModule."+HighlightFocusedLayoutItemDetailViewController.HighlightFocusedLayoutItem, "text /javascript")]
namespace Xpand.ExpressApp.Web.SystemModule {
     public class HighlightFocusedLayoutItemDetailViewController : HighlightFocusedLayoutItemDetailViewControllerBase {
         public const string HighlightFocusedLayoutItem = "HighlightFocusedLayoutItemDetailViewController.js";
         
         private const string ClientSideEventHandlerFunctionFormat = @"function(s,e){{{0}}}";
        protected override void OnActivated() {
            base.OnActivated();
            if(View.ViewEditMode == ViewEditMode.Edit) {
                foreach(WebPropertyEditor item in View.GetItems<WebPropertyEditor>()) {
                    if(item.Editor != null) {
                        ApplyFocusedStyle(item);
                    }
                    else {
                        item.ControlCreated += (s, _) => ApplyFocusedStyle(s);
                    }
                }
            }
        }

         protected override void OnViewControlsCreated(){
             base.OnViewControlsCreated();
             var script = GetType().Assembly.GetManifestResourceStream(GetType(),HighlightFocusedLayoutItem).ReadToEndAsString();
             WebWindow.CurrentRequestWindow.RegisterStartupScript(HighlightFocusedLayoutItem,script);
        }

         protected override void ApplyFocusedStyle(object element) {
             if (element is ASPxLookupPropertyEditor editor) {
                if (editor.DropDownEdit != null)
                    ApplyFocusedStyleCore(editor.DropDownEdit.DropDown);

                if (editor.FindEdit != null)
                    ApplyFocusedStyleCore(editor.FindEdit.Editor);
             }
             else {
                 if(element is WebPropertyEditor propertyEditor) {
                     ApplyFocusedStyleCore(propertyEditor.Editor as ASPxWebControl);
                 }
             }
         }

         private void ApplyFocusedStyleCore(ASPxWebControl dxControl) {
             if(dxControl != null) {
                 void LoadEventHandler(object s, EventArgs e) {
                     var control = (ASPxWebControl) s;
                     AddEventHandlerSafe(control, "Init", "window.initEditor(s,e);");
                     AddEventHandlerSafe(control, "GotFocus", "window.gotFocusEditor(s,e);");
                 }

                 void DisposedEventHandler(object s, EventArgs e) {
                     var control = (ASPxWebControl) s;
                     control.Disposed -= DisposedEventHandler;
                     control.Load -= LoadEventHandler;
                 }

                 dxControl.Disposed += DisposedEventHandler;
                 dxControl.Load += LoadEventHandler;
             }
         }
         private static void AddEventHandlerSafe(ASPxWebControl control, string eventName, string handler) {
             string existingHandler = control.GetClientSideEventHandler(eventName);
             if(String.IsNullOrEmpty(existingHandler)) {
                 control.SetClientSideEventHandler(eventName, String.Format(ClientSideEventHandlerFunctionFormat, handler));
             }
             else {
                 existingHandler = $"{existingHandler.Substring(0, existingHandler.LastIndexOf('}'))}{handler}\r\n}}";
                 control.SetClientSideEventHandler(eventName, existingHandler);
             }
         }
     }
}
