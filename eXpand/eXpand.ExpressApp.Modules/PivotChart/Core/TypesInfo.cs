using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.PivotChart;

namespace eXpand.ExpressApp.PivotChart.Core {
    public class TypesInfo {
        static IValueManager<TypesInfo> instanceManager;


        public static TypesInfo Instance {
            get {
                if (instanceManager == null) {
                    instanceManager = ValueManager.CreateValueManager<TypesInfo>();
                }
                return instanceManager.Value ?? (instanceManager.Value = new TypesInfo());
            }
        }


        public Type PivotOptionsViewType { get; set; }
        public Type PivotOptionsMenuType { get; set; }

        public Type PivotOptionsHintType { get; set; }
        public Type PivotOptionsFilterPopupType { get; set; }
        public Type PivotOptionsDataFieldType { get; set; }
        public Type PivotOptionsDataType { get; set; }
        public Type PivotOptionsCustomizationType { get; set; }
        public Type PivotOptionsChartDataSourceType { get; set; }
        public Type PivotOptionsBehaviorType { get; set; }
        public Type AnalysisType { get; set; }
        public Type PivotOptionsSelectionType { get; set; }


        public void AddTypes(IEnumerable<Type> types) {
            AnalysisType = GetInfoType(types, typeof (IAnalysisInfo));
            PivotOptionsBehaviorType = GetInfoType(types, typeof (IPivotOptionsBehavior));
            PivotOptionsChartDataSourceType = GetInfoType(types, typeof (IPivotOptionsChartDataSource));
            PivotOptionsCustomizationType = GetInfoType(types, typeof (IPivotOptionsCustomization));
            PivotOptionsDataFieldType = GetInfoType(types, typeof (IPivotOptionsDataField));
            PivotOptionsDataType = GetInfoType(types, typeof (IPivotOptionsData));
            PivotOptionsFilterPopupType = GetInfoType(types, typeof (IPivotOptionsFilterPopup));
            PivotOptionsHintType = GetInfoType(types, typeof (IPivotOptionsHint));
            PivotOptionsMenuType = GetInfoType(types, typeof (IPivotOptionsMenu));
            PivotOptionsSelectionType = GetInfoType(types, typeof (IPivotOptionsSelection));
            PivotOptionsViewType = GetInfoType(types, typeof (IPivotOptionsView));
        }

        Type GetInfoType(IEnumerable<Type> types, Type type1) {
            Type infoType =
                types.Where(type1.IsAssignableFrom).GroupBy(type => type).Select(grouping => grouping.Key).
                    FirstOrDefault();
            if (infoType == null)
                throw new NoNullAllowedException("No Class that implemenets " + type1.AssemblyQualifiedName +
                                                 " found at AdditionalBusinessClasses list. " +
                                                 typeof (PivotChartModule).Name +
                                                 " should be the last module added to Application.Modules. Please check InitializeComponent method of your XafApplication descenant");
            return infoType;
        }
    }
}