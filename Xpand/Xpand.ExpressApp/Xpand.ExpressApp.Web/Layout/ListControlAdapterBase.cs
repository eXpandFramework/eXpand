using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Xpand.ExpressApp.Web.Layout {
    public abstract class ListControlAdapterBase<TControl> : IListControlAdapter where TControl : Control {

        private TControl control;

        protected TControl Control {
            get { return control; }
            set {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (control != null)
                    throw new InvalidOperationException("Control is already assigned.");

                control = value;
            }
        }


        Control IListControlAdapter.Control {
            get { return Control; }
            set { Control = (TControl)value; }
        }



        public abstract string CreateSetBoundsScript(string widthFunc, string heightFunc);
    }
}
