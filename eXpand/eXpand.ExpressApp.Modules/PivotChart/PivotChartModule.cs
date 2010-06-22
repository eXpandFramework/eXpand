using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.PivotChart.Security;

namespace eXpand.ExpressApp.PivotChart {
    public sealed partial class PivotChartModule : ModuleBase
    {
        
        public PivotChartModule(){
            InitializeComponent();
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null) {
                typesInfo.FindTypeInfo(typeof (ShowInAnalysisPermission)).FindMember("ControllerType").AddAttribute(new VisibleInDetailViewAttribute(false));
                typesInfo.FindTypeInfo(typeof(PivotSettingsPermission)).FindMember("ControllerType").AddAttribute(new VisibleInDetailViewAttribute(false));
            }
        }
    }
}