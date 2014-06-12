using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xpand.ExpressApp.NH.Core;

namespace Xpand.ExpressApp.NH.Core
{
    [DataContract]
    public class TypeMetadata : ITypeMetadata
    {

        private List<IPropertyMetadata> properties;

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
        public IPropertyMetadata KeyProperty
        {
            get;
            set;
        }

        [DataMember]
        public IList<IPropertyMetadata> Properties
        {
            get
            {
                if (properties == null)
                    properties = new List<IPropertyMetadata>();

                return properties;
            }
        }
    }
}
