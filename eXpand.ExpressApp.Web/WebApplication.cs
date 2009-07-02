using System.ComponentModel;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core;


namespace eXpand.ExpressApp.Web
{
    public partial class WebApplication : DevExpress.ExpressApp.Web.WebApplication
    {
//        private IObjectSpaceProvider objectSpaceProvider;
//        private const string XpoModelDictionaryDifferenceModuleTypeName = "XAFPoint.ExpressApp.DictionaryDifferenceStore.XpoModelDictionaryDifferenceStore, XAFPoint.ExpressApp.DictionaryDifferenceStore, Version=*, Culture=neutral, PublicKeyToken=c52ffed5d5ff0958";

        public WebApplication()
        {
            InitializeComponent();
            DatabaseVersionMismatch += (sender, args) => this.DatabaseVersionMismatchEvent(sender, args);

        }
//        protected override DictionaryDifferenceStore CreateModelDifferenceStoreCore()
//        {
//            Type type = Type.GetType(
//                XpoModelDictionaryDifferenceModuleTypeName);
//            if (type != null)
//                return
//                    (DictionaryDifferenceStore)
//                    Activator.CreateInstance(type, new object[] { objectSpaceProvider.CreateUpdatingSession(), this });
//
//            return base.CreateModelDifferenceStoreCore();
//        }

        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            this.CreateCustomObjectSpaceprovider(args);
            base.OnCreateCustomObjectSpaceProvider(args);
            
//            objectSpaceProvider = args.ObjectSpaceProvider;
        }

        public WebApplication(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }


    }
}