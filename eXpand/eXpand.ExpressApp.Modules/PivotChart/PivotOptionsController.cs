using AnalysisViewControllerBase = eXpand.ExpressApp.PivotChart.AnalysisViewControllerBase;

namespace eXpand.ExpressApp.PivotChart
{
    public partial class PivotOptionsController : AnalysisViewControllerBase
    {
        public PivotOptionsController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
    }
}
