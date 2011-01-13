using System.Linq;
using EnvDTE;
using XpandAddIns.Enums;

namespace XpandAddIns.Extensioons {
    public static class ProjectItemExtensions {
        public static Property FindProperty(this ProjectItem projectItem, ProjectItemProperty projectItemProperty) {
            return projectItem.Properties.Cast<Property>().Where(property => property.Name == projectItemProperty.ToString()).FirstOrDefault();
        }
    }
}
