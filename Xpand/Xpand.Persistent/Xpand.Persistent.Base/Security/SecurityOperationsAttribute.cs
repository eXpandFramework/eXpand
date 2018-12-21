using System;

namespace Xpand.Persistent.Base.Security{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SecurityOperationsAttribute : Attribute {
        public SecurityOperationsAttribute(string collectionName, string operationProviderProperty) {
            CollectionName = collectionName;
            OperationProviderProperty = operationProviderProperty;
        }

        public string CollectionName { get; }

        public string OperationProviderProperty { get; }
    }
}