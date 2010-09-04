using System;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.ExpressApp.PivotChart.Win.Core
{
    public class TypesInfo:PivotChart.Core.TypesInfo
    {
        static IValueManager<TypesInfo> instanceManager;


        public static TypesInfo Instance
        {
            get
            {
                if (instanceManager == null)
                {
                    instanceManager = ValueManager.CreateValueManager<TypesInfo>();
                }
                return instanceManager.Value ?? (instanceManager.Value = new TypesInfo());
            }
        }
        public Type PivotOptionsViewType { get; set; }
        public Type PivotOptionsMenuType { get; set; }
        public Type PivotOptionsHintType { get; set; }
        public Type PivotOptionsFilterPopupType { get; set; }
        
        public Type PivotOptionsCustomizationType { get; set; }
        public Type PivotOptionsChartDataSourceType { get; set; }
        public Type PivotOptionsBehaviorType { get; set; }
        public Type PivotOptionsSelectionType { get; set; }
        public override void AddTypes(IEnumerable<Type> types) {
            base.AddTypes(types);
            PivotOptionsBehaviorType = GetInfoType(types, typeof(IPivotOptionsBehavior));
            PivotOptionsChartDataSourceType = GetInfoType(types, typeof(IPivotOptionsChartDataSource));
            PivotOptionsCustomizationType = GetInfoType(types, typeof(IPivotOptionsCustomization));
            PivotOptionsFilterPopupType = GetInfoType(types, typeof(IPivotOptionsFilterPopup));
            PivotOptionsHintType = GetInfoType(types, typeof(IPivotOptionsHint));
            PivotOptionsMenuType = GetInfoType(types, typeof(IPivotOptionsMenu));
            PivotOptionsSelectionType = GetInfoType(types, typeof(IPivotOptionsSelection));
            PivotOptionsViewType = GetInfoType(types, typeof(IPivotOptionsView));
        }
    }
}
