using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.Controllers
{
    public class RunTimeMembersController : ViewController
    {
        public RunTimeMembersController(){
            TargetObjectType = typeof (ModelDifferenceObject);
            TargetViewType=ViewType.DetailView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ObjectSpace.ObjectSaved+=ObjectSpaceOnObjectSaved;
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            View.ObjectSpace.ObjectSaving-=ObjectSpaceOnObjectSaved;
        }

        private void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs args){
            DictionaryHelper.AddFields(((ModelDifferenceObject) View.CurrentObject).Model.Application, XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        }
    }
}
