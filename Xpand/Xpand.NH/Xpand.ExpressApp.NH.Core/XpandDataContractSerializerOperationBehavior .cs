using System;
using System.Runtime.Serialization;
using System.ServiceModel.Description;

namespace Xpand.ExpressApp.NH.Core
{
    public class XpandDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {
        public XpandDataContractSerializerOperationBehavior(OperationDescription operationDescription)
            : base(operationDescription)
        {

        }

        public override XmlObjectSerializer CreateSerializer(Type type, System.Xml.XmlDictionaryString name, System.Xml.XmlDictionaryString ns, System.Collections.Generic.IList<Type> knownTypes)
        {
            return new DataContractSerializer(type, name, ns, knownTypes, int.MaxValue, false, true, null, new XpandDataContractResolver());
        }
    }
}
