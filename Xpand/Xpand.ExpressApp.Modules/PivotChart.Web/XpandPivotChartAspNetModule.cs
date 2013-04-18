using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.PivotChart.Web;
using DevExpress.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.PivotChart.Core;
using Xpand.ExpressApp.PivotChart.Web.Options;
using Xpand.Xpo.Converters.ValueConverters;
using AnalysisPropertyEditorNodeUpdater = Xpand.ExpressApp.PivotChart.Web.Core.AnalysisPropertyEditorNodeUpdater;
using TypesInfo = Xpand.ExpressApp.PivotChart.Core.TypesInfo;

namespace Xpand.ExpressApp.PivotChart.Web {
    [ToolboxBitmap(typeof(PivotChartAspNetModule), "Resources.Toolbox_Module_PivotChart_Web.ico")]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabModules)]
    public sealed class XpandPivotChartAspNetModule : XpandPivotChartModuleBase {
        public XpandPivotChartAspNetModule() {
            RequiredModuleTypes.Add(typeof(XpandPivotChartModule));
            RequiredModuleTypes.Add(typeof(PivotChartModuleBase));
            RequiredModuleTypes.Add(typeof(PivotChartAspNetModule));
        }

        public override TypesInfo TypesInfo {
            get { return Core.TypesInfo.Instance; }
        }

        protected override IMemberInfo OnCreateMember(ITypeInfo typeInfo, string name, Type propertyType) {
            IMemberInfo memberInfo = base.OnCreateMember(typeInfo, name, propertyType);
            if (propertyType == typeof(Unit))
                memberInfo.AddAttribute(new ValueConverterAttribute(typeof(UnitValueConverter)));
            return memberInfo;
        }

        protected override IModelNodesGeneratorUpdater GetAnalysisPropertyEditorNodeUpdater() {
            return new AnalysisPropertyEditorNodeUpdater();
        }


        protected override Dictionary<Type, Type> GetOptionsMapperDictionary() {
            return PivotGridOptionMapper.Instance.Dictionary;
        }
    }
}