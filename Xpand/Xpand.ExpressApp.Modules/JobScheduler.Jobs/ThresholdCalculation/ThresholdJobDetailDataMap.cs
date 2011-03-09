using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation {
    public enum ThresholdSeverity {
        None,
        Warning = 1,
        Minor = 2,
        Major = 3,
        Critical = 4
    }
    [CreatableItem]
    public class ThresholdJobDetailDataMap : XpandJobDetailDataMap {
        public ThresholdJobDetailDataMap(Session session)
            : base(session) {
        }
        private ThresholdSeverity _severity;
        public ThresholdSeverity Severity {
            get {
                return _severity;
            }
            set {
                SetPropertyValue("Severity", ref _severity, value);
            }
        }
        private string _criteria;
        [CriteriaObjectTypeMember("DataType")]
        [EditorAlias(EditorAliases.CriteriaPropertyEditor)]
        public string Criteria {
            get {
                return _criteria;
            }
            set {
                SetPropertyValue("Criteria", ref _criteria, value);
            }
        }
        private Type _dataType;
        [ValueConverter(typeof(TypeValueConverter))]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type DataType {
            get {
                return _dataType;
            }
            set {
                SetPropertyValue("DataType", ref _dataType, value);
            }
        }
    }
}