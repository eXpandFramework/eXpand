using System;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.PivotChart.Web;

namespace eXpand.ExpressApp.PivotChart.Web.Core {
    public class TypesInfo : PivotChart.Core.TypesInfo {
        static IValueManager<TypesInfo> instanceManager;

        public static TypesInfo Instance {
            get {
                if (instanceManager == null) {
                    instanceManager = ValueManager.CreateValueManager<TypesInfo>();
                }
                return instanceManager.Value ?? (instanceManager.Value = new TypesInfo());
            }
        }

        public Type PivotGridWebOptionsChartDataSourceType { get; set; }
        public Type PivotGridWebOptionsCustomizationType { get; set; }
        public Type PivotGridWebOptionsLoadingPanelType { get; set; }
        public Type PivotGridOptionsOLAPType { get; set; }
        public Type PivotGridWebOptionsPagerType { get; set; }
        public Type PivotGridWebOptionsViewType { get; set; }
        public override void AddTypes(System.Collections.Generic.IEnumerable<Type> types)
        {
            base.AddTypes(types);
            PivotGridOptionsOLAPType = GetInfoType(types, typeof(IPivotGridOptionsOLAP));
            PivotGridWebOptionsChartDataSourceType = GetInfoType(types, typeof(IPivotGridWebOptionsChartDataSource));
            PivotGridWebOptionsCustomizationType = GetInfoType(types, typeof(IPivotGridWebOptionsCustomization));
            PivotGridWebOptionsLoadingPanelType = GetInfoType(types, typeof(IPivotGridWebOptionsLoadingPanel));
            PivotGridWebOptionsPagerType = GetInfoType(types, typeof(IPivotGridWebOptionsPager));
            PivotGridWebOptionsViewType = GetInfoType(types, typeof(IPivotGridWebOptionsView));
        }
    }

    
}