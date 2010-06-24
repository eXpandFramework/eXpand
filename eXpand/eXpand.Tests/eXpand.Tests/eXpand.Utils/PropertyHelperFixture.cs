using System.Reflection;
using eXpand.Utils.Helpers;
using MbUnit.Framework;

namespace eXpand.Tests.eXpand.Utils
{
    [TestFixture]
    public class PropertyHelperFixture
    {
        [Test]
        public void Test()
        {
            PropertyInfo prop = PropertyHelper<PropertyHelperFixture>.GetProperty(x => x.PropertyName);

            Assert.AreEqual("PropertyName", prop.Name);
        }

        public string PropertyName { get; set; }
    }
}