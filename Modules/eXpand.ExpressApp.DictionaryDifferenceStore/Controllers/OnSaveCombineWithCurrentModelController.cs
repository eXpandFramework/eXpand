using System.ComponentModel;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Controllers
{
    public partial class OnSaveCombineWithCurrentModelController : ViewController
    {
        public OnSaveCombineWithCurrentModelController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (XpoModelDictionaryDifferenceStore);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.Committing+=ObjectSpaceOnCommitting;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs args)
        {
            var store = ((XpoModelDictionaryDifferenceStore) View.CurrentObject);
            XpoUserModelDictionaryDifferenceStore activeStore =
                XpoUserModelDictionaryDifferenceStoreBuilder.GetActiveStore(store.Session, store.DifferenceType, 
                                                                            Application.GetType().FullName);
            if (store.Equals(activeStore))
                Application.Model.AddAspect(store.Aspect,store.Model);
        }
    }
}
