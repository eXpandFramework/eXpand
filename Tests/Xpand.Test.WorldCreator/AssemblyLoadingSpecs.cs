using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Test.WorldCreator.TestArtifacts;
using Xpand.Test.WorldCreator.TestArtifacts.Autofixture;
using Xunit;

namespace Xpand.Test.WorldCreator{
    public class AssemblyLoadingSpecs : BaseSpecs{
        [Theory, WorldCreatorAutoData()]
        public void Only_Persistent_Assemblies_That_Will_be_Compiled_Should_Be_Validated(IWCTestData testData,
            IList<IPersistentAssemblyInfo> persistentAssemblyInfos,
            IAssemblyManager assemblyManager){
            persistentAssemblyInfos.First().DoNotCompile = true;

            var validatorResults = assemblyManager.ValidateAssemblyInfos();

            validatorResults.Length.Should().Be(2);
        }

        [Theory, WorldCreatorAutoData]
        public void Assemblies_Should_be_loaded_in_memory_if_validation_succeed(IWCTestData testData,
            IPersistentClassInfo persistentClassInfo, IAssemblyManager assemblyManager){
            persistentClassInfo.PersistentAssemblyInfo.Name += Guid.NewGuid().ToString("N");

            var assemblies = assemblyManager.LoadAssemblies().ToArray();

            assemblies.Length.Should().Be(1);

            var assembly = assemblies.First();
            Path.GetFileNameWithoutExtension(assembly.Location)
                .Should()
                .Be(persistentClassInfo.PersistentAssemblyInfo.Name);
            AppDomain.CurrentDomain.GetAssemblies().Should().Contain(assembly);
        }

        [Theory, WorldCreatorAutoData()]
        public void Assembly_Should_not_Validate_if_version_equal_to_file_version(IWCTestData testData,
            IPersistentClassInfo persistentClassInfo, IAssemblyManager assemblyManager){
            persistentClassInfo.PersistentAssemblyInfo.Revision++;
            persistentClassInfo.PersistentAssemblyInfo.Name += Guid.NewGuid().ToString("n");
            assemblyManager.LoadAssemblies();

            var validAssemblyInfos = assemblyManager.ValidateAssemblyInfos();

            validAssemblyInfos.Length.Should().Be(0);
        }

        [Theory, WorldCreatorAutoData()]
        public void Assemblies_that_do_not_need_validation_should_load_in_memory(IWCTestData testData,
            IPersistentClassInfo persistentClassInfo, IAssemblyManager assemblyManager,Compiler compiler){
            var persistentAssemblyInfo = persistentClassInfo.PersistentAssemblyInfo;
            compiler.Compile(persistentAssemblyInfo.GenerateCode(), persistentAssemblyInfo.Name);

            var assemblies = assemblyManager.LoadAssemblies().ToArray();

            assemblies.Length.Should().Be(1);

            var assembly = assemblies.First();
            Path.GetFileNameWithoutExtension(assembly.Location)
                .Should()
                .Be(persistentClassInfo.PersistentAssemblyInfo.Name);
            AppDomain.CurrentDomain.GetAssemblies().Should().Contain(assembly);
        }

        [Theory, WorldCreatorInlineAutoData(0),WorldCreatorInlineAutoData(2), ]
        public void Assembly_Should_Validate_if_version_different_to_file_version(int revision,IWCTestData testData,
            IPersistentAssemblyInfo assemblyInfo, IAssemblyManager assemblyManager){
            assemblyInfo.Name += Guid.NewGuid().ToString("n");
            assemblyInfo.Revision = 1;
            assemblyManager.CodeValidator.Compiler.Compile(assemblyInfo.GenerateCode(),assemblyInfo.Name, new byte[0]);

            assemblyInfo.Revision = revision;
            var validAssemblyInfos = assemblyManager.ValidateAssemblyInfos();

            validAssemblyInfos.Length.Should().Be(1);
        }

        [Theory, WorldCreatorAutoData()]
        public void Assembly_Should_Validate_if_file_not_exists(IWCTestData testData,
            IPersistentClassInfo persistentClassInfo, IAssemblyManager assemblyManager){
            persistentClassInfo.PersistentAssemblyInfo.Name += Guid.NewGuid().ToString("N");

            var persistentAssemblyInfos = assemblyManager.ValidateAssemblyInfos();

            persistentAssemblyInfos.Length.Should().Be(1);
        }

        [Theory, WorldCreatorAutoData()]
        public void Assembly_Should_not_be_Validated_if_already_loaded(IWCTestData testData,
            IPersistentClassInfo persistentClassInfo, IAssemblyManager assemblyManager){
            persistentClassInfo.PersistentAssemblyInfo.Name += Guid.NewGuid().ToString("n");
            (assemblyManager.LoadAssemblies().ToArray().FirstOrDefault() != null).Should().BeTrue();

            var loadAssemblies = assemblyManager.LoadAssemblies();

            loadAssemblies.Count.Should().Be(0);
        }

        [Theory, WorldCreatorAutoData()]
        public void Validation_Failures_Should_be_saved_in_the_Persistent_Assembly_info(IWCTestData testData,
            IPersistentClassInfo persistentClassInfo, IAssemblyManager assemblyManager){
            persistentClassInfo.CodeTemplateInfo.TemplateInfo.TemplateCode = "invalid";

            assemblyManager.ValidateAssemblyInfos();

            persistentClassInfo.PersistentAssemblyInfo.Errors.Should()
                .Contain("A namespace cannot directly contain members such as fields or methods");
        }
    }
}