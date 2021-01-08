using System;

namespace Xpand.Persistent.Base.General.CustomAttributes {

    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class TooltipAttribute : Attribute, ICustomAttribute {
        public TooltipAttribute(string value) {
            Value = value;
        }


        public string Name => "Tooltip";

        public string Value { get; }
    }
}
