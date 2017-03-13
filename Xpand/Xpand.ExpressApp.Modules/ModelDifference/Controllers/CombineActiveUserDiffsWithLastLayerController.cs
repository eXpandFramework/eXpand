using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public class CombineActiveUserDiffsWithLastLayerController : ViewController<DetailView> {

        public CombineActiveUserDiffsWithLastLayerController() {
            TargetObjectType = typeof(UserModelDifferenceObject);
        }

        protected override void OnActivated() {
            base.OnActivated();
            var userModelDifferenceObject = ((UserModelDifferenceObject)View.CurrentObject);
            if (userModelDifferenceObject != null)
                if ( ReferenceEquals(GetDifference(Application.GetType().FullName, userModelDifferenceObject.Name), userModelDifferenceObject)){
                    var lastLayer = ((ModelApplicationBase)Application.Model).LastLayer;
                    userModelDifferenceObject.CreateAspectsCore(lastLayer);
                    ObjectSpace.CommitChanges();
                }
            ObjectSpace.ObjectSaved+=ObjectSpaceOnObjectSaved;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.ObjectSaved-=ObjectSpaceOnObjectSaved;
        }

        private void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs args){
            var userModelDifferenceObject = args.Object as UserModelDifferenceObject;
            if (userModelDifferenceObject != null &&ReferenceEquals(GetDifference(Application.GetType().FullName, userModelDifferenceObject.Name),userModelDifferenceObject)){
                var applicationModel = (ModelApplicationBase) Application.Model;
                var model = applicationModel.CreatorInstance.CreateModelApplication();
                model.Id = applicationModel.LastLayer.Id;
                foreach (var aspectObject in userModelDifferenceObject.AspectObjects.Where(o => !string.IsNullOrWhiteSpace(o.Xml)))
                    new ModelXmlReader().ReadFromString(model, userModelDifferenceObject.GetAspectName(aspectObject),
                        aspectObject.Xml);
                ModelApplicationHelper.RemoveLayer(applicationModel);
                ModelApplicationHelper.AddLayer(applicationModel, model);
            }

        }


        protected virtual ModelDifferenceObject GetDifference(string applicationName, string name) {
            return new QueryUserModelDifferenceObject(((XPObjectSpace)View.ObjectSpace).Session).GetActiveModelDifference(applicationName, name);

        }

    }
}