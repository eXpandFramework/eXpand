using System.Linq;
using EnvDTE;
using XpandAddIns.Enums;

namespace XpandAddIns.Extensions {
    public static class ConfiguartionExtensions {
        public static Property FindProperty(this Configuration configuration, ConfigurationProperty configurationProperty) {
            return configuration.Properties.Cast<Property>().Where(property => property.Name == configurationProperty.ToString()).Single();
        }
    }
}