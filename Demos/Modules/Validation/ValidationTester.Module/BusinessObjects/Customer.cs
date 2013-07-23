using DevExpress.Persistent.BaseImpl;
using System;
using DevExpress.Xpo;
using DevExpress.Persistent.Validation;
using Xpand.ExpressApp.Validation;
using DevExpress.Persistent.Base;
using System.ComponentModel;

namespace ValidationTester.Module.BusinessObjects {
    [DefaultClassOptions]
    [DefaultProperty("Name")]
    [NavigationItem("Validation")]
    [DeferredDeletion(false)]
    public class Customer : BaseObject {
        public Customer(Session session) : base(session) { }

        // Fields...
        private DateTime _registration;
        private int _age;
        private string _city;
        private string _name;

        [RuleRequiredField("Name_rulereq", DefaultContexts.Save, "Everybody has a name!")]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [RuleRequiredField("City_WarningProperty", DefaultContexts.Save)]
        [RuleErrorType("City_WarningProperty", RuleType.Warning)]
        public string City {
            get { return _city; }
            set { SetPropertyValue("City", ref _city, value); }
        }

        [RuleRange("Age_InformationProperty", DefaultContexts.Save, 10, 60, CustomMessageTemplate = "Too old or too young.")]
        [RuleErrorType("Age_InformationProperty", RuleType.Information)]
        public int Age {
            get { return _age; }
            set { SetPropertyValue("Age", ref _age, value); }
        }

        [RuleValueComparison("Registration_ValueChangedProperty", RuleTypeController.ObjectSpaceObjectChanged,
            ValueComparisonType.GreaterThanOrEqual, "LocalDateTimeToday()", ParametersMode.Expression, CustomMessageTemplate = "Registration before today?",SkipNullOrEmptyValues = false)]
        [RuleErrorType("Registration_ValueChangedProperty", RuleType.Information)]
        [ImmediatePostData]
        public DateTime Registration {
            get { return _registration; }
            set { SetPropertyValue("Registration", ref _registration, value); }
        }
    }
}
