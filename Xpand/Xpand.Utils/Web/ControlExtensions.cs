using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Xpand.Utils.Web {
    public static class ControlExtensions {
        public static IEnumerable<Control> FindNestedControls(this Control container,Type controlType){
            foreach (Control control in container.Controls) {
                if (controlType.IsInstanceOfType(control))
                    yield return control;
                foreach (var child in control.FindNestedControls(controlType))
                    yield return child;
            }
        }

        public static IEnumerable<TNestedControl> FindNestedControls<TNestedControl>(this Control container) where TNestedControl : Control{
            return container.FindNestedControls(typeof (TNestedControl)).Cast<TNestedControl>();
        }

        public static IEnumerable<TNestedControl> FindNestedControls<TNestedControl>(this Control container, string id) where TNestedControl : Control{
            return container.FindNestedControls<TNestedControl>().Where(control => control.ID == id);
        }
    }
}
