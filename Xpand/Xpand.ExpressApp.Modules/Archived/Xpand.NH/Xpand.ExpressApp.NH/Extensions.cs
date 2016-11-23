using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH
{
    public static class Extensions
    {
        public static TResult WhenNotNull<T,TResult>(this T obj,  Func<T, TResult> func) where T:class
        {
            if (obj != null)
                return func(obj);
            else
                return default(TResult);
        }
    }
}
