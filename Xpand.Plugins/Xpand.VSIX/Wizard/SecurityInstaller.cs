using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Utilities;
using VSLangProj;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Wizard{
    internal class SecurityInstaller: IDTE2Provider {
        private readonly XpandModule[] _allModules;

        SecurityInstaller(XpandModule[] allModules){
            _allModules = allModules;
        }

        private void ReplaceUpdater() {
            var solution = this.Solution();
            if (solution.HasAuthentication()){
                var project = solution.Projects().First(p => p.GetPlatform() == Platform.Agnostic);
                var path = Path.Combine(Path.GetDirectoryName(project.FullName) + "", $@"DatabaseUpdate\Updater.{project.FileExtension()}");
                File.Delete(path);
                var resourceName = GetType().Namespace + $".Resources.Updater.{project.FileExtension()}";
                var stream = GetType().Assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                    throw new StreamNotFoundException(resourceName);
                using (var streamReader = new StreamReader(stream)){
                    var contents = streamReader.ReadToEnd().Replace("$projectsuffix$",Path.GetFileNameWithoutExtension(solution.FullName));
                    contents = AddModelDifferencePermissions(contents);
                    File.WriteAllText(path, contents);
                }
            }
            ReferenceAssemblies();
        }

        private string AddModelDifferencePermissions(string contents){
            return _allModules.Any(module => module.IsModelDifference)
                ? contents.Replace("ObjectSpace.CommitChanges()",
                    $@"user.Roles.Add(DirectCast(ObjectSpace.GetDefaultModelRole(""ModelRole""), SecuritySystemRole)){Environment
                        .NewLine}ObjectSpace.CommitChanges()").Replace("Imports System", $"Imports System{Environment.NewLine}Imports Xpand.ExpressApp.ModelDifference.Security")
                : contents;
        }

        public static void Install(XpandModule[] allModules, bool existingSolution){
            var securityInstaller = new SecurityInstaller(allModules);
            if (!existingSolution){
                securityInstaller.ReplaceUpdater();
                securityInstaller.ModifyApplicationProjects(allModules);
            }
        }

        private void ModifyApplicationProjects(XpandModule[] allModules){
            var applicationProjects = this.Solution().Projects().Where(project => project.IsApplicationProject());
            var assemblyPath = allModules.First(module => module.IsSecurity).AgnosticVersion.AssemblyPath;
            foreach (var project in applicationProjects){
                ((VSProject) project.Object).References.Add(assemblyPath);
                var path = Path.Combine(Path.GetDirectoryName(project.FileName)+"",project.IsWeb()?$"WebApplication.{project.FileExtension()}": $"WinApplication.Designer.{project.FileExtension()}");
                var text = File.ReadAllText(path);
                text=Regex.Replace(text, @"DevExpress\.ExpressApp\.Security\.AuthenticationStandard\b", "Xpand.ExpressApp.Security.AuthenticationProviders.XpandAuthenticationStandard");
                text=Regex.Replace(text, @"DevExpress\.Persistent\.BaseImpl\.PermissionPolicy\.PermissionPolicyUser\b", "Xpand.Persistent.BaseImpl.Security.XpandPermissionPolicyUser");
                text=Regex.Replace(text, @"DevExpress\.Persistent\.BaseImpl\.PermissionPolicy\.PermissionPolicyRole\b", "Xpand.Persistent.BaseImpl.Security.XpandPermissionPolicyRole");
                text=Regex.Replace(text, @"DevExpress\.ExpressApp\.Security\.AuthenticationStandardLogonParameters\b", "Xpand.ExpressApp.Security.AuthenticationProviders.XpandLogonParameters");
                File.WriteAllText(path,text);
            }
        }

        private void ReferenceAssemblies(){
            var agnosticProject = this.Solution().Projects().First(project => project.GetPlatform()==Platform.Agnostic);
            var vsProject = ((VSProject) agnosticProject.Object);
            if (!vsProject.References.Cast<Reference>().Any(reference => reference.Name.Contains("Xpand.ExpressApp.Security"))){
                var securityModule = ModuleManager.GetModules(Platform.Agnostic).First(module => module.IsSecurity);
                vsProject.References.Add(securityModule.AssemblyPath);
            }
            var modelDiffModule = _allModules.FirstOrDefault(module => module.IsModelDifference);
            if (modelDiffModule!=null&&vsProject.References.Cast<Reference>().Any(reference => reference.Name==Path.GetFileNameWithoutExtension(modelDiffModule.AssemblyPath))){
                vsProject.References.Add(Path.GetFileNameWithoutExtension(modelDiffModule.AssemblyPath));
            }
        }
    }
}