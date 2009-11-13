using System.Collections.Generic;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.Core
{
    public static class ClassInfoNodeWrapperExtensions
    {
        public static List<ListViewInfoNodeWrapper> GetListViews(this ClassInfoNodeWrapper wrapper)
        {
            return new ApplicationNodeWrapper(wrapper.Node.Parent.Parent).Views.GetListViews(wrapper);
        }
        public static List<DetailViewInfoNodeWrapper> GetDetailViews(this ClassInfoNodeWrapper wrapper)
        {
            return new ApplicationNodeWrapper(wrapper.Node.Parent.Parent).Views.GetDetailViews(wrapper);
        }
    }
}
