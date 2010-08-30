using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace eXpand.ExpressApp.Core.DynamicModel {
    public class DynamicModelPropertiesSource : IDynamicModelPropertiesSource {
        readonly Type _type;
        readonly string _category;

        public DynamicModelPropertiesSource(Type type, string category) {
            _type = type;
            _category = category;
        }
        #region IDynamicModelPropertiesSource Members
        public PropertyInfo[] GetProperties() {
            var simplePropertyInfos = _type.GetProperties().Select(info => new SimplePropertyInfo(info)).ToArray();
            foreach (var simplePropertyInfo in simplePropertyInfos) {
                simplePropertyInfo.AddAttribute(new CategoryAttribute(_category));
            }
            return simplePropertyInfos;
            ;
        }
        #endregion
    }
}