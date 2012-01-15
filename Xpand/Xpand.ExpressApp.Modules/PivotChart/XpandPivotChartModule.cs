using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using DevExpress.ExpressApp;
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
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.PivotChart");
            try {
                if (RuntimeMode) {
                    var assembly = Assembly.Load("DevExpress.Persistent.BaseImpl" + XafAssemblyInfo.VersionSuffix);
                    TypesInfo.LoadTypes(assembly);
                    var typeInfo = TypesInfo.FindTypeInfo("DevExpress.Persistent.BaseImpl.Analysis").Type;
                    AdditionalExportedTypes.Add(typeInfo);
                }
            } catch (FileNotFoundException) {
                throw new TypeLoadException("Please make sure DevExpress.Persistent.BaseImpl is referenced from your application project and has its Copy Local==true");
            }


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