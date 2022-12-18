using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Validation.Controllers {
    public class ObjectSpaceObjectChangedValidationContextController:ViewController<ObjectView>{
        public const string ObjectSpaceObjectChanged = "ObjectSpaceObjectChanged";
        protected override void OnActivated(){
            base.OnActivated();
            var rulesForContext = ((IModelApplicationValidation) Application.Model).Validation.Rules.Any(rule =>
                rule is IRuleBaseProperties properties && properties.TargetType == View.ObjectTypeInfo.Type &&
                properties.TargetContextIDs == ObjectSpaceObjectChanged);
            if (rulesForContext)
                ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            if (!string.IsNullOrEmpty(objectChangedEventArgs.PropertyName)) {
                ValidateControlValueChangedContext(objectChangedEventArgs.Object);
            }
        }

        protected void ValidateControlValueChangedContext(object currentObject) {
            Validator.GetService(Site).ValidateTarget(ObjectSpace, currentObject, ObjectSpaceObjectChanged);
        }

    }
}
