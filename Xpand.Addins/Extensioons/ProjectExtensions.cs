using System.Linq;
using DevExpress.CodeRush.Core;
using EnvDTE;
using XpandAddIns.Enums;
using Project=EnvDTE.Project;

namespace XpandAddIns.Extensioons
{
    public static class ProjectExtensions
    {
        public static Property FindProperty(this Project project,ProjectProperty projectProperty)
        {
            return project.Properties.Cast<Property>().Where(property => property.Name == projectProperty.ToString()).Single();
        }

    }
}