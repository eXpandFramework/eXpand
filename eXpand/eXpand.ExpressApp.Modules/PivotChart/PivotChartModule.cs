using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.PivotChart.AnalysisControlVisibility;
using eXpand.ExpressApp.PivotChart.Security;

namespace eXpand.ExpressApp.PivotChart {
    public sealed partial class PivotChartModule : ModuleBase
    {
        
        public PivotChartModule(){
            InitializeComponent();
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelMember, IModelMemberAnalysisControlVisibility>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorAnalysisControlVisibility>();
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