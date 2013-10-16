using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
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
            try {
                if (RuntimeMode) {
                    AppDomain.CurrentDomain.AssemblyResolve += DXAssemblyResolve;
                    Assembly assembly = Assembly.Load("DevExpress.Persistent.BaseImpl" + XafAssemblyInfo.VersionSuffix);
                    TypesInfo.LoadTypes(assembly);
                    var info = TypesInfo.FindTypeInfo("DevExpress.Persistent.BaseImpl.Analysis");
                    if (info==null)
                        throw new FileNotFoundException();
                    Type typeInfo = info.Type;
                    AdditionalExportedTypes.Add(typeInfo);
                }
            }
            catch (FileNotFoundException) {
                throw new FileNotFoundException(
                    "Please make sure DevExpress.Persistent.BaseImpl is referenced from your application project and has its Copy Local==true");
            }
            finally {
                AppDomain.CurrentDomain.AssemblyResolve-=DXAssemblyResolve;
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
                typesInfo.FindTypeInfo(typeof(ShowInAnalysisPermission)).FindMember("ControllerType").AddAttribute(
                    new VisibleInDetailViewAttribute(false));
                typesInfo.FindTypeInfo(typeof(PivotSettingsPermission)).FindMember("ControllerType").AddAttribute(
                    new VisibleInDetailViewAttribute(false));
            }
        }
    }
}