using System.ComponentModel;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class BaseWindowController : WindowController
    {
        public BaseWindowController()
        {
            InitializeComponent();
            RegisterActions((IContainer) components);
        }
    }
}