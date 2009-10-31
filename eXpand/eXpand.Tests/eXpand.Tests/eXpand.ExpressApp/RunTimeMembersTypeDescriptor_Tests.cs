using System.ComponentModel;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp;
using MbUnit.Framework;
using System.Linq;

namespace eXpand.Tests.eXpand.ExpressApp {
    [TestFixture]
    public class RunTimeMembersTypeDescriptor_Tests:XpandBaseFixture
    {
        [Test]
        public void Runtime_Properties_Can_BeValidated() {
            TypeDescriptor.AddProvider(new RuntimeMembersTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(User))), typeof(User));
            var member = Session.DefaultSession.GetClassInfo(typeof(User)).CreateMember("Test",typeof(string));
            member.AddAttribute(new RuleRequiredFieldAttribute(null,DefaultContexts.Save));

            var validationResult = Validator.RuleSet.ValidateTarget(new User(Session.DefaultSession),ContextIdentifier.Save);

            ValidationState validationState = validationResult.Results.Where(item => item.Rule.UsedProperties.Contains("Test")).Single().State;
            Assert.AreEqual(ValidationState.Invalid, validationState);
        }
    }
}