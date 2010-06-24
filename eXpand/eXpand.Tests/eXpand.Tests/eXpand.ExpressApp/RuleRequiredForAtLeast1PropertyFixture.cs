using System.Reflection;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;
using MbUnit.Framework;

namespace eXpand.Tests.eXpand.ExpressApp
{
    [TestFixture]
    public class RuleRequiredForAtLeast1PropertyFixture
    {
        private UnitOfWork unitOfWork;

        [SetUp]
        public void Setup()
        {
            unitOfWork = new UnitOfWork();
            unitOfWork.ClearDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            unitOfWork.Dispose();
        }
        [Test]
        public void WhenBothPropertiesAreNotNull()
        {
            var myClass = new MyClass(unitOfWork){Prop1 = "fddf",Prop2 = "dfdfg"};
            Validator.RuleSet.Validate(myClass,ContextIdentifier.Save);
            ValidationState state = Validator.RuleSet.ValidateTarget(myClass,ContextIdentifier.Save).State;

            Assert.AreEqual(ValidationState.Valid, state);
        }
        [Test][ExpectedException(typeof(ValidationException))]
        public void WhenBothPropertiesAreNull()
        {
            var myClass = new MyClass(unitOfWork);
            Validator.RuleSet.Validate(myClass,ContextIdentifier.Save);
        }
        [Test]
        public void WhenFirstPropertiesIsNull()
        {
            var myClass = new MyClass(unitOfWork) {Prop2 = "ffff"};
            Validator.RuleSet.Validate(myClass,ContextIdentifier.Save);
        }

        [Test]
        public void WhenSecondPropertiesIsNull()
        {
            var myClass = new MyClass(unitOfWork) {Prop1 = "ffff"};
            Validator.RuleSet.Validate(myClass,ContextIdentifier.Save);
        }
    }

    [RuleRequiredForAtLeast1Property(null, DefaultContexts.Save, "Prop1,Prop2")]
    class MyClass : BaseObject
    {
        public MyClass(Session session)
            : base(session)
        {
        }
        private string prop1;
        public string Prop1
        {
            get
            {
                return prop1;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref prop1, value);
            }
        }
        private string prop2;
        public string Prop2
        {
            get
            {
                return prop2;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref prop2, value);
            }
        }
    }
}