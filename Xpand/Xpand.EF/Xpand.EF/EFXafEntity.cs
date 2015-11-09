using System;
using System.ComponentModel;

namespace Xpand.ExpressApp {
    public class EFXafEntity : EFBaseObject {
        Int32 _ID;
        [Browsable(false)]
        public Int32 ID {
            get { return _ID; }
            set { SetPropertyValue("ID", ref _ID, value); }
        }
    }
}
