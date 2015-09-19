using System.Linq;
using EnvDTE;
using Xpand.CodeRush.Plugins.Enums;

namespace Xpand.CodeRush.Plugins.Extensions {
    public static class ConfiguartionExtensions {
        public static Property FindProperty(this Configuration configuration, ConfigurationProperty configurationProperty) {
            return configuration.Properties.Cast<Property>().Single(property => property.Name == configurationProperty.ToString());
        }
    }
}