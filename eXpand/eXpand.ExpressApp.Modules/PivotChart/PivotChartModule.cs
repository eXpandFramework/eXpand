using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using TypesInfo = eXpand.ExpressApp.PivotChart.Core.TypesInfo;

namespace eXpand.ExpressApp.PivotChart {
    public sealed partial class PivotChartModule : ModuleBase
    {
        
        public PivotChartModule(){
            InitializeComponent();
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (Application != null)
                TypesInfo.Instance.AddTypes(Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null)
                typesInfo.FindTypeInfo(TypesInfo.Instance.AnalysisType).AddAttribute(new DefaultPropertyAttribute("Name"));
        }
    }
}