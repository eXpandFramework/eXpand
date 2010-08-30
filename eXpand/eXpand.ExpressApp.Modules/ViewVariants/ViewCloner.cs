using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.ViewVariants {
    [NonPersistent]
    public class ViewCloner : BaseObject {
        string caption;

        string clonedViewName;

        public ViewCloner(Session session) : base(session) {
        }

        public string Caption {
            get { return caption; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref caption, value); }
        }

        [Browsable(false)]
        public string ClonedViewName {
            get { return clonedViewName; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref clonedViewName, value); }
        }
    }
}