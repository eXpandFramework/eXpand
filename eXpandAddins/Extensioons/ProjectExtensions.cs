using System.Linq;
using EnvDTE;
using XAFAddIns.Enums;

namespace XAFAddIns.Extensioons
{
    public static class ProjectExtensions
    {
        public static Property FindProperty(this Project project,ProjectProperty projectProperty)
        {
            return project.Properties.Cast<Property>().Where(property => property.Name == projectProperty.ToString()).Single();
        }
    }
}