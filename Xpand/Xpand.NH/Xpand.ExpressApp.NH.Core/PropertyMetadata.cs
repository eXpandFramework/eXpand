using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xpand.ExpressApp.NH.Core
{
    [DataContract]
    public class PropertyMetadata : IPropertyMetadata
    {
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public RelationType RelationType
        {
            get;
            set;
        }
    }
}
