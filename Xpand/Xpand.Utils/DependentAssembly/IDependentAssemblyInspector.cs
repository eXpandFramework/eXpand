using System.Collections.Generic;

namespace Xpand.Utils.DependentAssembly {
    public interface IDependentAssemblyInspector
    {
        List<string> GetAssemblies(string assembly);
    }
}