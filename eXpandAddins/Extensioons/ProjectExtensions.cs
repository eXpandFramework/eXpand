using System.Linq;
using DevExpress.CodeRush.Core;
using EnvDTE;
using eXpandAddIns.Enums;
using Project=EnvDTE.Project;

namespace eXpandAddIns.Extensioons
{
    public static class ProjectExtensions
    {
        public static Property FindProperty(this Project project,ProjectProperty projectProperty)
        {
            return project.Properties.Cast<Property>().Where(property => property.Name == projectProperty.ToString()).Single();
        }

    }
}