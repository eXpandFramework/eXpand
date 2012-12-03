using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Reflection;
using Microsoft.Build.Evaluation;

namespace Xpand.MSBuild.Tasks
{
    public class ConvertToSimpleNames : Task
    {

        [Required]
        public ITaskItem[] Projects { get; set; }

        public override bool Execute()
        {
            foreach (var item in Projects)
            {
                Log.LogMessage(item.ItemSpec);
                ProcessProject(item.ItemSpec);
            }

            return true;
        }


        private void ProcessProject(string projectFileName)
        {

            Project project = new Project(projectFileName);

            foreach (var item in project.GetItems("Reference"))
            {
                AssemblyName assemblyName = new AssemblyName(item.UnevaluatedInclude);
                if (assemblyName.Name.StartsWith("DevExpress.", StringComparison.OrdinalIgnoreCase) || assemblyName.Name.StartsWith("Xpand.", StringComparison.OrdinalIgnoreCase))
                {
                    item.UnevaluatedInclude = assemblyName.Name;

                    foreach (string mdn in item.Metadata.Select(md => md.Name).ToArray())
                        item.RemoveMetadata(mdn);

                }

            }


            project.Save(projectFileName);
        }
    }
}
