using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TemplateWizard;
using Xpand.VSIX.Extensions;
using Language = Xpand.VSIX.Extensions.Language;

namespace Xpand.VSIX.Wizard {
    public class SolutionWizard : IWizard,IDTE2Provider {
        private readonly bool _existingSolution;
        private SolutionEvents _solutionEvents;

        public SolutionWizard():this(false){
        }

        private SolutionWizard(bool existingSolution){
            _existingSolution = existingSolution;
        }

        public virtual void BeforeOpeningFile(ProjectItem projectItem) {
        }
        public virtual void ProjectFinishedGenerating(Project project) {
        }
        
        public void ProjectItemFinishedGenerating(ProjectItem projectItem) {
        }
        
        public virtual void RunFinished() {
        }

        public virtual void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams) {
            var dte2 = this.DTE2();
            _solutionEvents = dte2.Events.SolutionEvents;
            _solutionEvents.QueryCloseSolution+=SolutionEventsOnQueryCloseSolution;
            var language = customParams.Any(o => Path.GetFileNameWithoutExtension(o.ToString()) == "SolutionVB") ? Language.VisualBasic : Language.CSharp;
            RunXAFWizard(replacementsDictionary, language, dte2);
            var modules = GetXpandModules();
            RunXpandWizard(modules, language);
        }

        private XpandModule[] GetXpandModules(){
            return this.DTE2().Solution.Projects()
                .Where(project => project.GetPlatform() != Platform.Agnostic)
                .SelectMany(project => ModuleManager.GetModules(project.GetPlatform())).Distinct().ToArray();
        }

        private void RunXpandWizard(XpandModule[] modules, Language language,bool favorAgnostic=false){
            try{
                if (!this.DTE2().Solution.HasAuthentication())
                    modules = modules.Except(modules.Where(module => module.IsSecurity)).ToArray();
                var wizardForm = new WizardForm(){Modules = modules, Language = language,ExistingSolution=_existingSolution,FavorAgnostic=favorAgnostic};
                wizardForm.ShowDialog();
            }
            catch (Exception e){
                this.DTE2().LogError(e.ToString());
                this.DTE2().WriteToOutput(e.ToString());
                throw;
            }
        }

        private  string GetAssemblyLocalPath(){
            string codebase = GetType().Assembly.CodeBase;
            var uri = new Uri(codebase, UriKind.Absolute);
            return Path.GetDirectoryName(uri.LocalPath);
        }

        private void RunXAFWizard(Dictionary<string, string> replacementsDictionary, Language language, DTE2 dte){
            try{
                string templateName = language == Language.VisualBasic ? "vb" : "cs";
                string templatePath = $@"{GetAssemblyLocalPath()}\ProjectTemplates\xaf.{templateName}.vstemplate";
                var projectName = replacementsDictionary["$projectname$"];
                var destination = replacementsDictionary["$destinationdirectory$"];
                var solutionPath = Path.GetFullPath(destination + @"\..\");
                dte.Solution.Create(solutionPath, projectName);
                dte.Solution.AddFromTemplate(templatePath, destination, projectName);
                dte.ExecuteCommand("File.SaveAll");

            }
            catch (Exception e){
                this.DTE2().LogError(e.ToString());
                this.DTE2().WriteToOutput(e.ToString());
                throw;
            }
        }

        private void SolutionEventsOnQueryCloseSolution(ref bool fCancel){
            _solutionEvents.QueryCloseSolution -= SolutionEventsOnQueryCloseSolution;
            if (!fCancel)
                fCancel = true;
        }

        public virtual bool ShouldAddProjectItem(string filePath) {
            return true;
        }

        public static void Show(){
            try{
                var solutionWizard = new SolutionWizard(true);
                var dte2 = solutionWizard.DTE2();
                var activeProject = ((object[])dte2.ActiveSolutionProjects).Cast<Project>().First();
                if (activeProject.IsApplicationProject())
                    throw new NotSupportedException("Application projects are not supported. You can only install in a module");
                var modules = solutionWizard.GetXpandModules();
                if (!modules.Any() && activeProject.GetPlatform() == Platform.Agnostic)
                    modules = ModuleManager.Instance.Modules.Where(module => module.Platform == Platform.Agnostic).ToArray();
                solutionWizard.RunXpandWizard(modules, activeProject.Language(),activeProject.GetPlatform()==Platform.Agnostic);
            }
            catch (Exception exception){
                DteExtensions.DTE.LogError(exception.ToString());
                DteExtensions.DTE.WriteToOutput(exception.ToString());
                throw;
            }
        }
    }

}
