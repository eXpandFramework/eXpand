using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;

using MbUnit.Framework;
using TypeMock.ArrangeActAssert;
using Xpand.Xpo;
using Xpand.Xpo.Parser;

namespace Xpand.Tests.Xpand.Xpo
{
    [TestFixture]
    public class PropertyPathParserFixture
    {
        [SetUp]
        public void Setup() {
            Isolate.Fake.StaticMethods(typeof(XpandReflectionHelper));
        }
        [Test][Isolated]
        public void When_PropertyPath_Is_A_Reference_Object_Without_Chain()
        {
            XPMemberInfo associationMemberInfo = GetAssociationMemberInfo();
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null,"Category")).WithExactArguments().WillReturn(associationMemberInfo);
            var propertyPathParser = new PropertyPathParser(null);
            CriteriaOperator criteriaOperator = propertyPathParser.Parse("Category","SomeProperty=50");

            Assert.AreEqual(new BinaryOperator("Category.SomeProperty",50).ToString(), criteriaOperator.ToString());
        }
        [Test]
        [Isolated]
        public void When_PropertyPath_Is_A_Collection_Without_Chain()
        {
            XPMemberInfo collectionMemberInfo = GetCollectionMemberInfo();
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null,"Orders")).WithExactArguments().WillReturn(collectionMemberInfo);
            var parser = new PropertyPathParser(null);

            CriteriaOperator criteriaOperator= parser.Parse("Orders", "Amount=50");

            Assert.AreEqual("[Orders][[Amount] = 50]", criteriaOperator.ToString());
        }
        [Test]
        [Isolated]
        public void When_PropertyPath_Is_A_Reference_Object_With_A_Reference_Object_In_Chain()
        {   
            XPMemberInfo associationMemberInfo = GetAssociationMemberInfo();
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null,"Order")).WillReturn(associationMemberInfo);
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Order.OrderLine")).WillReturn(associationMemberInfo);
            var parser = new PropertyPathParser(null);

            CriteriaOperator criteriaOperator = parser.Parse("Order.OrderLine", "Amount=50");

            Assert.AreEqual(new BinaryOperator("Order.OrderLine.Amount", 50).ToString(), criteriaOperator.ToString());
        }

        private XPMemberInfo GetCollectionMemberInfo() {
            var collectionMemberInfo = Isolate.Fake.Instance<XPMemberInfo>();
            Isolate.WhenCalled(() => collectionMemberInfo.IsCollection).WillReturn(true);
            return collectionMemberInfo;
        }

        private XPMemberInfo GetAssociationMemberInfo() {
            var associationMemberInfo = Isolate.Fake.Instance<XPMemberInfo>();
            
            return associationMemberInfo;
        }

        [Test]
        [Isolated]
        public void When_PropertyPath_Is_A_Reference_Object_With_A_Collection_In_Chain()
        {
            XPMemberInfo collectionMemberInfo = GetCollectionMemberInfo();
            XPMemberInfo associationMemberInfo = GetAssociationMemberInfo();
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null,"Order")).WithExactArguments().WillReturn(associationMemberInfo);
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null,"Order.OrderLines")).WithExactArguments().WillReturn(collectionMemberInfo);
            var parser = new PropertyPathParser(null);

            CriteriaOperator criteriaOperator= parser.Parse("Order.OrderLines", "Ammount=50");

            Assert.AreEqual(new ContainsOperator("Order.OrderLines",new BinaryOperator("Ammount",50)).ToString(), criteriaOperator.ToString());
        }
        [Test]
        [Isolated]
        public void When_PropertyPath_Is_A_Reference_Object_With_A_Collection_And_A_Reference_In_Chain()
        {
            XPMemberInfo collectionMemberInfo = GetCollectionMemberInfo();
            XPMemberInfo associationMemberInfo = GetAssociationMemberInfo();
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customer")).WithExactArguments().WillReturn(associationMemberInfo);
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customer.Orders")).WithExactArguments().WillReturn(collectionMemberInfo);
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customer.Orders.OrderLine")).WithExactArguments().WillReturn(associationMemberInfo);
            var parser = new PropertyPathParser(null);

            CriteriaOperator criteriaOperator= parser.Parse("Customer.Orders.OrderLine", "Ammount=50");

            Assert.AreEqual(new ContainsOperator("Customer.Orders",new BinaryOperator("OrderLine.Ammount",50)).ToString(), criteriaOperator.ToString());
        }
        [Test]
        [Isolated]
        public void When_PropertyPath_Is_A_Reference_Object_With_A_Collection_And_A_Collection_In_Chain()
        {
            XPMemberInfo collectionMemberInfo = GetCollectionMemberInfo();
            XPMemberInfo associationMemberInfo = GetAssociationMemberInfo();
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customer")).WithExactArguments().WillReturn(associationMemberInfo);
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customer.Orders")).WithExactArguments().WillReturn(collectionMemberInfo);
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customer.Orders.OrderLines")).WithExactArguments().WillReturn(collectionMemberInfo);
            var parser = new PropertyPathParser(null);

            CriteriaOperator criteriaOperator= parser.Parse("Customer.Orders.OrderLines", "Ammount=50");

            Assert.AreEqual(new ContainsOperator("Customer.Orders",new ContainsOperator("OrderLines",new BinaryOperator("Ammount",50))).ToString(), criteriaOperator.ToString());
        }
        [Test]
        [Isolated]
        public void When_PropertyPath_Is_A_Collection_With_A_Reference_Object_In_Chain()
        {
            XPMemberInfo collectionMemberInfo = GetCollectionMemberInfo();
            XPMemberInfo associationMemberInfo = GetAssociationMemberInfo();
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Orders")).WithExactArguments().WillReturn(collectionMemberInfo);
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Orders.OrderLine")).WithExactArguments().WillReturn(associationMemberInfo);
            var parser = new PropertyPathParser(null);

            CriteriaOperator criteriaOperator = parser.Parse("Orders.OrderLine", "Ampunt=50");


            Assert.AreEqual(new ContainsOperator("Orders", new BinaryOperator("OrderLine.Ampunt", 50)).ToString(), criteriaOperator.ToString());
        }
        [Test]
        [Isolated]
        public void When_PropertyPath_Is_A_Collection_With_A_Reference_Object_Chain_In_Chain()
        {
            XPMemberInfo collectionMemberInfo = GetCollectionMemberInfo();
            XPMemberInfo associationMemberInfo = GetAssociationMemberInfo();
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customers")).WithExactArguments().WillReturn(collectionMemberInfo);
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customers.Order")).WithExactArguments().WillReturn(associationMemberInfo);
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customers.Order.OrderLine")).WithExactArguments().WillReturn(associationMemberInfo);
            var parser = new PropertyPathParser(null);

            CriteriaOperator criteriaOperator = parser.Parse("Customers.Order.OrderLine", "Ammount=50");

            Assert.AreEqual(new ContainsOperator("Customers", new BinaryOperator("Order.OrderLine.Ammount",50)).ToString(), criteriaOperator.ToString());
        }
        [Test]
        [Isolated]
        public void When_Parameter_Is_A_Chain()
        {
            XPMemberInfo collectionMemberInfo = GetCollectionMemberInfo();
            Isolate.WhenCalled(() => XpandReflectionHelper.GetXpMemberInfo(null, "Customers")).WithExactArguments().WillReturn(collectionMemberInfo);
            var parser = new PropertyPathParser(null);

            CriteriaOperator criteriaOperator = parser.Parse("Customers", "Order.OrderLine.Ammount=50");

            Assert.AreEqual(new ContainsOperator("Customers", new BinaryOperator("Order.OrderLine.Ammount", 50)).ToString(), criteriaOperator.ToString());
        }
    }
}
