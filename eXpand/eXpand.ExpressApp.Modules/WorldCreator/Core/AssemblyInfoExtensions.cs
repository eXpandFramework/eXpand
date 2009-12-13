using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Core
{
    public static class AssemblyInfoExtensions
    {
        public static void Validate(this IPersistentAssemblyInfo assemblyInfo)
        {
            new CompileEngine().CompileModule(assemblyInfo, parameters => {
                parameters.GenerateInMemory = false;
                parameters.OutputAssembly += "validating_" + parameters.OutputAssembly;
            });
        }
    }
}
