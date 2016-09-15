using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Web;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class AutoCommitController : ExpressApp.SystemModule.AutoCommitController {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var listView =  View as ListView;
            if (((IModelObjectViewAutoCommit)View.Model).AutoCommit) {
                ASPxGridListEditor gridListEditor = listView?.Editor as ASPxGridListEditor;
                if (gridListEditor != null) {
                    if (gridListEditor.IsBatchMode) {
                        GridViewClientSideEvents gridViewClientSideEvents;
                        ClientSideEventsHelper.AssignClientHandlerSafe(gridListEditor.Grid, nameof(gridViewClientSideEvents.Init), GetInitScript(), "grid.Init");
                        ClientSideEventsHelper.AssignClientHandlerSafe(gridListEditor.Grid, nameof(gridViewClientSideEvents.BatchEditStartEditing), "function(s, e) { clearTimeout(s.timerHandle); }", "grid.BatchEditStartEditing");
                        ClientSideEventsHelper.AssignClientHandlerSafe(gridListEditor.Grid, nameof(gridViewClientSideEvents.BatchEditEndEditing), "function(s, e) { s.timerHandle = setTimeout(function() { s.UpdateEdit();}, 100); }", "grid.BatchEditEndEditing");
                        ClientSideEventsHelper.AssignClientHandlerSafe(gridListEditor.Grid, nameof(gridViewClientSideEvents.EndCallback), GetInitScript(), "grid.EndCallback");
                    }
                }
            }
        }

        private string GetInitScript() {
            return @"function(s, e) { 
                        s.timerHandle = -1; 
                        for (var i = 0; i < s.GetColumnsCount() ; i++) {
                            var editor = s.GetEditor(i);
				            if (!!editor)
				                ASPxClientUtils.AttachEventToElement(editor.GetMainElement(), ""mouseleave"", function(){s.batchEditApi.EndEdit();s.UpdateEdit();});
                        }
                    }";
        }
    }
}