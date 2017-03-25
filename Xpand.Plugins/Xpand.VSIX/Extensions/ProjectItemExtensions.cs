using System.Linq;
using EnvDTE;

namespace Xpand.VSIX.Extensions{
    public enum ProjectItemProperty {
        Extension,
        FileName,
        CustomToolOutput,
        DateModified,
        IsLink,
        BuildAction,
        SubType,
        CopyToOutputDirectory,
        IsSharedDesignTimeBuildInput,
        ItemType,
        IsCustomToolOutput,
        HTMLTitle,
        CustomTool,
        URL,
        Filesize,
        CustomToolNamespace,
        Author,
        FullPath,
        IsDependentFile,
        IsDesignTimeBuildInput,
        DateCreated,
        LocalPath,
        ModifiedBy,

    }

    public static class ProjectItemExtensions {
        public static Property FindProperty(this ProjectItem projectItem, ProjectItemProperty projectItemProperty) {
            string projectItemPropertyStr = projectItemProperty.ToString();
            return projectItem.Properties.Cast<Property>().FirstOrDefault(property => property.Name == projectItemPropertyStr);
        }
    }
}