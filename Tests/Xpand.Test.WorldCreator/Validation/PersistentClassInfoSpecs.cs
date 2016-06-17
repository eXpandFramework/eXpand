using Xpand.Persistent.BaseImpl.PersistentMetaData;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using Xunit;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using FluentAssertions;
using Xpand.ExpressApp.WorldCreator.BusinessObjects.Validation;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Test.WorldCreator.TestArtifacts;
using Xpand.Test.WorldCreator.TestArtifacts.Autofixture;

namespace Xpand.Test.WorldCreator.Validation{
    [Trait(Traits.Validation, Traits.Validation)]
    public class PersistentClassInfoSpecs{
        [Theory, WorldCreatorAutoData]
        public void Parent_MapInheritance_required_When_Merging(IWCTestData testData){
            var objectSpace = testData.Application.CreateObjectSpace();
            var classInfo = objectSpace.Create<IPersistentClassInfo>();
            classInfo.MergedObjectFullName = GetType().FullName;
            classInfo.BaseType = null;

            var message = RuleClassInfoMerge.ErrorMessage;

            Validator.RuleSet.StateOf(classInfo, message).Should().Be(ValidationState.Invalid);

            var mapInheritanceAttribute = objectSpace.CreateObject<PersistentMapInheritanceAttribute>();
            classInfo.TypeAttributes.Add(mapInheritanceAttribute);
            Validator.RuleSet.StateOf(classInfo, message).Should().Be(ValidationState.Valid);


            mapInheritanceAttribute.MapInheritanceType = MapInheritanceType.OwnTable;
            Validator.RuleSet.StateOf(classInfo, message).Should().Be(ValidationState.Invalid);
        }


        [Theory, WorldCreatorAutoData]
        public void BaseClass_Required_When_Merging_ToExisting_Type(IWCTestData testData){
            var objectSpace = testData.Application.CreateObjectSpace();
            var persistentClassInfo = objectSpace.CreateObject<PersistentClassInfo>();
            persistentClassInfo.MergedObjectType = GetType();
            persistentClassInfo.BaseType = null;
            var basetype = nameof(persistentClassInfo.BaseType);
            var baseclassinfo = nameof(persistentClassInfo.BaseClassInfo);
            var ruleSet = Validator.RuleSet;

            ruleSet.StateOf<RuleRequiredField>(persistentClassInfo, basetype).Should().Be(ValidationState.Invalid);
            ruleSet.StateOf<RuleRequiredField>(persistentClassInfo, baseclassinfo).Should().Be(ValidationState.Invalid);

            persistentClassInfo.BaseType = GetType();
            ruleSet.StateOf<RuleRequiredField>(persistentClassInfo, basetype).Should().Be(ValidationState.Valid);
            ruleSet.StateOf<RuleRequiredField>(persistentClassInfo, baseclassinfo).Should().Be(ValidationState.Skipped);

            persistentClassInfo.BaseType = null;
            persistentClassInfo.BaseClassInfo = objectSpace.CreateObject<PersistentClassInfo>();
            ruleSet.StateOf<RuleRequiredField>(persistentClassInfo, basetype).Should().Be(ValidationState.Skipped);
            ruleSet.StateOf<RuleRequiredField>(persistentClassInfo, baseclassinfo).Should().Be(ValidationState.Valid);
        }

        [Theory, WorldCreatorAutoData]
        public void Name_Is_Required(IWCTestData testData){
            var persistentClassInfo = testData.Application.CreateObjectSpace().Create<IPersistentClassInfo>();
            var usedProperties = nameof(persistentClassInfo.Name);


            Validator.RuleSet.StateOf<RuleRequiredField>(persistentClassInfo, usedProperties)
                .Should()
                .Be(ValidationState.Invalid);
            persistentClassInfo.Name = "name";

            Validator.RuleSet.StateOf<RuleRequiredField>(persistentClassInfo, usedProperties)
                .Should()
                .Be(ValidationState.Valid);
        }

        [Theory, WorldCreatorAutoData]
        public void Name_Is_Valid_Code_Identifier(WCTestData testData, IPersistentClassInfo persistentClassInfo){
            persistentClassInfo.Name = "123";

            Validator.RuleSet.StateOf<RuleValidCodeIdentifier>(persistentClassInfo, nameof(persistentClassInfo.Name))
                .Should().Be(ValidationState.Invalid);
        }

        [Theory, WorldCreatorAutoData]
        public void Name_Is_Unique(IWCTestData testData){
            var objectSpace = testData.Application.CreateObjectSpace();
            var templatedTypeInfo = objectSpace.Create<IPersistentTemplatedTypeInfo>();
            templatedTypeInfo.Name = "name";
            var persistentClassInfo = objectSpace.Create<IPersistentClassInfo>();
            persistentClassInfo.Name = "name";
            
            objectSpace.CommitChanges();
            var usedProperties = nameof(persistentClassInfo.Name);


            Validator.RuleSet.StateOf<RuleObjectExists>(persistentClassInfo, usedProperties)
                .Should()
                .Be(ValidationState.Valid);
            
            var classInfo = objectSpace.Create<IPersistentClassInfo>();
            classInfo.Name = "name";

            Validator.RuleSet.StateOf<RuleObjectExists>(classInfo, usedProperties)
                .Should()
                .Be(ValidationState.Invalid);
        }
    }
}