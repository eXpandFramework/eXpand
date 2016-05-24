using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.CodeRush.StructuralParser;
using EnvDTE;
using VSLangProj;

namespace Xpand.CodeRush.Plugins.Extensions {
    public static class DteExtensions {
        private static OutputWindowPane _outputWindowPane;

        public static IEnumerable<IFullReference> GetReferences(this DTE dte) {
            return dte.Solution.Projects.OfType<Project>().SelectMany(project => ((VSProject)project.Object).References.OfType<IFullReference>()).Where(reference =>
                reference.SpecificVersion && (reference.Identity.StartsWith("Xpand") || reference.Identity.StartsWith("DevExpress"))).ToArray();
        }

        public static void InitOutputCalls(this DTE dte, string text){
            dte.WriteToOutput(Environment.NewLine+"------------------"+text+ "------------------" + Environment.NewLine);
        }

        public static void WriteToOutput(this DTE dte, string text) {
            InitializeVsPaneIfNeeded();
            if (_outputWindowPane == null || string.IsNullOrEmpty(text))
                return;
            ActivatePane();
            _outputWindowPane.OutputString(text+Environment.NewLine);
        }

        private static void ActivatePane(){
            DevExpress.CodeRush.Core.CodeRush.ApplicationObject.Windows.Item(Constants.vsWindowKindOutput).Activate();
            _outputWindowPane.Activate();
        }

        public static void ClearOutputWindow() {
            InitializeVsPaneIfNeeded();
            if (_outputWindowPane == null)
                return;
            _outputWindowPane.Clear();
        }

        static void InitializeVsPaneIfNeeded() {
            if (_outputWindowPane != null){
                ActivatePane();
                return;
            }
            var wnd = DevExpress.CodeRush.Core.CodeRush.ApplicationObject.Windows.Item(Constants.vsWindowKindOutput);
            if (wnd == null)
                return;
            var outputWindow = wnd.Object as OutputWindow;
            if (outputWindow == null)
                return;
            try {
                _outputWindowPane = outputWindow.OutputWindowPanes.Item("XpandAddIns");
            }
            catch {
                _outputWindowPane = outputWindow.OutputWindowPanes.Add("XpandAddIns");
            }
        }
        public static IEnumerable<AssemblyReference> GetSelectedAssemblyReferences(this ProjectElement projectElement, string constants) {
            var assemblyReferences = projectElement.AssemblyReferences.OfType<AssemblyReference>();
            DTE dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
            var items = ((UIHierarchy)dte.Windows.Item(constants).Object).SelectedItems;
            var selectedItems = ((IEnumerable)items).OfType<UIHierarchyItem>().Select(item => item.Name);
            assemblyReferences = assemblyReferences.Where(reference => selectedItems.Contains(reference.Name));
            return assemblyReferences;
        }

    }
}
