using System.Linq;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;
using eXpand.Utils.Helpers;
using eXpand.Xpo;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects{
    [TestFixture]
    public class Saving_ModelDifferenceObjects : XpandBaseFixture
    {
        [Row(DifferenceType.Role, ValidationState.Skipped)]
        [Row(DifferenceType.User, ValidationState.Skipped)]
        [Row(DifferenceType.Model, ValidationState.Invalid)]
        [Test]
        [Isolated]
        [MultipleAsserts]
        public void Same_Application_Objets_Cannot_Exist(DifferenceType differenceType, ValidationState validationState)
        {
            var ruleSet = new RuleSet();
            var persistentApplication = new PersistentApplication(Session.DefaultSession);
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = persistentApplication };
            modelDifferenceObject.Save();

            var modelDifferenceObject1 = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = persistentApplication };
            Isolate.WhenCalled(() => modelDifferenceObject1.DifferenceType).WillReturn(differenceType);

            RuleSetValidationResult target = ruleSet.ValidateTarget(modelDifferenceObject1, ContextIdentifier.Save);


            Assert.IsInstanceOfType(typeof(RuleCombinationOfPropertiesIsUnique), target.Results[0].Rule);
            Assert.AreEqual(validationState, target.Results[0].State);


        }
        [Row(DifferenceType.Role, ValidationState.Skipped)]
        [Row(DifferenceType.User, ValidationState.Skipped)]
        [Row(DifferenceType.Model, ValidationState.Skipped)]
        [Test]
        [Isolated]
        public void Many_Disabled_Objects_With_Same_ApplicationName_DifferenceType_Can_Exist(DifferenceType differenceType, ValidationState validationState)
        {
            var ruleSet = new RuleSet();

            var persistentApplication = new PersistentApplication(Session.DefaultSession);
            new ModelDifferenceObject(Session.DefaultSession) { Disabled = true, PersistentApplication = persistentApplication }.Save();

            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { Disabled = true, PersistentApplication = persistentApplication };
            Isolate.WhenCalled(() => modelDifferenceObject.DifferenceType).WillReturn(differenceType);
            RuleSetValidationResult target = ruleSet.ValidateTarget(modelDifferenceObject, ContextIdentifier.Save);

            var resultItem = target.Results.Where(item => item.Rule is RuleCombinationOfPropertiesIsUnique).Single();
            Assert.AreEqual(validationState, resultItem.State);
        }
        [Test]
        [Row(DifferenceType.Role, ValidationState.Invalid)]
        [Row(DifferenceType.User, ValidationState.Invalid)]
        [Row(DifferenceType.Model, ValidationState.Invalid)]
        [Isolated]
        public void DateCreated_Can_Not_Be_Null(DifferenceType differenceType, ValidationState validationState)
        {

            var o = new ModelDifferenceObject(Session.DefaultSession);
            Isolate.WhenCalled(() => o.DifferenceType).WillReturn(differenceType);
            var ruleSet = new RuleSet();

            RuleSetValidationResult target = ruleSet.ValidateTarget(o, ContextIdentifier.Save);

            RuleSetValidationResultItem @default = target.Results.Where(
                item =>
                item.Rule is RuleRequiredField &&
                ((RuleRequiredField)item.Rule).TargetMember.Name == o.GetPropertyInfo(x => x.DateCreated).Name).FirstOrDefault();
            Assert.AreEqual(validationState, @default.State);
        }
        [Test]
        [Isolated]
        public void PersistentApplication_Is_Required()
        {
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = null };

            var validationResult = new RuleSet().ValidateTarget(modelDifferenceObject, ContextIdentifier.Save);

            var validationResultItem = validationResult.Results.Where(item => item.Rule is RuleRequiredField && item.Rule.UsedProperties.Contains(modelDifferenceObject.GetPropertyInfo(x => x.PersistentApplication).Name)).FirstOrDefault();
            Assert.IsNull(modelDifferenceObject.PersistentApplication);
            Assert.IsNotNull(validationResultItem);
            Assert.AreEqual(ValidationState.Invalid, validationResultItem.State);
        }
        [Test]
        [Isolated]
        public void Same_ApplicationName_Differences_Cannot_Exist()
        {
            new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession) { UniqueName = "appName" } }.Save();

            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { PersistentApplication = new PersistentApplication(Session.DefaultSession) { UniqueName = "appName" } };
            var objectsToValidate = new SaveContextTargetObjectSelector().GetObjectsToValidate(Session.DefaultSession, modelDifferenceObject);

            RuleSetValidationResult targets = Validator.RuleSet.ValidateAllTargets(objectsToValidate, ContextIdentifier.Save);

            var resultItem = targets.Results.Where(
                item =>
                item.Rule is RuleUniqueValue &&
                item.Rule.UsedProperties.Contains(
                    modelDifferenceObject.GetPropertyInfo(x => modelDifferenceObject.PersistentApplication.UniqueName).Name)).
                FirstOrDefault();

            Assert.IsNotNull(resultItem);
            Assert.AreEqual(ValidationState.Invalid, resultItem.State);
        }
        [Test]
        [Isolated]
        public void Model_Is_Persistent()
        {
            var dictionary = DefaultDictionary;
            dictionary.AddAspect("el",elDictionary.RootNode);
            var modelDifferenceObject = new ModelDifferenceObject(Session.DefaultSession) { Model = dictionary,PersistentApplication = new PersistentApplication(Session.DefaultSession)};
            modelDifferenceObject.Save();

            var o = (ModelDifferenceObject)new Session(XpoDefault.DataLayer).GetObject(modelDifferenceObject);

            Assert.AreEqual(dictionary.RootNode.ToXml(), o.Model.RootNode.ToXml());
        }

    }
}