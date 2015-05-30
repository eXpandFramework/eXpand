using Fasterflect;

namespace Xpand.ExpressApp.EasyTest.WebAdapter {
    public static class Extensions {
        public static void CreateBrowser(this XpandTestWebAdapter adapter,string url) {
            adapter.CallMethod("CreateBrowser", url);
        }
    }
}
