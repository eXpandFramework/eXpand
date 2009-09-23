using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.Controllers
{
    public partial class RunTimeMembersController : ViewController
    {
        public RunTimeMembersController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (ModelDifferenceObject);
            TargetViewType=ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (isCurreAppDiffObject()){
                View.ObjectSpace.ObjectSaved+=ObjectSpaceOnObjectSaved;
            }
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            if (isCurreAppDiffObject())
                View.ObjectSpace.ObjectSaving-=ObjectSpaceOnObjectSaved;
        }
        private void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs args){
            DictionaryHelper.AddFields(((ModelDifferenceObject) View.CurrentObject).Model.RootNode, XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        }

        private bool isCurreAppDiffObject(){
            return ((ModelDifferenceObject) View.CurrentObject).PersistentApplication.UniqueName==Application.GetType().FullName;
        }
    }
}
