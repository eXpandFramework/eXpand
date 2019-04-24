using System;
using DevExpress.Persistent.Validation;
using FluentAssertions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Test.WorldCreator.TestArtifacts;
using Xpand.Test.WorldCreator.TestArtifacts.Autofixture;
using Xunit;

namespace Xpand.Test.WorldCreator.Validation{
    [Trait(Traits.Validation, "Given a persistent assembly info")]
    public class PersistentAssemblyInfoSpecs{
        [Theory, WorldCreatorAutoData]
        public void Name_Is_Required(IWCTestData testData){
            var persistentAssemblyInfo = testData.Application.CreateObjectSpace().Create<IPersistentAssemblyInfo>();
            var usedProperties = nameof(persistentAssemblyInfo.Name);

            Validator.RuleSet.StateOf<RuleRequiredField>(persistentAssemblyInfo, usedProperties)
                .Should()
                .Be(ValidationState.Invalid);

            persistentAssemblyInfo.Name = "name";

            Validator.RuleSet.StateOf<RuleRequiredField>(persistentAssemblyInfo, usedProperties)
                .Should()
                .Be(ValidationState.Valid);
        }


        [Theory, WorldCreatorAutoData]
        public void Name_Is_Unique(IWCTestData testData){
            var objectSpace = testData.Application.CreateObjectSpace();
            var persistentAssemblyInfo = objectSpace.Create<IPersistentAssemblyInfo>();
            persistentAssemblyInfo.Name = "name";
            var assemblyInfo = objectSpace.Create<IPersistentAssemblyInfo>();
            objectSpace.CommitChanges();
            var usedProperties = nameof(persistentAssemblyInfo.Name);


            Validator.RuleSet.StateOf<RuleUniqueValue>(persistentAssemblyInfo, usedProperties)
                .Should()
                .Be(ValidationState.Valid);

            assemblyInfo.Name = "name";

            Validator.RuleSet.StateOf<RuleUniqueValue>(persistentAssemblyInfo, usedProperties)
                .Should()
                .Be(ValidationState.Invalid);
        }

        [Theory, WorldCreatorAutoData]
        public void Name_Is_Valid_File_name(WCTestData testData, IPersistentAssemblyInfo persistentAssemblyInfo) {
            persistentAssemblyInfo.Name = "file@";
            throw new NotImplementedException();
//            Validator.RuleSet.StateOf<RuleValidFileName>(persistentAssemblyInfo, nameof(persistentAssemblyInfo.Name))
//                .Should().Be(ValidationState.Invalid);
        }

    }
}