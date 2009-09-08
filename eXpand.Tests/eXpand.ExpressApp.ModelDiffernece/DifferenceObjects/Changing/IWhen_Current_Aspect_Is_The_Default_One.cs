using MbUnit.Framework;
using TypeMock.ArrangeActAssert;

namespace eXpand.Tests.eXpand.ExpressApp.ModelDiffernece.DifferenceObjects.Changing{
    public interface IWhen_Current_Aspect_Is_The_Default_One{
        [Test]
        [Isolated]
        void When_Changing_XmlContent_Then_It_Should_Be_Added_To_Aspects_Collection();

        [Test]
        [Isolated]
        void When_Changing_Model_Then_Should_Be_Added_Apsects_Collection();
    }
}