using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.Xpo {
    public static class Extensions {
        public static IObjectSpace XPObjectSpace(this object xpObject){
            return DevExpress.ExpressApp.Xpo.XPObjectSpace.FindObjectSpaceByObject(xpObject);
        }
    }
}
