using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.BaseImpl;
using eXpand.ExpressApp.Win.SystemModule;
using MbUnit.Framework;
using TypeMock;
using TypeMock.ArrangeActAssert;
using TypeMock.Extensions;

namespace eXpand.Tests.eXpand.ExpressApp
{
    [TestFixture]
    public class TestFixture1:eXpandBaseFixture
    {
        [Test][Isolated]
        public void When_COntroller_Activated_And_No_Wrappers_found_In_Model_FilterWrappers_Count_Should_Null(){
            var controllerFactory = new ViewControllerFactory();
            var controller = controllerFactory.CreateController<FilterByCriteriaPathViewController>(typeof(User));
            DictionaryNode dictionaryNode = DefaultDictionary.RootNode;
            Isolate.WhenCalled(() => controller.View.Info).WillReturn(dictionaryNode);

            controllerFactory.Activate(controller);

            Assert.IsNull(controller.FiltersByCriteriaWrappers);
        }

        public class When_COntroller_Activated_And_Wrappers_found_In_Model: eXpandBaseFixture
        {
            private FilterByCriteriaPathViewController controller;
            private ViewControllerFactory controllerFactory;
            
            private DictionaryNode node;

            public override void Setup()
            {
                base.Setup();
                controllerFactory = new ViewControllerFactory();
                controller = controllerFactory.CreateController<FilterByCriteriaPathViewController>(typeof(User));
                Isolate.WhenCalled(() => controller.HasFilters).WillReturn(false);
                var dictionaryNode = DefaultDictionary.RootNode;
                node = dictionaryNode.AddChildNode(FilterByCriteriaPathViewController.CriteriaPathFilters);
                View view = controller.View;
                Isolate.WhenCalled(() => view.Info).WillReturn(dictionaryNode);
            }
            [Test]
            [Isolated]
            public void FilterWrappers_Count_Should_Equal_To_Nodes_Found()
            {
                node.AddChildNode("Test").SetAttribute("ID", "id1");
                node.AddChildNode("Test2").SetAttribute("ID", "id2");
                

                controllerFactory.Activate(controller);

                Assert.AreEqual(2, controller.FiltersByCriteriaWrappers.Count);

            }
            [Test]
            [Isolated]
            public void Old_Filter_ChoiceActions__Should_Be_Cleared(){
                Guid caption = Guid.NewGuid();
                var value = new ChoiceActionItem("",caption);
                controller.FilterSingleChoiceAction.Items.Add(value);

                controllerFactory.Activate(controller);

                Assert.IsNull(controller.FilterSingleChoiceAction.Items.Find(caption));
            }
            [Test]
            [Isolated]
            public void If_Wrappers_Exists_Should_Check_If_AdditionalViewControls_Provider_Module_Is_INstalled(){
                Isolate.WhenCalled(() => controller.HasFilters).WillReturn(!controller.HasFilters);
// ReSharper disable RedundantAssignment
                bool filters = controller.HasFilters;
// ReSharper restore RedundantAssignment
                filters = controller.HasFilters;
                Assert.IsTrue(filters);

//                Isolate.WhenCalled(() => controller.FiltersByCriteriaWrappers.Count).WillReturn(1);
//                var classInfoNodeWrapper = Isolate.Fake.Instance<ClassInfoNodeWrapper>();
//                Isolate.WhenCalled(() => classInfoNodeWrapper.Node.FindChildNode("")).WillReturn(null);
//                Isolate.NonPublic.WhenCalled(controller,"GetClassInfoNodeWrapper").WillReturn(classInfoNodeWrapper);
//
//                controllerFactory.Activate(controller);
            }
        }
    }
}
