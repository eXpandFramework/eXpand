namespace eXpand.ExpressApp.PivotChart.Win.Controllers {
    public class AnalysisReadOnlyController:DevExpress.ExpressApp.PivotChart.AnalysisReadOnlyController {
        protected override void OnActivated()
        {
            base.OnActivated();
            Active[GetType().FullName] = false;
        }   
    }
}