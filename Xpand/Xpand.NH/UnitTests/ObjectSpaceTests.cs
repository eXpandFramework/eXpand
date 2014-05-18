using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestEntities;
using TestDataLayer;
using System.Collections;
using Xpand.ExpressApp.NH;
using Xpand.ExpressApp.NH.DataLayer;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;

namespace UnitTests
{
    [TestClass]
    public class ObjectSpaceTests : NHibernateTestBase
    {
        [TestMethod]
        public void TestRefreshObjectInstance()
        {
            PersistenceManager pm = CreatePersistenceManager();
            NHObjectSpaceProvider provider = new NHObjectSpaceProvider(XafTypesInfo.Instance, pm);
            using (var os = provider.CreateObjectSpace())
            {
                var person = os.CreateObject<Person>();
                person.FirstName = "Max";
                person.LastName = "Mustermann";
                person.BirthDate = new DateTime(2000, 1, 1);
                os.CommitChanges();
                Assert.AreNotEqual(Guid.Empty, person.Id);

                Assert.AreSame(person, os.FindObject<Person>(new BinaryOperator("Id", person.Id)));
            }
        }

    }
}
