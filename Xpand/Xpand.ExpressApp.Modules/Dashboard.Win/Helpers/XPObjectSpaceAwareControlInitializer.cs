using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;

namespace Xpand.ExpressApp.Dashboard.Win.Helpers {
    public interface IXPObjectSpaceAwareControl {
        void UpdateDataSource(IObjectSpace objectSpace);
    }

    public class XPObjectSpaceAwareControlInitializer {
        public XPObjectSpaceAwareControlInitializer(IXPObjectSpaceAwareControl control, IObjectSpace objectSpace) {
            Guard.ArgumentNotNull(control, "control");
            Guard.ArgumentNotNull(objectSpace, "objectSpace");
            control.UpdateDataSource(objectSpace);
            objectSpace.Reloaded += (sender, args) => control.UpdateDataSource(objectSpace);
        }

        public XPObjectSpaceAwareControlInitializer(IXPObjectSpaceAwareControl control, XafApplication theApplication)
            : this(control, theApplication != null ? theApplication.CreateObjectSpace() : null) {
        }
    }
}