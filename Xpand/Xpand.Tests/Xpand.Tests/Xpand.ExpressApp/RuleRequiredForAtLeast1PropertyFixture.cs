using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using MbUnit.Framework;
using Xpand.Persistent.Base.Validation.AtLeast1PropertyIsRequired;

namespace Xpand.Tests.Xpand.ExpressApp
{
    [TestFixture]
    public class RuleRequiredForAtLeast1PropertyFixture
    {
        private UnitOfWork _unitOfWork;
        XPObjectSpace _objectSpace;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new UnitOfWork();
            _unitOfWork.ClearDatabase();
            _objectSpace = new XPObjectSpace(XafTypesInfo.Instance, ((XpoTypeInfoSource) XafTypesInfo.PersistentEntityStore), () => _unitOfWork);
        }

        [TearDown]
        public void TearDown()
        {
            _unitOfWork.Dispose();
        }
        [Test]
        public void WhenBothPropertiesAreNotNull()
        {
            var myClass = new MyClass(_unitOfWork){Prop1 = "fddf",Prop2 = "dfdfg"};
            Validator.RuleSet.Validate(_objectSpace, myClass,ContextIdentifier.Save);
            ValidationState state = Validator.RuleSet.ValidateTarget(_objectSpace, myClass,ContextIdentifier.Save).State;

            Assert.AreEqual(ValidationState.Valid, state);
        }
        [Test][ExpectedException(typeof(ValidationException))]
        public void WhenBothPropertiesAreNull()
        {
            var myClass = new MyClass(_unitOfWork);
            Validator.RuleSet.Validate(_objectSpace, myClass,ContextIdentifier.Save);
        }
        [Test]
        public void WhenFirstPropertiesIsNull()
        {
            var myClass = new MyClass(_unitOfWork) {Prop2 = "ffff"};
            Validator.RuleSet.Validate(_objectSpace, myClass,ContextIdentifier.Save);
        }

        [Test]
        public void WhenSecondPropertiesIsNull()
        {
            var myClass = new MyClass(_unitOfWork) {Prop1 = "ffff"};
            Validator.RuleSet.Validate(_objectSpace, myClass,ContextIdentifier.Save);
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