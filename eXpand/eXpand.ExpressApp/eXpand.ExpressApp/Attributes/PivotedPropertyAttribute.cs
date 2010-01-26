using System;

namespace eXpand.ExpressApp.Attributes {
    public class PivotedPropertyAttribute:Attribute
    {
        readonly string _collectionName;

        public PivotedPropertyAttribute(string collectionName) {
            _collectionName = collectionName;
        }

        public string CollectionName {
            get { return _collectionName; }
        }
    }
}