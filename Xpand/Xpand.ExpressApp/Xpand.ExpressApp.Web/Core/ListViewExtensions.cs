using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Web.Core
{
    public static class ListviewExtensions
    {
        public static bool IsNested(this XpandListView xpandListView, Frame frame)
        {
            return (frame.Template == null);
        }

    }
}
