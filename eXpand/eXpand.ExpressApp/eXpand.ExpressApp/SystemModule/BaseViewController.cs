using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.SystemModule {
    public abstract class BaseViewController<ViewT> : BaseViewController where ViewT : View {
        protected BaseViewController() {
            TypeOfView = typeof (ViewT);
        }

        public new ViewT View {
            get { return (ViewT) base.View; }
        }
    }

    public abstract class BaseViewController : ViewController {
    }
}