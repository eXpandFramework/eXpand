using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Validation.Controllers {
    public class ObjectSpaceObjectChangedValidationContextController:ViewController<ObjectView>{
        public const string ObjectSpaceObjectChanged = "ObjectSpaceObjectChanged";
        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
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
            Validator.RuleSet.ValidateAll(ObjectSpace, new List<object> { currentObject }, ObjectSpaceObjectChanged);
        }

    }
}
