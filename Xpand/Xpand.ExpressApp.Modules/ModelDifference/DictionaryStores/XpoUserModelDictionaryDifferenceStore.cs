using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.ModelDifference.Security;
using Xpand.ExpressApp.Security.Core;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DictionaryStores{
    public class XpoUserModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore{
        
        public XpoUserModelDictionaryDifferenceStore(XafApplication application)
            : base(application){
        }

        public override DifferenceType DifferenceType{
            get { return DifferenceType.User; }
        }

        protected internal override ModelDifferenceObject GetActiveDifferenceObject(string name){
            return new QueryUserModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(Application.GetType().FullName,null);
        }

        protected internal IQueryable<ModelDifferenceObject> GetActiveDifferenceObjects(){
            var allLayers =
                new List<ModelDifferenceObject>(new QueryRoleModelDifferenceObject(ObjectSpace.Session)
                    .GetActiveModelDifferences(Application.GetType().FullName, null).Cast<ModelDifferenceObject>());

            allLayers.AddRange(new QueryUserModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(Application.GetType().FullName,null).Cast<ModelDifferenceObject>());
            return allLayers.AsQueryable();
        }

        public override void Load(ModelApplicationBase model)
        {
            base.Load(model);
            
            var modelDifferenceObjects = GetActiveDifferenceObjects().ToList();
            if (modelDifferenceObjects.Count() == 0){
                SaveDifference(model);
                return;
            }
            
            CombineWithActiveDifferenceObjects(model, modelDifferenceObjects);
        }

        void CombineWithActiveDifferenceObjects(ModelApplicationBase model, IEnumerable<ModelDifferenceObject> modelDifferenceObjects)
        {
            var reader = new ModelXmlReader();
            foreach (var modelDifferenceObject in modelDifferenceObjects){
                foreach (var aspectObject in modelDifferenceObject.AspectObjects) {
                    var xml = aspectObject.Xml;
                    if (!string.IsNullOrEmpty(xml))
                        reader.ReadFromString(model, modelDifferenceObject.GetAspectName(aspectObject), xml);
                }
            }
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(ObjectSpace session){
            var aspectObject = new UserModelDifferenceObject(ObjectSpace.Session);
            aspectObject.AssignToCurrentUser();
            return aspectObject;
        }

        protected internal override void OnDifferenceObjectSaving(ModelDifferenceObject userModelDifferenceObject, ModelApplicationBase model){
            var userStoreObject = ((UserModelDifferenceObject) userModelDifferenceObject);
            if (!userStoreObject.NonPersistent){
                if (!(model.AspectCount == 1 && string.IsNullOrEmpty(model.Xml))) {
                    var modelApplicationBuilder = new ModelApplicationBuilder(userStoreObject.PersistentApplication.ExecutableName);
                    userModelDifferenceObject.CreateAspects(model, modelApplicationBuilder.GetMasterModel());
                }
                base.OnDifferenceObjectSaving(userModelDifferenceObject, model);
            }

            if (SecuritySystem.Instance is ISecurityComplex && SecuritySystemExtensions.IsGranted(new ModelCombinePermission(ApplicationModelCombineModifier.Allow), false)){
                var space = Application.CreateObjectSpace();
                IEnumerable<ModelDifferenceObject> differences = GetDifferences(space);
                foreach (var difference in differences){
                    difference.CreateAspects(model);
                    space.SetModified(difference);
                }
                space.CommitChanges();
            }
        }

        private IEnumerable<ModelDifferenceObject> GetDifferences(ObjectSpace space){
            return new QueryModelDifferenceObject(space.Session).GetModelDifferences(
                ((IUser) SecuritySystem.CurrentUser).Permissions.OfType<ModelCombinePermission>().Select(
                    permission => permission.Difference));
        }

    }
}