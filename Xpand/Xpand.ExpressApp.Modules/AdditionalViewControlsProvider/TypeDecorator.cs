using System;
using Xpand.Persistent.Base.AdditionalViewControls;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider {
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class TypeDecorator : Attribute {
        readonly Type _controlType;
        readonly Type _defaultType;

        public TypeDecorator(Type controlType, Type defaultType, bool isDefaultDecorator) {
            _controlType = controlType;
            _defaultType = defaultType;
            IsDefaultDecorator = isDefaultDecorator;
        }

        public bool IsDefaultDecorator { get; }

        public TypeDecorator(Type controlType) {
            _controlType = controlType;
        }

        public Type DefaultType => _defaultType??_controlType;

        public Type ControlType => _controlType;

        public Position Position { get; set; }
    }
}