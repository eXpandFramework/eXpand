using System;
using System.ComponentModel;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.ValueConverters;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler.Jobs.ThresholdCalculation{
    public enum ThresholdSeverity{
        None,
        Warning = 1,
        Minor = 2,
        Major = 3,
        Critical = 4
    }

    [CreatableItem]
    public class ThresholdJobDetailDataMap : XpandJobDetailDataMap{
        private string _criteria;
        private Type _dataType;
        private ThresholdSeverity _severity;

        public ThresholdJobDetailDataMap(Session session)
            : base(session){
        }

        public ThresholdSeverity Severity{
            get { return _severity; }
            set { SetPropertyValue("Severity", ref _severity, value); }
        }

        [CriteriaOptions("DataType")]
        [EditorAlias(EditorAliases.CriteriaPropertyEditor)]
        [RuleRequiredField]
        public string Criteria{
            get { return _criteria; }
            set { SetPropertyValue("Criteria", ref _criteria, value); }
        }

        [ValueConverter(typeof (TypeValueConverter))]
        [TypeConverter(typeof (LocalizedClassInfoTypeConverter))]
        [RuleRequiredField]
        public Type DataType{
            get { return _dataType; }
            set { SetPropertyValue("DataType", ref _dataType, value); }
        }
    }
}