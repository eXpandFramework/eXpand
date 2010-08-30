using System.Collections.Generic;

namespace eXpandAddIns.ModelEditor {
    public interface IDependedAttributeInspector
    {
        List<string> GetAssemblies(string assembly);
    }
}