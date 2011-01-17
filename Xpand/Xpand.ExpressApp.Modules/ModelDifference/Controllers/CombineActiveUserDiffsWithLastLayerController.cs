using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public interface IModelOptionsApplicationModelDiffs : IModelOptions {
        [DefaultValue(true)]
        [Description("When an active user model difference is saved then it will be combined with application model difference")]
        [Category("eXpand.ModelDifference")]
        bool CombineActiveUserDiffsWithLastLayerOnSave { get; set; }
        [Category("eXpand.ModelDifference")]
        [DefaultValue(true)]
        bool CombineLastLayerWithActiveUserDiffsOnLoad { get; set; }
    }
    public class CombineActiveUserDiffsWithLastLayerController : ViewController, IModelExtender {
        bool _combineActiveUserDiffsWithLastLayerOnSave;

        public CombineActiveUserDiffsWithLastLayerController() {
            TargetObjectType = typeof(UserModelDifferenceObject);
        }

        protected override void OnActivated() {
            base.OnActivated();
            _combineActiveUserDiffsWithLastLayerOnSave = ((IModelOptionsApplicationModelDiffs)Application.Model.Options).CombineActiveUserDiffsWithLastLayerOnSave;
            if (_combineActiveUserDiffsWithLastLayerOnSave)
                ObjectSpace.ObjectSaving += ObjectSpaceOnObjectSaving;
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var combineLastLayerWithActiveUserDiffsOnLoad = ((IModelOptionsApplicationModelDiffs)Application.Model.Options).CombineLastLayerWithActiveUserDiffsOnLoad;
            var userModelDifferenceObject = ((UserModelDifferenceObject) View.CurrentObject);
            if (combineLastLayerWithActiveUserDiffsOnLoad && ReferenceEquals(GetDifference(Application.GetType().FullName,userModelDifferenceObject.Name), userModelDifferenceObject)) {
                var lastLayer = ((ModelApplicationBase)Application.Model).LastLayer;
                userModelDifferenceObject.CreateAspectsCore(lastLayer);
            }
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_combineActiveUserDiffsWithLastLayerOnSave)
                ObjectSpace.ObjectSaved -= ObjectSpaceOnObjectSaving;
        }

        void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args) {
            var store = (args.Object) as UserModelDifferenceObject;
            if (store != null && ReferenceEquals(GetDifference(Application.GetType().FullName, store.Name), store)) {
                ModelApplicationBase modelApplicationBase = ((ModelApplicationBase)Application.Model).LastLayer;
                new ModelXmlReader().ReadFromModel(modelApplicationBase, store.Model);
            }
        }
        protected virtual ModelDifferenceObject GetDifference(string applicationName, string name) {
            return new QueryUserModelDifferenceObject(((ObjectSpace)View.ObjectSpace).Session).GetActiveModelDifference(applicationName, name);

        }

        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsApplicationModelDiffs>();
        }

        #endregion
    }
}