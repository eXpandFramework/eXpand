using DevExpress.ExpressApp;
using Ploeh.AutoFixture;
using Xpand.ExpressApp.WorldCreator;

namespace Xpand.Test.WorldCreator.TestArtifacts{
    public interface IWCTestData : ITestData{
        WorldCreatorModule WorldCreatorModule { get; }
        Window MainWindow { get; }
    }

    public class WCTestData : TestData, IWCTestData{
        public WCTestData(IObjectSpace objectSpace, IFixture fixture, XafApplication application,
            WorldCreatorModule worldCreatorModule, Window mainWindow) : base(objectSpace, fixture, application){
            WorldCreatorModule = worldCreatorModule;
            MainWindow = mainWindow;
        }

        public WorldCreatorModule WorldCreatorModule { get; }
        public Window MainWindow { get; }
    }
}