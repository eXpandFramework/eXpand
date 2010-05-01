using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.SystemModule {
    public abstract class BaseViewController<ViewT> : BaseViewController where ViewT : View
    {
        public BaseViewController()
            : base()
        {
            this.TypeOfView = typeof(ViewT);
        }
        public new ViewT View
        {
            get { return (ViewT)base.View; }
        }
    }

    public abstract partial class BaseViewController : ViewController {
        protected BaseViewController() {
            InitializeComponent();
            RegisterActions(components);
        }
    }
}