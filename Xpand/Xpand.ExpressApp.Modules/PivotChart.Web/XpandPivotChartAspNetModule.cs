using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using Xpand.ExpressApp.PivotChart.Core;
using Xpand.ExpressApp.PivotChart.Web.Options;
using Xpand.Xpo.Converters.ValueConverters;
using AnalysisPropertyEditorNodeUpdater = Xpand.ExpressApp.PivotChart.Web.Core.AnalysisPropertyEditorNodeUpdater;
using TypesInfo = Xpand.ExpressApp.PivotChart.Core.TypesInfo;

namespace Xpand.ExpressApp.PivotChart.Web {
    public sealed partial class XpandPivotChartAspNetModule : XpandPivotChartModuleBase {
        public XpandPivotChartAspNetModule() {
            InitializeComponent();
        }
        protected override IMemberInfo OnCreateMember(ITypeInfo typeInfo, string name, Type propertyType)
        {
            var memberInfo = base.OnCreateMember(typeInfo, name, propertyType);
            if (propertyType == typeof(Unit))
                memberInfo.AddAttribute(new ValueConverterAttribute(typeof(UnitValueConverter)));
            return memberInfo;
        }

        protected override IModelNodesGeneratorUpdater GetAnalysisPropertyEditorNodeUpdater() {
            return new AnalysisPropertyEditorNodeUpdater();
        }

        public override TypesInfo TypesInfo {
            get { return Core.TypesInfo.Instance; }
        }


        protected override Dictionary<Type, Type> GetOptionsMapperDictionary() {
            return PivotGridOptionMapper.Instance.Dictionary;
        }
    }

}