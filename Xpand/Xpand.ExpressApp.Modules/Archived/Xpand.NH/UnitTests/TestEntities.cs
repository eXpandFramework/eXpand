using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestEntities;
using TestDataLayer;
using System.Collections;
using Xpand.ExpressApp.NH.DataLayer;

namespace UnitTests
{
    [TestClass]
    public class TestEntities : NHibernateTestBase
    {
        [TestMethod]
        public void TestCreateObjects()
        {
            PersistenceManager pm = CreatePersistenceManager();
            var person =
                new Person { FirstName = "Sergej", LastName = "Derjabkin", BirthDate = new DateTime(2014, 1, 1) };

            person.PhoneNumbers.Add(new PhoneNumber { Number = "555-777" });
            pm.UpdateObjects(new[] { person }, null);

            IList objects = pm.GetObjects("From Person");
            Assert.AreEqual(1, objects.Count);
            person = (Person)objects[0];
            Assert.AreEqual("Sergej", person.FirstName);
            Assert.AreEqual("Derjabkin", person.LastName);
            Assert.AreEqual(1, person.PhoneNumbers.Count);
            Assert.AreEqual(person, person.PhoneNumbers[0].Person);
        }


    }
}
