using DevExpress.ExpressApp;
using Ploeh.AutoFixture;

namespace Xpand.Test{
    public interface ITestData {
        IObjectSpace ObjectSpace { get; }
        IFixture Fixture { get; }

        XafApplication Application { get; }
    }

    public class TestData:ITestData{
        public TestData(IObjectSpace objectSpace, IFixture fixture, XafApplication application){
            ObjectSpace = objectSpace;
            Fixture = fixture;
            Application = application;
        }

        public IObjectSpace ObjectSpace { get; }

        public IFixture Fixture { get; }

        public XafApplication Application { get; }
    }
}