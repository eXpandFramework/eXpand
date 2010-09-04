using System;

namespace Xpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CustomQueryPropertiesAttribute : Attribute
    {
        readonly string theName;
        readonly string theValue;
        public string Name { get { return theName; } }
        public string Value { get { return theValue; } }
        public CustomQueryPropertiesAttribute(string theName, string theValue)
        {
            this.theName = theName;
            this.theValue = theValue;
        }
    }
}