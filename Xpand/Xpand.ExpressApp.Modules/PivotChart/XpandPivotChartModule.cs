using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.PivotChart.AnalysisControlVisibility;
using Xpand.ExpressApp.PivotChart.Security;

namespace Xpand.ExpressApp.PivotChart {
    [ToolboxItem(false)]
    public sealed partial class XpandPivotChartModule : XpandModuleBase {

        public XpandPivotChartModule() {
            InitializeComponent();
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelMember, IModelMemberAnalysisControlVisibility>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorAnalysisControlVisibility>();
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null) {
                typesInfo.FindTypeInfo(typeof(ShowInAnalysisPermission)).FindMember("ControllerType").AddAttribute(new VisibleInDetailViewAttribute(false));
                typesInfo.FindTypeInfo(typeof(PivotSettingsPermission)).FindMember("ControllerType").AddAttribute(new VisibleInDetailViewAttribute(false));
            }
        }
    }
}