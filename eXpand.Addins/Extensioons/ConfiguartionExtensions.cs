using System.Linq;
using EnvDTE;
using eXpandAddIns.Enums;

namespace eXpandAddIns.Extensioons
{
    public static class ConfiguartionExtensions
    {
        public static Property FindProperty(this Configuration configuration,ConfigurationProperty configurationProperty)
        {
            return configuration.Properties.Cast<Property>().Where(property => property.Name == configurationProperty.ToString()).Single();
        }
    }
}