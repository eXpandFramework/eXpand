using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.SystemModule
{
    [Browsable(false)]
    public  partial class BaseViewController : ViewController
    {
        public BaseViewController()
        {
            InitializeComponent();
            RegisterActions((IContainer) components);
        }


        protected ClassInfoNodeWrapper GetClassInfoNodeWrapper()
        {
            return new ApplicationNodeWrapper(Application.Info).BOModel.Classes.Where(wrapper => wrapper.ClassTypeInfo==View.ObjectTypeInfo).Single();
        }
    }
}