using System.ComponentModel;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using Xpand.ExpressApp.PivotChart.AnalysisControlVisibility;
using Xpand.ExpressApp.PivotChart.Security;
using Xpand.ExpressApp.Validation;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.PivotChart {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandPivotChartModule : XpandModuleBase {
        public XpandPivotChartModule() {
            RequiredModuleTypes.Add(typeof(PivotChartModuleBase));
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
        }

        [SecuritySafeCritical]
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.PivotChart");
            LoadDxBaseImplType("DevExpress.Persistent.BaseImpl.Analysis");
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelMember, IModelMemberAnalysisControlVisibility>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorAnalysisControlVisibility>();
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (Application != null) {
                typesInfo.FindTypeInfo(typeof(ShowInAnalysisPermission)).FindMember("ControllerType").AddAttribute(
                    new VisibleInDetailViewAttribute(false));
                typesInfo.FindTypeInfo(typeof(PivotSettingsPermission)).FindMember("ControllerType").AddAttribute(
                    new VisibleInDetailViewAttribute(false));
            }
        }
    }
}