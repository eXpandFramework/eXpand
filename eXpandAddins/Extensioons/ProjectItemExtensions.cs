using EnvDTE;
using eXpandAddIns.Enums;
using System.Linq;

namespace eXpandAddIns.Extensioons
{
    public static class ProjectItemExtensions
    {
        public static Property FindProperty(this ProjectItem projectItem,ProjectItemProperty projectItemProperty)
        {
            return projectItem.Properties.Cast<Property>().Where(property => property.Name == projectItemProperty.ToString()).FirstOrDefault();
        }
    }
}
