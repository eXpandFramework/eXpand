using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using eXpand.ExpressApp.PivotChart.Web.Core;
using eXpand.Xpo.Converters.ValueConverters;
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
        public override TypesInfo TypesInfo {
            get { return Core.TypesInfo.Instance; }
        }

        protected override Type GetPropertyEditorType() {
            return typeof(AnalysisEditorWeb);
        }

        protected override Dictionary<Type, Type> GetOptionsMapperDictionary() {
            return PivotGridOptionMapper.Instance.Dictionary;
        }
    }

}