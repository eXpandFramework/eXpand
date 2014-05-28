using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.BaseImpl
{
    [DataContract]
    [DefaultClassOptions]
    public sealed class User : UserBase<Role>
    {
    }
}
