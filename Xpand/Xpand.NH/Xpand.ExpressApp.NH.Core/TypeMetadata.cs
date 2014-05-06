using System;
using System.Runtime.Serialization;
using Xpand.ExpressApp.NH.Core;

namespace Xpand.ExpressApp.NH.Core
{
    [DataContract]
    public class TypeMetadata : ITypeMetadata
    {


        public Type Type
        {
            get
            {
                return Type.GetType(TypeName);
            }
            set
            {
                if (value != null)
                    TypeName = value.AssemblyQualifiedName;
                else
                    TypeName = null;
            }
        }


        [DataMember]
        public string TypeName
        {
            get;
            set;
        }

        [DataMember]
        public string KeyPropertyName
        {
            get;
            set;
        }
    }
}
