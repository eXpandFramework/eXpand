using System;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider {
    public class TypeDecorator : Attribute {
        readonly Type controlType;

        public TypeDecorator(Type controlType) {
            this.controlType = controlType;
        }

        public Type ControlType {
            get { return controlType; }
        }
    }
}