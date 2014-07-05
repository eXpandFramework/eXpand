using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    [DataContract]
    public class SortPropertyInfo : ISortPropertyInfo
    {

        [DataMember]
        public string PropertyName
        {
            get;
            set;
        }

        [DataMember]
        public bool Descending
        {
            get;
            set;
        }
    }
}
