using System;
using System.Linq;
using FluentAssertions;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Test.WorldCreator.TestArtifacts;
using Xpand.Test.WorldCreator.TestArtifacts.Autofixture;
using Xunit;

namespace Xpand.Test.WorldCreator.Compilation{
    [Trait(Traits.Compilation,"From a persistent assembly generate")]
    public class PersistentAssemblySpecs:BaseSpecs {
        [WorldCreatorAutoData,Theory]
        public void Assmebly_File(IWCTestData testData,Compiler compiler,IPersistentAssemblyInfo assemblyInfo){
            var code = assemblyInfo.GenerateCode();

            var compilerResult = compiler.Compile(code, assemblyInfo.Name);

            compilerResult.AssemblyDefinition.Should().NotBeNull();
        }

        
        [WorldCreatorAutoData, Theory]
        public void Strongly_typed_assembly_file(){
            throw new NotImplementedException();
        }

        [WorldCreatorAutoData, Theory]
        public void Aclass(IWCTestData testData, Compiler compiler, IPersistentClassInfo persistentClassInfo){
            var code = persistentClassInfo.GenerateCode();

            var compilerResult = compiler.Compile(code,persistentClassInfo.Name);


            var moduleDefinition = compilerResult.AssemblyDefinition.MainModule;
            moduleDefinition.Types.Any(definition => definition.Name == persistentClassInfo.Name)
                .Should()
                .BeTrue();
        }
    }
}