using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TestEntities
{
    [DataContract(IsReference = true)]
    public class PhoneNumber
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public Person Person { get; set; }
    }
}
