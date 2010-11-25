using System;

namespace Xpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true)]
    public sealed class CustomQueryPropertyAttribute : Attribute
    {
        readonly string _name;
        readonly Type _type;
        public string Name { get { return _name; } }
        public Type Type { get { return _type; } }
        public CustomQueryPropertyAttribute(string name, Type type) {
            _name = name;
            _type = type;
        }
    }
}