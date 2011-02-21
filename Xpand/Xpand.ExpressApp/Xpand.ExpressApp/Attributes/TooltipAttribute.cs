using System;

namespace Xpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class TooltipAttribute :Attribute {
        readonly string toolTip;

        public TooltipAttribute(string toolTip) {
            this.toolTip = toolTip;
        }

        public string ToolTip {
            get { return toolTip; }
        }
    }
}
