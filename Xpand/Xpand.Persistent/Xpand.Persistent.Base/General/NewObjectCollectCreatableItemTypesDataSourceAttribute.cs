using System;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field,AllowMultiple = false)]
    public class NewObjectCollectCreatableItemTypesDataSourceAttribute:Attribute {
        readonly string propertyName;

        public NewObjectCollectCreatableItemTypesDataSourceAttribute(string propertyName) {
            this.propertyName = propertyName;
        }

        public string PropertyName {
            get { return propertyName; }
        }
    }
}
