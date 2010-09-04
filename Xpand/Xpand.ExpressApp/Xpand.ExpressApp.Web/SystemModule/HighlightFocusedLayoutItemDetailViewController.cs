using System.Reflection;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxEditors;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class HighlightFocusedLayoutItemDetailViewController : HighlightFocusedLayoutItemDetailViewControllerBase
    {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (View.ViewEditMode == ViewEditMode.Edit)
                foreach (ASPxPropertyEditor pe in View.GetItems<ASPxPropertyEditor>())
                    AssignStyle(pe.Editor);
        }
        protected override void AssignStyle(object control)
        {
            EditClientSideEvents clientSideEvents = FindClientSideEvents(control);
            if (clientSideEvents != null)
            {
                const string functionGotFocus = @"
                function (s, e){
                    var el = document.getElementById(s.name);
                    el.style.backgroundColor = ""yellow"";
                }";
                const string functionLostFocus = @"
                function (s, e){
                    //debugger;
                    var el = document.getElementById(s.name);
                    el.style.backgroundColor = """";
                }";
                clientSideEvents.GotFocus = functionGotFocus;
                clientSideEvents.LostFocus = functionLostFocus;
            }
        }
        private static EditClientSideEvents FindClientSideEvents(object control)
        {
            if (control != null)
            {
                PropertyInfo pi = control.GetType().GetProperty("ClientSideEvents");
                if (pi != null)
                    return pi.GetValue(control, null) as EditClientSideEvents;
            }
            return null;
        }
    }
}