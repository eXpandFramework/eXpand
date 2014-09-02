using System.Collections.Generic;
using System.Linq;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.StructuralParser;
using EnvDTE;

namespace XpandAddIns.Extensions {
    public static class DteExtensions {
        private static OutputWindowPane _outputWindowPane;

        public static void WriteToOutput(this DTE dte, string text) {
            ClearOutputWindow();
            if (_outputWindowPane == null || string.IsNullOrEmpty(text))
                return;
            //_DxCoreTestPane.Activate();
            _outputWindowPane.OutputString(text);
        }

        static void ClearOutputWindow() {
            InitializeVsPaneIfNeeded();
            if (_outputWindowPane == null)
                return;
            _outputWindowPane.Clear();
        }

        static void InitializeVsPaneIfNeeded() {
            if (_outputWindowPane != null){
                _outputWindowPane.Activate();
                return;
            }
            var wnd = CodeRush.ApplicationObject.Windows.Item(Constants.vsWindowKindOutput);
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
            DTE dte = CodeRush.ApplicationObject;
            var items = ((UIHierarchy)dte.Windows.Item(constants).Object).SelectedItems;
            var selectedItems = ((System.Collections.IEnumerable)items).OfType<UIHierarchyItem>().Select(item => item.Name);
            assemblyReferences = assemblyReferences.Where(reference => selectedItems.Contains(reference.Name));
            return assemblyReferences;
        }

    }
}
