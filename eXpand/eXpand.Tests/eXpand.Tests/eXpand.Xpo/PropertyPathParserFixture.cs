using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using eXpand.Xpo.Parser;
using eXpand.Xpo.PersistentMetaData;
using MbUnit.Framework;

namespace eXpand.Tests.eXpand.Xpo
{
    [TestFixture]
    public class PropertyPathParserFixture:eXpandBaseFixture
    {

        [Test]
        public void When_PropertyPath_Is_A_Reference_Object_Without_Chain()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            persistentClassInfo.AddReferenceMemberInfo(new PersistentClassInfo { Name = "Category" });
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> { persistentClassInfo});

            var propertyPathParser = new PropertyPathParser(persistentClassInfo.ClassInfo);
            CriteriaOperator criteriaOperator = propertyPathParser.Parse("Category","SomeCategory");

            Assert.AreEqual("[Category] = 'SomeCategory'", criteriaOperator.ToString());
        }
        [Test]
        public void When_PropertyPath_Is_A_Collection_Without_Chain()
        {
            var persistentClassInfo = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            persistentClassInfo.AddCollectionMemberInfo("Order", "Orders");
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> {persistentClassInfo});
            var parser = new PropertyPathParser(persistentClassInfo.ClassInfo);

            CriteriaOperator criteriaOperator= parser.Parse("Orders", "Amount=50");

            Assert.AreEqual("[Orders][[Amount] = 50]", criteriaOperator.ToString());
        }
        [Test]
        public void When_PropertyPath_Is_A_Reference_Object_With_A_Reference_Object_In_Chain()
        {
            var customer = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            var order = new PersistentClassInfo(Session.DefaultSession) { Name = "Order" };
            customer.AddReferenceMemberInfo(order);
            var orderLine = new PersistentClassInfo(Session.DefaultSession) { Name = "OrderLine" };
            order.AddReferenceMemberInfo(orderLine);
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> { customer,order,orderLine });

            var parser = new PropertyPathParser(customer.ClassInfo);

            CriteriaOperator criteriaOperator = parser.Parse("Order.OrderLine", "Amount=50");

//            Assert.AreEqual("[Orders][[OrderLines][[Amount] = 50]]", criteriaOperator.ToString());
            Assert.AreEqual("[Order.OrderLine] = \'Amount=50\'", criteriaOperator.ToString());
        }
        [Test]
        public void When_PropertyPath_Is_A_Reference_Object_With_A_Collection_In_Chain()
        {
            var customer = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            var order = new PersistentClassInfo(Session.DefaultSession) {Name = "Order"};
            customer.AddReferenceMemberInfo(order);
            order.AddCollectionMemberInfo("OrderLine", "OrderLines");
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> { customer, order});
            var parser = new PropertyPathParser(customer.ClassInfo);

            CriteriaOperator criteriaOperator= parser.Parse("Order.OrderLines", "Amount=50");

            Assert.AreEqual("[Order.OrderLines][[Ammount] = 50]", criteriaOperator.ToString());
        }
        [Test]
        public void When_PropertyPath_Is_A_Collection_With_A_Reference_Object_In_Chain()
        {
            var customer = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            var order = new PersistentClassInfo(Session.DefaultSession) { Name = "Order" };
            customer.AddCollectionMemberInfo(order, "Orders");
            var orderLine = new PersistentClassInfo(Session.DefaultSession) { Name = "OrderLine" };
            var referenceOrderMemberInfo = new PersistentReferenceMemberInfo(Session.DefaultSession,
                                                                        new PersistentAssociationAttribute(
                                                                            Session.DefaultSession) { AssociationName = "OrderLine" }) { Name = "OrderLine", ReferenceType = orderLine };
            order.OwnMembers.Add(referenceOrderMemberInfo);
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> { customer, order, orderLine });

            var parser = new PropertyPathParser(Session.DefaultSession.Dictionary.GetClassInfo("", customer.Name));

            CriteriaOperator criteriaOperator = parser.Parse("Order.OrderLine", "Ampunt=50");

            //            Assert.AreEqual("[Orders][[OrderLines][[Amount] = 50]]", criteriaOperator.ToString());
            Assert.AreEqual("[Order.OrderLine] = \'Ampunt=50\'", criteriaOperator.ToString());
        }
        [Test]
        public void When_PropertyPath_Is_A_Collection_With_A_Reference_Object_Chain_In_Chain()
        {
            
        }
        [Test]
        public void When_Parameter_Is_A_Chain()
        {
            throw new NotImplementedException();
        }
    }
}
