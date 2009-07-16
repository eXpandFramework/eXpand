using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Core
{
    public static class ListViewExtensions
    {
        public static bool IsNested(this ListView listView, Frame frame)
        {
            return frame.Template == null;
        }

    }
}
