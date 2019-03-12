using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using VSLangProj;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Wizard {
    public class ModulesInstaller:IDTE2Provider {
        private readonly XpandModule[] _allModules;
        private bool _agnosticModulesInstalled;

        ModulesInstaller(IEnumerable<XpandModule> modules){
            _allModules = modules.ToArray();
        }

        public static void Install(IEnumerable<XpandModule> modules, bool existingSolution){
            var moduleInstaller = new ModulesInstaller(modules);
            moduleInstaller.Install(existingSolution);
        }

        void Install(bool existingSolution){
            foreach (var project in DteExtensions.DTE.Solution.Projects()) {
                var platform = project.GetPlatform();
                var platformModules = _allModules.Where(module => module.Platform==platform).ToArray();
                if (platformModules.Any()){
                    if (!project.IsApplicationProject()){
                        RegisterModules(platformModules, project, platform);
                    }
                    else{
                        if (_allModules.Any(module => module.IsWorldCreator)) {
                            AddWorldCreatorConnectionString(project);
                        }
                    }
                    var assembliesPath = Path.GetDirectoryName(platformModules.First().AssemblyPath)+"";
                    var references = ((VSProject)project.Object).References;
                    references.Add(Path.Combine(assembliesPath, "Xpand.Persistent.Base.dll"));
                    references.Add(Path.Combine(assembliesPath, "Xpand.Persistent.BaseImpl.dll"));
                }
            }
            if (_allModules.Any(module => module.IsSecurity))
                SecurityInstaller.Install(_allModules,existingSolution);
        }


        private void AddWorldCreatorConnectionString(Project project){
            var projectDirectory = Path.GetDirectoryName(project.FileName) + "";
            var path = Path.Combine(projectDirectory, "web.config");
            if (!File.Exists(path))
                path = Path.Combine(projectDirectory, "app.config");
            string connectionString=$@"<add name=""WorldCreatorConnectionString"" connectionString=""Integrated Security=SSPI;Pooling=false;Data Source=(localdb)\mssqllocaldb;Initial Catalog={Path.GetFileNameWithoutExtension(DteExtensions.DTE.Solution.FullName)}WorldCreator""/>";
            var text = File.ReadAllText(path);
            text = Regex.Replace(text, "(.*<connectionStrings>.*)", $@"$1{Environment.NewLine}    {connectionString}");
            File.WriteAllText(path,text);
        }

        private void RegisterModules(XpandModule[] modules, Project project,Platform platform){
            string platformString = null;
            if (platform!=Platform.Agnostic){
                platformString =platform.ToString();
                if (!_agnosticModulesInstalled){
                    _agnosticModulesInstalled = true;
                    var agnosticProject = project.DTE.Solution.Projects().First(project1 => project1.GetPlatform()==Platform.Agnostic);
                    var xpandModules = modules.Select(module => module.AgnosticVersion).Where(module => module != null).ToArray();
                    RegisterModules(xpandModules,agnosticProject,Platform.Agnostic);
                }
            }
            var references = ((VSProject)project.Object).References;
            foreach (var xpandModule in modules) {
                references.Add(xpandModule.AssemblyPath);
            }
            var path = Path.Combine(Path.GetDirectoryName(project.FileName)+"",platformString+$"Module.Designer.{project.FileExtension()}");
            var input = File.ReadAllText(path);
            var replacementText = GetReplacementText(modules,project);
            var pattern = ModuleRegistrationPattern(platform,project);
            var newText = Regex.Replace(input,pattern,replacementText);
            File.WriteAllText(path,newText);
        }


        private string ModuleRegistrationPattern(Platform platform, Project project){
            string typeName = @"DevExpress\.ExpressApp\.SystemModule\.SystemModule";
            if (platform == Platform.Win)
                typeName = @"DevExpress\.ExpressApp\.Win\.SystemModule\.SystemWindowsFormsModule";
            else if (platform == Platform.Web)
                typeName = @"DevExpress\.ExpressApp\.Web\.SystemModule\.SystemAspNetModule";
            return $@"({project.Self()}\.RequiredModuleTypes\.Add\({project.TypeofFunction()}\({typeName}\)\){project.CommandSeperator()})";
        }


        private string GetReplacementText(XpandModule[] modules, Project project){
            var lines = modules.Select(module => $"			{project.Self()}.RequiredModuleTypes.Add({project.TypeofFunction()}({module.TypeDefinition.FullName})){project.CommandSeperator()}");
            return $"$1{Environment.NewLine}{string.Join(Environment.NewLine, lines)}";
        }

    }
}
