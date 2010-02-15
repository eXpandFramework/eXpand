namespace eXpand.ExpressApp.PivotChart {
    public class AnalysisReadOnlyController:DevExpress.ExpressApp.PivotChart.AnalysisReadOnlyController {
        protected override void OnActivated()
        {
            base.OnActivated();
            Active[GetType().FullName] = false;
        }   
    }
}