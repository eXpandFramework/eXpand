using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.Security;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public class XpoUserModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore{
        

        public XpoUserModelDictionaryDifferenceStore(XafApplication application)
            : base(application){
        }

        public override DifferenceType DifferenceType{
            get { return DifferenceType.User; }
        }


        protected internal override ModelDifferenceObject GetActiveDifferenceObject(){
            return new QueryUserModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(Application.GetType().FullName);
        }

        protected internal IQueryable<ModelDifferenceObject> GetActiveDifferenceObjects(){
            IQueryable<UserModelDifferenceObject> modelDifferenceObjects = new QueryUserModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(
                Application.GetType().FullName);
            List<RoleModelDifferenceObject> roleAspectObjects = new QueryRoleModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(
                Application.GetType().FullName).ToList();
            IEnumerable<ModelDifferenceObject> roleAspectObjectsConcat = roleAspectObjects.Cast<ModelDifferenceObject>().Concat(modelDifferenceObjects.Cast<ModelDifferenceObject>());
            return roleAspectObjectsConcat.AsQueryable();
        }


        protected override Dictionary LoadDifferenceCore(Schema schema)
        {
            
                
            var dictionary = new Dictionary(schema);
            
            foreach (var aspect in Application.Model.Aspects){
                dictionary.AddAspect(aspect, new DictionaryNode("Application"));
            }
            var modelDifferenceObjects = GetActiveDifferenceObjects().ToList();
            if (modelDifferenceObjects.Count() == 0){
                SaveDifference(dictionary);
                return dictionary;
            }
            return CombineWithActiveDifferenceObjects(modelDifferenceObjects);
//            Dictionary combinedModel = activeAspectObjects[0].GetCombinedModel();
//            foreach (ModelDifferenceObject modelStoreObject in activeAspectObjects){
//                combinedModel.CombineWith(modelStoreObject);
//            }
        }

        public Dictionary CombineWithActiveDifferenceObjects(List<ModelDifferenceObject> modelDifferenceObjects){
            Dictionary combinedModel = modelDifferenceObjects[0].GetCombinedModel(Application.Model);
            foreach (var modelDifferenceObject in modelDifferenceObjects){
                combinedModel.CombineWith(modelDifferenceObject.Model);
            }
            return combinedModel.GetDiffs();
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(ObjectSpace session){
            var aspectObject = new UserModelDifferenceObject(ObjectSpace.Session);
            aspectObject.AssignToCurrentUser();
            return aspectObject;
        }

        protected internal override void OnAspectStoreObjectSaving(ModelDifferenceObject userModelDifferenceObject, Dictionary diffDictionary){
            var userStoreObject = ((UserModelDifferenceObject) userModelDifferenceObject);
            if (!userStoreObject.NonPersistent){
                base.OnAspectStoreObjectSaving(userModelDifferenceObject, diffDictionary);
            }
            if (SecuritySystem.IsGranted(new ApplicationModelCombinePermission(ApplicationModelCombineModifier.Allow))){
                ObjectSpace space = Application.CreateObjectSpace();
                ModelDifferenceObject activeModelDifferenceObject =
                    new QueryModelDifferenceObject(space.Session).GetActiveModelDifference(userStoreObject.PersistentApplication.UniqueName);
                if (activeModelDifferenceObject != null){
                    Dictionary combinedModel = activeModelDifferenceObject.GetCombinedModel();
                    combinedModel.CombineWith(userModelDifferenceObject.Model);
                    activeModelDifferenceObject.Model=combinedModel;
                    space.CommitChanges();
                }
            }
            
        }


    }
}