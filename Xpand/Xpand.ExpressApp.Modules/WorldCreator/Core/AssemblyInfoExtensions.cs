using System;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public static class AssemblyInfoExtensions {
        public static Type Validate(this IPersistentAssemblyInfo assemblyInfo, string path) {
            return new CompileEngine().CompileModule(assemblyInfo, parameters => {
                parameters.GenerateInMemory = false;
                parameters.OutputAssembly = parameters.OutputAssembly + "Validating";
            }, path);
        }
    }
}
