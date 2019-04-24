using Mono.Cecil;
using Ploeh.AutoFixture;

namespace Xpand.Test.AutoFixture.Customizations {
    public class AssemblyDefintionCustomization :ICustomization{
        public void Customize(IFixture fixture){
            var assemblyNameDefinition = fixture.Create<AssemblyNameDefinition>();
            var assemblyDefinition = AssemblyDefinition.CreateAssembly(assemblyNameDefinition, fixture.Create<string>(),
                fixture.Create<ModuleKind>());
            fixture.Register(() => assemblyDefinition);
        }
    }
}
