using System.Collections.Generic;
using System.Linq;
using DevExpress.CodeRush.Core;
using DevExpress.CodeRush.StructuralParser;
using EnvDTE;

namespace XpandAddIns.Extensioons {
    public static class DteExtensions {
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
