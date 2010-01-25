using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.PivotChart.Core {
    internal class TypesInfo {
        static IValueManager<TypesInfo> instanceManager;


        public static TypesInfo Instance {
            get {
                if (instanceManager == null) {
                    instanceManager = ValueManager.CreateValueManager<TypesInfo>();
                }
                return instanceManager.Value ?? (instanceManager.Value = new TypesInfo());
            }
        }

        public Type AnalysisType { get; set; }


        public void AddTypes(IEnumerable<Type> types) {
            AnalysisType = GetInfoType(types, typeof (IAnalysisInfo));
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