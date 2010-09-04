using System.Reflection;
using MbUnit.Framework;
using Xpand.Utils.Helpers;

namespace Xpand.Tests.Xpand.Utils
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