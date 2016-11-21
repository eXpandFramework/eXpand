using System.Collections.Generic;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    public static class WebPropertyEditorsExtensions {
        public static IEnumerable<ASPxWebControl> GetEditors(this WebPropertyEditor propertyEditor){
            var lookupPropertyEditor = propertyEditor as ASPxLookupPropertyEditor;
            if (lookupPropertyEditor != null){
                yield return lookupPropertyEditor.DropDownEdit.DropDown;
                yield return lookupPropertyEditor.FindEdit.Editor;
                yield break;
            }
            var searchLookupPropertyEditor = propertyEditor as ASPxSearchLookupPropertyEditor;
            if (searchLookupPropertyEditor != null){
                yield return searchLookupPropertyEditor.SearchDropDownEdit.DropDown;
                yield break;
            }
            yield return propertyEditor.Editor as ASPxWebControl;
        }

    }
}
