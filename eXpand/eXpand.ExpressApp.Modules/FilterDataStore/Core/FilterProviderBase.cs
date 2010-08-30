using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Provider;
using System.Reflection;
using eXpand.Xpo;
using System.Linq;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.FilterDataStore.Core {
    public abstract class FilterProviderBase : ProviderBase {
        [DefaultValue(StatementContext.Both)]
        public virtual StatementContext StatementContext { get; set; }


        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
            foreach (var propertyInfo in GetType().GetProperties()) {
                SetPropertyValue(config, propertyInfo);    
            }
        }

        void SetPropertyValue(NameValueCollection config, PropertyInfo propertyInfo) {
            if (!(string.IsNullOrEmpty(config[propertyInfo.Name.MakeFirstCharLower()]))){
                propertyInfo.SetValue(this,ReflectorHelper.ChangeType(config[propertyInfo.Name.MakeFirstCharLower()],propertyInfo.PropertyType), null);
            }
            else {
                var defaultValueAttribute = propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute),true).OfType<DefaultValueAttribute>().FirstOrDefault();
                if (defaultValueAttribute != null)
                    propertyInfo.SetValue(this, defaultValueAttribute.Value, null);
            }
        }

        public abstract object FilterValue { get; set; }
        public abstract string FilterMemberName { get; set; }
        public abstract int FilterMemberSize { get; set; }
        public abstract bool FilterMemberIndexed { get; set; }
        public abstract bool UseFilterValueWhenNull { get; set; }
        public abstract Type FilterMemberType { get; set; }
    }
}