using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    public interface ISortPropertyInfo
    {
        string PropertyName { get; set; }
        bool Descending { get; set; }
    }
}
