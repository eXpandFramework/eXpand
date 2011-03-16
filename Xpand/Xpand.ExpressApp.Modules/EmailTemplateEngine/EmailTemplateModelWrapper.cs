namespace Xpand.EmailTemplateEngine {
    using System.Collections.Generic;
    using System.Dynamic;

    public class EmailTemplateModelWrapper : DynamicObject {
        private readonly IDictionary<string, object> propertyMap;

        public EmailTemplateModelWrapper(IDictionary<string, object> propertyMap) {
            Invariant.IsNotNull(propertyMap, "propertyMap");

            this.propertyMap = propertyMap;
        }

        public override IEnumerable<string> GetDynamicMemberNames() {
            return propertyMap.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            return propertyMap.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value) {
            propertyMap[binder.Name] = value;
            return true;
        }
    }
}