using System.Collections.Generic;

namespace eXpand.Utils.DependentAssembly {
    public interface IDependentAssemblyInspector
    {
        List<string> GetAssemblies(string assembly);
    }
}