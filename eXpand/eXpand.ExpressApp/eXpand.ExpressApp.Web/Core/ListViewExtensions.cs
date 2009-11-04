using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Web.Core
{
    public static class ListviewExtensions
    {
        public static bool IsNested(this ListView listView, Frame frame)
        {
            return (frame.Template == null);
        }

    }
}
