using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;
using Constants = EnvDTE.Constants;

namespace Xpand.VSIX.Extensions {
    public static class ShellExtensions {
        public static void  ShowToolWindow<T>(this Package package) where T: ToolWindowPane {
            ToolWindowPane window = package.FindToolWindow(typeof(T), 0, true);
            if (window?.Frame == null) {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
        public static IEnumerable<T> GetItems<T>(this IEnumerable collection,
        Func<T, IEnumerable> selector) {
            var stack = new Stack<IEnumerable<T>>();
            stack.Push(collection.OfType<T>());

            while (stack.Count > 0) {
                IEnumerable<T> items = stack.Pop();
                foreach (var item in items) {
                    yield return item;

                    IEnumerable<T> children = selector(item).OfType<T>();
                    stack.Push(children);
                }
            }
        }

        public static OleMenuCommand EnableForDXSolution(this OleMenuCommand oleMenuCommand){
            oleMenuCommand.BeforeQueryStatus+=(sender, args) => oleMenuCommand.Enabled = DteExtensions.DTE.Solution.GetDXVersion() != null;
            return oleMenuCommand;
        }

        public static OleMenuCommand EnableForSolution(this OleMenuCommand oleMenuCommand){
            oleMenuCommand.BeforeQueryStatus+=(sender, args) => oleMenuCommand.Enabled=DteExtensions.DTE.Solution.Projects().Any();
            return oleMenuCommand;
        }

        public static OleMenuCommand ActiveForDXSolution(this OleMenuCommand oleMenuCommand){
            oleMenuCommand.BeforeQueryStatus+=(sender, args) => oleMenuCommand.Visible = DteExtensions.DTE.Solution.GetDXVersion() != null;
            return oleMenuCommand;
        }

        public static OleMenuCommand EnableForConfigFile(this OleMenuCommand oleMenuCommand){
            oleMenuCommand.EnableForProjectFile("app.config|web.config");
            return oleMenuCommand;
        }

        public static OleMenuCommand EnableForProjectFile(this OleMenuCommand oleMenuCommand, string regex){
            oleMenuCommand.BeforeQueryStatus += (sender, args) =>{
                var project = DteExtensions.DTE.Solution.FindStartUpProject();
                var enabled = project != null && project.ProjectItems.Cast<ProjectItem>().Any(item => Regex.IsMatch(item.Name,regex,RegexOptions.IgnoreCase));
                oleMenuCommand.Enabled=enabled;
            };
            return oleMenuCommand;
        }

        public static OleMenuCommand EnableForAssemblyReferenceSelection(this OleMenuCommand oleMenuCommand){
            oleMenuCommand.BeforeQueryStatus+= (sender, args) =>{
                var uihSolutionExplorer = DteExtensions.DTE.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
                oleMenuCommand.Enabled = false;
                if (uihSolutionExplorer?.SelectedItems != null)
                    oleMenuCommand.Enabled =((UIHierarchyItem[]) uihSolutionExplorer.SelectedItems).Any(item => item.Object is Reference||item.Object.GetType().Name=="OAReferenceItem");
            };
            return oleMenuCommand;
        }

        public static OleMenuCommand EnableForActiveFile(this OleMenuCommand oleMenuCommand, params string[] fileNames){
            oleMenuCommand.BeforeQueryStatus+= (sender, args) =>{
                var activeDocument = DteExtensions.DTE.ActiveDocument;
                oleMenuCommand.Enabled = activeDocument != null && (fileNames.Any(s => activeDocument.FullName.EndsWith(s))||fileNames.Length==0);
            };
            return oleMenuCommand;
        }


    }
}
