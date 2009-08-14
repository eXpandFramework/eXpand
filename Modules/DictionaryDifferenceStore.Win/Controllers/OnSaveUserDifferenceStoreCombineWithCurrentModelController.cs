using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;
using eXpand.ExpressApp.DictionaryDifferenceStore.Controllers;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Win.Controllers
{
    public partial class OnSaveUserDifferenceStoreCombineWithCurrentModelController : OnSaveCombineWithCurrentModelController<XpoUserModelDictionaryDifferenceStore>
    {
        public OnSaveUserDifferenceStoreCombineWithCurrentModelController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
    }
}