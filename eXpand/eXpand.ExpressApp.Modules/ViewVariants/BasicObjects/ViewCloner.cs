using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.ViewVariants.BasicObjects
{
    [NonPersistent]
    public class ViewCloner : BaseObject
    {
        public ViewCloner(Session session) : base(session) { }
        private string caption;

        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref caption, value);
            }
        }
        private string clonedViewName;
        [Browsable(false)]
        public string ClonedViewName
        {
            get
            {
                return clonedViewName;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref clonedViewName, value);
            }
        }
    }
}