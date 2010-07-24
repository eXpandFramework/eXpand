using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public interface IModelOptionsApplicationModelDiffs:IModelOptions
    {
        [DefaultValue(true)]
        [Description("When an active model difference is saved then it will be combined with application model difference")]
        [Category("eXpand.ModelDifference")]
        bool SynchronizeApplicationModelDiffs { get; set; }    
    }
    public class SyncronizeApplicationModelDiffsController: ViewController 
    {
        public SyncronizeApplicationModelDiffsController()
        {
            TargetObjectType = typeof(ModelDifferenceObject);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectSaving += ObjectSpaceOnObjectSaving;
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            ObjectSpace.ObjectSaved -= ObjectSpaceOnObjectSaving;
        }

        internal void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args){

            var store = (args.Object) as ModelDifferenceObject;
            if (store != null && ReferenceEquals(GetDifference(Application.GetType().FullName, store.ModelId), store))
            {
                var lastLayer = ((ModelApplicationBase)Application.Model).LastLayer;
                var cloneLayer = lastLayer.Clone();
                ((ModelApplicationBase)Application.Model).RemoveLayer(lastLayer);
                ((ModelApplicationBase)Application.Model).AddLayer(store.Model.Clone());
                ((ModelApplicationBase)Application.Model).AddLayer(cloneLayer);
            }
        }
        protected virtual ModelDifferenceObject GetDifference(string applicationName, string modelId) {
            return new QueryModelDifferenceObject(View.ObjectSpace.Session).GetActiveModelDifference(applicationName,modelId);

        }
    }
}