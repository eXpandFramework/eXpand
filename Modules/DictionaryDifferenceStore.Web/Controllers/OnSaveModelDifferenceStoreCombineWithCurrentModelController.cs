using System.ComponentModel;
using eXpand.ExpressApp.DictionaryDifferenceStore.BaseObjects;
using eXpand.ExpressApp.DictionaryDifferenceStore.Controllers;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Web.Controllers
{
    public partial class OnSaveModelDifferenceStoreCombineWithCurrentModelController : OnSaveCombineWithCurrentModelController<XpoModelDictionaryDifferenceStore>
    {
        public OnSaveModelDifferenceStoreCombineWithCurrentModelController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
    }
}