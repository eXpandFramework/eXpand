using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.SystemModule;
using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp
{
    [TestFixture]
    public class HidingFromNewMenu:XpandBaseFixture
    {
        [Test]
        [Isolated]
        public void Get_Decorated_Objects(){
            XafTypesInfo.Instance.RegisterEntity(typeof(Party));
            XafTypesInfo.Instance.RegisterEntity(typeof(Person));
            XafTypesInfo.Instance.FindTypeInfo(typeof(Person)).AddAttribute(new HideFromNewMenuAttribute());
            XafTypesInfo.Instance.RegisterEntity(typeof(User));
            XafTypesInfo.Instance.FindTypeInfo(typeof(User)).AddAttribute(new HideFromNewMenuAttribute());
            var controller = new ViewControllerFactory().CreateAndActivateController<HideFromNewMenuViewController>(typeof(Party));

            List<ITypeInfo> persistentBases = controller.GetHiddenTypes();

            Assert.AreEqual(2, persistentBases.Count);
        }

        [Test]
        [Isolated]
        public void When_Object_Is_Decorated_With_HideFromNewMenu_Attribute_Should_Not_Appear_To_Menu(){
            XafTypesInfo.Instance.RegisterEntity(typeof(Person));
            XafTypesInfo.Instance.FindTypeInfo(typeof (Person)).AddAttribute(new HideFromNewMenuAttribute());
            var controller = new NewObjectViewController();
            controller.NewObjectAction.Items.Add(new ChoiceActionItem("", typeof (Person)));
            var viewControllerFactory = new ViewControllerFactory();
            var hideFromNewMenuViewController = viewControllerFactory.CreateController<HideFromNewMenuViewController>(ViewType.Any, new Person(Session.DefaultSession));
            var frame = Isolate.Fake.Instance<Frame>(Members.CallOriginal);
            frame.RegisterController(controller);
            Isolate.WhenCalled(() => hideFromNewMenuViewController.Frame).WillReturn(frame);
            viewControllerFactory.Activate(hideFromNewMenuViewController, new HandleInfo{ControlsCreated = true});

            viewControllerFactory.ControlsCreatedHandler.Invoke(this,EventArgs.Empty);

            Assert.AreEqual(0, controller.NewObjectAction.Items.Count);

        }
    }
}
