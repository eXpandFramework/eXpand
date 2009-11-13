using System;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.Core
{
    public static class BOModelNodeWrapperExtensions
    {
        public static ClassInfoNodeWrapper FindClassByType<Type>(this BOModelNodeWrapper wrapper){
            return FindClassByType(wrapper,typeof(Type));
        }
        public static ClassInfoNodeWrapper FindClassByType(this BOModelNodeWrapper wrapper,Type type)
        {
            return wrapper.FindClassByName(type.FullName);
        }
    }
}
