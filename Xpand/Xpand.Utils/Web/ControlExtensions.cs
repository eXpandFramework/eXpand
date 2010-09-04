using System.Collections.Generic;
using System.Web.UI;

namespace Xpand.Utils.Web
{
    public static class ControlExtensions
    {
        public static IEnumerable<TNestedControl> FindNestedControls<TNestedControl>(this Control container) where TNestedControl : Control
        {
            if (container.Controls != null)
                foreach (Control item in container.Controls){
                    if (item is TNestedControl)
                        yield return (TNestedControl)item;
                    foreach (TNestedControl child in FindNestedControls<TNestedControl>(item))
                        yield return child;
                }
        }
        public static IEnumerable<TNestedControl> FindNestedControls<TNestedControl>(this Control container, string id) where TNestedControl : Control
        {
            if (container.Controls != null)
                foreach (Control item in container.Controls){
                    if (item is TNestedControl &&item.ID==id)
                        yield return (TNestedControl)item;
                    foreach (TNestedControl child in FindNestedControls<TNestedControl>(item,id))
                        yield return child;
                }
        }
    }
}
