using System.Linq;
using System.Reflection;
using CThru;
using DevExpress.Persistent.Validation;

namespace eXpand.ExpressApp.WorldCreator.CThru {
    public class ExistentMembersEnableValidationAspect : Aspect {
        static FieldInfo backingField;


        public ExistentMembersEnableValidationAspect() {
            backingField =
                typeof (RulePropertyValueProperties).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(
                    info => info.Name == "propertyName").Single();
        }

        public override bool ShouldIntercept(InterceptInfo info) {
            return info.TargetInstance is RulePropertyValueProperties;
        }

        public override void MethodBehavior(DuringCallbackEventArgs e) {
            base.MethodBehavior(e);
            if (e.MethodName == "set_TargetPropertyName") {
                e.MethodBehavior = MethodBehaviors.SkipActualMethod;
                backingField.SetValue(e.TargetInstance, e.ParameterValues[0]);
            }
        }
    }
}