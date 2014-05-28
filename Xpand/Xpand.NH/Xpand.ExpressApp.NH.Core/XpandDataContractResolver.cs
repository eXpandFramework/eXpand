using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.Xml;
using System.Linq;
using System.Globalization;

namespace Xpand.ExpressApp.NH.Core
{
	public class XpandDataContractResolver : DataContractResolver
	{
		private string xmlNamespace;


		public XpandDataContractResolver(string xmlNamespace)
		{
			this.xmlNamespace = xmlNamespace;
		}

		public XpandDataContractResolver()
			: this("http://schemas.expandframework.com/")
		{
		}
		public override Type ResolveName(string typeName, string typeNamespace, Type declaredType,
										 DataContractResolver knownTypeResolver)
		{
			if (declaredType == null)
				throw new ArgumentNullException("declaredType");
			if (knownTypeResolver == null)
				throw new ArgumentNullException("knownTypeResolver");

			var type = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, knownTypeResolver);
			if (type != null)
				return type;

            type = Type.GetType(typeName);
            if (type != null)
				return type;

            int lastSlashIndex = typeNamespace.LastIndexOf("/");
            if (lastSlashIndex >= 0)
            {
                string ns = typeNamespace.Substring(lastSlashIndex + 1);
                return Type.GetType(string.Format(CultureInfo.InvariantCulture, "{0}.{1}, {0}", ns, typeName));
            }

            return null;

		}

		public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver,
											out System.Xml.XmlDictionaryString typeName,
											out System.Xml.XmlDictionaryString typeNamespace)
		{

			if (type == null)
				throw new ArgumentNullException("type");
			if (declaredType == null)
				throw new ArgumentNullException("declaredType");
			if (knownTypeResolver == null)
				throw new ArgumentNullException("knownTypeResolver");

			if (knownTypeResolver.TryResolveType(type, declaredType, knownTypeResolver, out typeName, out typeNamespace))
				return true;

			if (type.IsPrimitive && declaredType == typeof(object))
			{
				return knownTypeResolver.TryResolveType(type, type, knownTypeResolver, out typeName, out typeNamespace);
			}

			XmlDictionary dict = new XmlDictionary();

			typeNamespace = dict.Add(xmlNamespace);
			typeName = dict.Add(type.AssemblyQualifiedName);

			return true;
		}

		public static void AddToEndpoint(ServiceEndpoint endpoint)
		{
			if (endpoint == null)
				throw new ArgumentNullException("endpoint");

            foreach (var operation in endpoint.Contract.Operations)
            {
                if (!operation.Behaviors.OfType<XpandDataContractSerializerOperationBehavior>().Any())
                {
                    operation.Behaviors.Remove(typeof(DataContractSerializerOperationBehavior));
                    operation.Behaviors.Add(new XpandDataContractSerializerOperationBehavior(operation));
                }
            }
		}
	}

    
}