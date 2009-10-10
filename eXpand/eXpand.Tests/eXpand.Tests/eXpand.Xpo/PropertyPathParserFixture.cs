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
            CriteriaOperator criteriaOperator = propertyPathParser.Parse("Category","SomeProperty=50");

            Assert.AreEqual(new BinaryOperator("Category.SomeProperty",50).ToString(), criteriaOperator.ToString());
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

            Assert.AreEqual(new BinaryOperator("Order.OrderLine.Amount", 50).ToString(), criteriaOperator.ToString());
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

            CriteriaOperator criteriaOperator= parser.Parse("Order.OrderLines", "Ammount=50");

            Assert.AreEqual(new ContainsOperator("Order.OrderLines",new BinaryOperator("Ammount",50)).ToString(), criteriaOperator.ToString());
        }
        [Test]
        public void When_PropertyPath_Is_A_Reference_Object_With_A_Collection_And_A_Reference_In_Chain()
        {
            var company = new PersistentClassInfo(Session.DefaultSession) { Name = "Company" };
            var customer = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            company.AddReferenceMemberInfo(customer);
            var order = new PersistentClassInfo(Session.DefaultSession) {Name = "Order"};
            customer.AddCollectionMemberInfo(order);
            var orderLine = new PersistentClassInfo(Session.DefaultSession) { Name = "OrderLine" };
            order.AddReferenceMemberInfo(orderLine);
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> {company, customer, order,orderLine});
            var parser = new PropertyPathParser(company.ClassInfo);

            CriteriaOperator criteriaOperator= parser.Parse("Customer.Orders.OrderLine", "Ammount=50");

            Assert.AreEqual(new ContainsOperator("Customer.Orders",new BinaryOperator("OrderLine.Ammount",50)).ToString(), criteriaOperator.ToString());
        }
        [Test]
        public void When_PropertyPath_Is_A_Reference_Object_With_A_Collection_And_A_Collection_In_Chain()
        {
            var company = new PersistentClassInfo(Session.DefaultSession) { Name = "Company" };
            var customer = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            company.AddReferenceMemberInfo(customer);
            var order = new PersistentClassInfo(Session.DefaultSession) {Name = "Order"};
            customer.AddCollectionMemberInfo(order);
            var orderLine = new PersistentClassInfo(Session.DefaultSession) { Name = "OrderLine" };
            order.AddCollectionMemberInfo(orderLine);
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> {company, customer, order,orderLine});
            var parser = new PropertyPathParser(company.ClassInfo);

            CriteriaOperator criteriaOperator= parser.Parse("Customer.Orders.OrderLines", "Ammount=50");

            Assert.AreEqual(new ContainsOperator("Customer.Orders",new ContainsOperator("OrderLines",new BinaryOperator("Ammount",50))).ToString(), criteriaOperator.ToString());
        }
        [Test]
        public void When_PropertyPath_Is_A_Collection_With_A_Reference_Object_In_Chain()
        {
            var customer = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            var order = new PersistentClassInfo(Session.DefaultSession) { Name = "Order" };
            customer.AddCollectionMemberInfo(order, "Orders");
            var orderLine = new PersistentClassInfo(Session.DefaultSession) { Name = "OrderLine" };
            order.AddReferenceMemberInfo(orderLine);
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> { customer, order, orderLine });
            var parser = new PropertyPathParser(Session.DefaultSession.Dictionary.GetClassInfo("", customer.Name));

            CriteriaOperator criteriaOperator = parser.Parse("Orders.OrderLine", "Ampunt=50");


            Assert.AreEqual(new ContainsOperator("Orders", new BinaryOperator("OrderLine.Ampunt", 50)).ToString(), criteriaOperator.ToString());
        }
        [Test]
        public void When_PropertyPath_Is_A_Collection_With_A_Reference_Object_Chain_In_Chain()
        {
            var company = new PersistentClassInfo(Session.DefaultSession) { Name = "Company" };
            var customer = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            company.AddCollectionMemberInfo(customer);
            var order = new PersistentClassInfo(Session.DefaultSession) { Name = "Order" };
            customer.AddReferenceMemberInfo(order);
            var orderLine = new PersistentClassInfo(Session.DefaultSession) { Name = "OrderLine" };
            order.AddReferenceMemberInfo(orderLine);
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> { company, customer, order, orderLine });
            var parser = new PropertyPathParser(company.ClassInfo);

            CriteriaOperator criteriaOperator = parser.Parse("Customers.Order.OrderLine", "Ammount=50");

            Assert.AreEqual(new ContainsOperator("Customers", new BinaryOperator("Order.OrderLine.Ammount",50)).ToString(), criteriaOperator.ToString());
        }
        [Test]
        public void When_Parameter_Is_A_Chain()
        {
            var company = new PersistentClassInfo(Session.DefaultSession) { Name = "Company" };
            var customer = new PersistentClassInfo(Session.DefaultSession) { Name = "Customer" };
            company.AddCollectionMemberInfo(customer);
            var order = new PersistentClassInfo(Session.DefaultSession) { Name = "Order" };
            customer.AddReferenceMemberInfo(order);
            var orderLine = new PersistentClassInfo(Session.DefaultSession) { Name = "OrderLine" };
            order.AddReferenceMemberInfo(orderLine);
            Session.DefaultSession.Dictionary.AddClasses(new List<PersistentClassInfo> { company, customer, order, orderLine });
            var parser = new PropertyPathParser(company.ClassInfo);

            CriteriaOperator criteriaOperator = parser.Parse("Customers", "Order.OrderLine.Ammount=50");

            Assert.AreEqual(new ContainsOperator("Customers", new BinaryOperator("Order.OrderLine.Ammount", 50)).ToString(), criteriaOperator.ToString());
        }
    }
}
