using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace SystemTester.Module.Web.Controllers {
    public class ModelDifferenceController:ObjectViewController<ListView,DevExpress.Persistent.BaseImpl.ModelDifference>{

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            ((ASPxGridListEditor) View.Editor).Grid.ClientSideEvents.Init = @"function (s,e){s.MoveColumn(s.GetColumn(0));}";
        }

    }
}
