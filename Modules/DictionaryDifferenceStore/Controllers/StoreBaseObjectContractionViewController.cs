using DevExpress.ExpressApp;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Controllers
{
    public partial class StoreBaseObjectContractionViewController : ViewController
    {
        public StoreBaseObjectContractionViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (XpoModelDictionaryDifferenceStore);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.CurrentObject != null && View.ObjectSpace.Session.IsNewObject(View.CurrentObject))
                XpoModelDictionaryDifferenceStoreBuilder.SetUp((XpoModelDictionaryDifferenceStore) View.CurrentObject,
                                                               Application.GetType().FullName);
                
        }
    }
}
