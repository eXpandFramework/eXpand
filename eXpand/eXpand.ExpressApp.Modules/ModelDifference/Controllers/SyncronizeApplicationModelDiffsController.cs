using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public interface IModelOptionsApplicationModelDiffs:IModelOptions
    {
        [DefaultValue(true)]
        [Description("When an active model difference is saved then it will be combined with application model difference")]
        [Category("eXpand.ModelDifference")]
        bool SynchronizeApplicationModelDiffs { get; set; }    
    }
    public class SyncronizeApplicationModelDiffsController: ViewController ,IModelExtender
    {
        bool _synchronizeApplicationModelDiffs;

        public SyncronizeApplicationModelDiffsController()
        {
            TargetObjectType = typeof(ModelDifferenceObject);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            _synchronizeApplicationModelDiffs = ((IModelOptionsApplicationModelDiffs)Application.Model.Options).SynchronizeApplicationModelDiffs;
            if (_synchronizeApplicationModelDiffs)
                ObjectSpace.ObjectSaving += ObjectSpaceOnObjectSaving;
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            if (_synchronizeApplicationModelDiffs)
                ObjectSpace.ObjectSaved -= ObjectSpaceOnObjectSaving;
        }

        internal void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args){

            var store = (args.Object) as ModelDifferenceObject;
            if (store != null && ReferenceEquals(GetDifference(Application.GetType().FullName, store.Name), store)){
                ((ModelApplicationBase)Application.Model).AddLayerBeforeLast(store.Model.Clone());
            }
        }
        protected virtual ModelDifferenceObject GetDifference(string applicationName, string name) {
            return new QueryModelDifferenceObject(View.ObjectSpace.Session).GetActiveModelDifference(applicationName,name);

        }

        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptions, IModelOptionsApplicationModelDiffs>();
        }

        #endregion
    }
}