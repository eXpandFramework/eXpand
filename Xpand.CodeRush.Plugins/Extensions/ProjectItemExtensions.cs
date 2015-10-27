using System.Linq;
using EnvDTE;
using Xpand.CodeRush.Plugins.Enums;

namespace Xpand.CodeRush.Plugins.Extensions {
    public static class ProjectItemExtensions {
        public static Property FindProperty(this ProjectItem projectItem, ProjectItemProperty projectItemProperty) {
            string projectItemPropertyStr = projectItemProperty.ToString();
            return projectItem.Properties.Cast<Property>().FirstOrDefault(property => property.Name == projectItemPropertyStr);
        }
    }
}
