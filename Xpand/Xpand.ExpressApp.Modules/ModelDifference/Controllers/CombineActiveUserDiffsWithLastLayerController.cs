using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public interface IModelOptionsApplicationModelDiffs : IModelOptions {
        [DefaultValue(true)]
        [Description("When an active user model difference is saved then it will be combined with the application user model difference")]
        [Category(ModelDifferenceModule.ModelDifferenceCategory)]
        bool CombineActiveUserDiffsWithLastLayerOnSave { get; set; }
        [Category(ModelDifferenceModule.ModelDifferenceCategory)]
        [DefaultValue(true)]
        [Description("When a user model difference is loaded will be compiled with the application user model difference")]
        bool CombineLastLayerWithActiveUserDiffsOnLoad { get; set; }
    }
    public class CombineActiveUserDiffsWithLastLayerController : ViewController<DetailView>, IModelExtender {
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
            if (userModelDifferenceObject != null)
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
            var userModelDifferenceObject = args.Object as UserModelDifferenceObject;
            if (userModelDifferenceObject != null && ReferenceEquals(GetDifference(Application.GetType().FullName, userModelDifferenceObject.Name), userModelDifferenceObject)) {
                var modelApplicationBase = ((ModelApplicationBase)Application.Model).LastLayer;
                foreach (var aspectObject in userModelDifferenceObject.AspectObjects){
                    var name = aspectObject.Name == CaptionHelper.DefaultLanguage ? "" : aspectObject.Name;
                    if (!string.IsNullOrEmpty(aspectObject.Xml))
                        new ModelXmlReader().ReadFromString(modelApplicationBase, name, aspectObject.Xml);
                }
            }
        }

        protected virtual ModelDifferenceObject GetDifference(string applicationName, string name) {
            return new QueryUserModelDifferenceObject(((XPObjectSpace)View.ObjectSpace).Session).GetActiveModelDifference(applicationName, name);

        }

        #region IModelExtender Members

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsApplicationModelDiffs>();
        }

        #endregion
    }
}