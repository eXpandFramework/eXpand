using System.Linq;
using EnvDTE;
using Xpand.CodeRush.Plugins.Enums;

namespace Xpand.CodeRush.Plugins.Extensions {
    public static class ConfigurationExtensions {
        public static Property FindProperty(this Configuration configuration, ConfigurationProperty configurationProperty) {
            string configPropertyStr = configurationProperty.ToString();
            return configuration.Properties.Cast<Property>().Single(property => property.Name == configPropertyStr);
        }
    }
}