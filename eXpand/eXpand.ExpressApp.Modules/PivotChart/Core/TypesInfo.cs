using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.PivotChart;

namespace eXpand.ExpressApp.PivotChart.Core {
    public abstract class TypesInfo { 
        public Type AnalysisType { get; set; }

        public Type PivotOptionsDataFieldType { get; set; }
        public Type PivotOptionsDataType { get; set; }
        public virtual void AddTypes(IEnumerable<Type> types) {
            AnalysisType = GetInfoType(types, typeof (IAnalysisInfo));
            PivotOptionsDataFieldType = GetInfoType(types, typeof(IPivotOptionsDataField));
            PivotOptionsDataType = GetInfoType(types, typeof(IPivotOptionsData));
        }

        protected Type GetInfoType(IEnumerable<Type> types, Type type1) {
            Type infoType =
                types.Where(type1.IsAssignableFrom).GroupBy(type => type).Select(grouping => grouping.Key).
                    FirstOrDefault();
            if (infoType == null)
                throw new NoNullAllowedException("No Class that implemenets " + type1.AssemblyQualifiedName +
                                                 " found at AdditionalBusinessClasses list. " +
                                                 typeof (XpandPivotChartModule).Name +
                                                 " should be the last module added to Application.Modules. Please check InitializeComponent method of your XafApplication descenant");
            return infoType;
        }
    }
}