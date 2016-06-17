using System.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Xpo;

namespace Xpand.Test {
    public class TestXafApplication:XafApplication {
        private readonly DataSet _dataSet;

        public TestXafApplication(DataSet dataSet){
            _dataSet = dataSet;
            DatabaseVersionMismatch+=OnDatabaseVersionMismatch;
        }

        private void OnDatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e){
            e.Updater.Update();
            e.Handled = true;
        }

        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args){
            args.ObjectSpaceProvider = new XPObjectSpaceProvider(new MemoryDataStoreProvider(_dataSet));
        }

        protected override LayoutManager CreateLayoutManagerCore(bool simple){
            return new TestLayoutManager();
        }
    }

    public class TestLayoutManager:LayoutManager{
        protected override object GetContainerCore(){
            return null;
        }
    }
}
