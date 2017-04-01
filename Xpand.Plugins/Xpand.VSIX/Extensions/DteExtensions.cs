using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;
using Constants = EnvDTE.Constants;

namespace Xpand.VSIX.Extensions {
    public static class DteExtensions {
        private static OutputWindowPane _outputWindowPane;
        public static DTE2 DTE = (DTE2) Package.GetGlobalService(typeof(DTE));
        public static IEnumerable<IFullReference> GetReferences(this DTE2 dte) {
            return dte.Solution.Projects.OfType<Project>().SelectMany(project => ((VSProject)project.Object).References.OfType<IFullReference>()).Where(reference =>
                reference.SpecificVersion && (reference.Identity.StartsWith("Xpand") || reference.Identity.StartsWith("DevExpress"))).ToArray();
        }

        public static void InitOutputCalls(this DTE2 dte, string text) {
            dte.WriteToOutput(Environment.NewLine + "------------------" + text + "------------------" + Environment.NewLine);
        }

        public static void LogError(this DTE2 dte, string text){
            var log = ((IServiceProvider) VSXpandPackage.Instance).GetService(typeof(SVsActivityLog)) as IVsActivityLog;
            log?.LogEntry((uint) __ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, VSXpandPackage.Instance.ToString(), text);
        }

        public static void WriteToOutput(this DTE2 dte, string text) {
            InitializeVsPaneIfNeeded(dte);
            if (_outputWindowPane == null || string.IsNullOrEmpty(text))
                return;
            ActivatePane();
            _outputWindowPane.OutputString(text + Environment.NewLine);
        }

        private static void ActivatePane() {
            DTE.Windows.Item(Constants.vsWindowKindOutput).Activate();
            _outputWindowPane.Activate();
        }

        public static void ClearOutputWindow(this DTE2 dte) {
            InitializeVsPaneIfNeeded(dte);
            _outputWindowPane?.Clear();
        }

        static void InitializeVsPaneIfNeeded(DTE2 dte) {
            if (_outputWindowPane != null) {
                ActivatePane();
                return;
            }
            var wnd = dte.Windows.Item(Constants.vsWindowKindOutput);
            var outputWindow = wnd?.Object as OutputWindow;
            if (outputWindow == null)
                return;
            try {
                _outputWindowPane = outputWindow.OutputWindowPanes.Item("XpandAddIns");
            }
            catch {
                _outputWindowPane = outputWindow.OutputWindowPanes.Add("XpandAddIns");
            }
        }
//        public static IEnumerable<AssemblyReference> GetSelectedAssemblyReferences(this ProjectElement projectElement, string constants) {
//            var assemblyReferences = projectElement.AssemblyReferences.OfType<AssemblyReference>();
//            DTE dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
//            var items = ((UIHierarchy)dte.Windows.Item(constants).Object).SelectedItems;
//            var selectedItems = ((IEnumerable)items).OfType<UIHierarchyItem>().Select(item => item.Name);
//            assemblyReferences = assemblyReferences.Where(reference => selectedItems.Contains(reference.Name));
//            return assemblyReferences;
//        }

    }
}
