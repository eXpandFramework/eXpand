using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using FluentAssertions;
using Xpand.ExpressApp.WorldCreator.BusinessObjects.Validation;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Test.WorldCreator.TestArtifacts;
using Xpand.Test.WorldCreator.TestArtifacts.Autofixture;
using Xunit;

namespace Xpand.Test.WorldCreator.Validation{
    public class PersistentMemberInfoSpecs{
        [Theory, WorldCreatorAutoData]
        public void Name_Is_Required(IWCTestData testData){
            NameIsRequired<IPersistentCoreTypeMemberInfo>(testData);
            NameIsRequired<IPersistentReferenceMemberInfo>(testData);
            NameIsRequired<IPersistentCollectionMemberInfo>(testData);
        }

        [Theory, WorldCreatorAutoData]
        public void Name_Is_Valid_Code_Identifier(WCTestData testData, IPersistentCoreTypeMemberInfo persistentClassInfo) {
            persistentClassInfo.Name = "123";

            Validator.RuleSet.StateOf<RuleValidCodeIdentifier>(persistentClassInfo, nameof(persistentClassInfo.Name))
                .Should().Be(ValidationState.Invalid);
        }

        private static void NameIsRequired<T>(IWCTestData testData) where T : IPersistentMemberInfo{
            var coreTypeMemberInfo = testData.Application.CreateObjectSpace().Create<T>();
            var usedProperties = nameof(coreTypeMemberInfo.Name);


            Validator.RuleSet.StateOf<RuleRequiredField>(coreTypeMemberInfo, usedProperties)
                .Should()
                .Be(ValidationState.Invalid);
            coreTypeMemberInfo.Name = "name";

            Validator.RuleSet.StateOf<RuleRequiredField>(coreTypeMemberInfo, usedProperties)
                .Should()
                .Be(ValidationState.Valid);
        }


        [Theory, WorldCreatorAutoData]
        public void Name_Is_Unique_For_a_Class(IWCTestData testData){
            var objectSpace = testData.Application.CreateObjectSpace();

            NameIsUniqueForAClass<IPersistentCoreTypeMemberInfo>(objectSpace);
            NameIsUniqueForAClass<IPersistentCollectionMemberInfo>(objectSpace);
            NameIsUniqueForAClass<IPersistentReferenceMemberInfo>(objectSpace);
        }

        private void NameIsUniqueForAClass<T>(IObjectSpace objectSpace) where T : IPersistentMemberInfo{
            var memberInfo = objectSpace.Create<T>();
            memberInfo.Name = "name";
            var persistentClassInfo = objectSpace.Create<IPersistentClassInfo>();
            memberInfo.Owner = persistentClassInfo;
            memberInfo = objectSpace.Create<T>();
            memberInfo.Name = "name";
            memberInfo.Owner = objectSpace.Create<IPersistentClassInfo>();

            objectSpace.CommitChanges();
            var usedProperties = nameof(memberInfo.Name);


            Validator.RuleSet.StateOf<RuleCombinationOfPropertiesIsUnique>(memberInfo, usedProperties)
                .Should()
                .Be(ValidationState.Valid);

            memberInfo.Owner = persistentClassInfo;

            Validator.RuleSet.StateOf<RuleCombinationOfPropertiesIsUnique>(memberInfo, usedProperties)
                .Should()
                .Be(ValidationState.Invalid);
        }
    }
}