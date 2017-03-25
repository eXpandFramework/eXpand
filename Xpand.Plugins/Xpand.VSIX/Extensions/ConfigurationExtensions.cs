using System.Configuration;
using System.Linq;
using EnvDTE;
using Configuration = EnvDTE.Configuration;

namespace Xpand.VSIX.Extensions{
    public enum ConfigurationProperty {
        OutputPath
    }

    public static class ConfigurationExtensions {
        public static Property FindProperty(this Configuration configuration, ConfigurationProperty configurationProperty) {
            string configPropertyStr = configurationProperty.ToString();
            return configuration.Properties.Cast<Property>().Single(property => property.Name == configPropertyStr);
        }
    }
}