namespace eXpand.ExpressApp.PivotChart.Web {
    public class AnalysisControlWeb : DevExpress.ExpressApp.PivotChart.Web.AnalysisControlWeb {
        protected override void ApplyReadOnly() {
        }
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
//            PivotGrid.ClientSideEvents.CellClick = "function(s, e) {alert('');}";
//            PivotGrid.ClientSideEvents.CellDblClick = "function(s, e) {alert('');}";
        }
    }
}