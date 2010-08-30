using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using eXpand.ExpressApp.PivotChart.Core;
using eXpand.ExpressApp.PivotChart.Web.Options;
using eXpand.Xpo.Converters.ValueConverters;
using AnalysisPropertyEditorNodeUpdater = eXpand.ExpressApp.PivotChart.Web.Core.AnalysisPropertyEditorNodeUpdater;
using TypesInfo = eXpand.ExpressApp.PivotChart.Core.TypesInfo;

namespace eXpand.ExpressApp.PivotChart.Web {
    public sealed partial class PivotChartXpandAspNetModule : PivotChartXpandModuleBase {
        public PivotChartXpandAspNetModule() {
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