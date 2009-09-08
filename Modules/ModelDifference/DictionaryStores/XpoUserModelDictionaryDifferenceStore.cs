using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.Security;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public class XpoUserModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore{
        public XpoUserModelDictionaryDifferenceStore(Session session, XafApplication application)
            : base(session, application){
            
        }

        public override DifferenceType DifferenceType{
            get { return DifferenceType.User; }
        }


        protected internal override ModelDifferenceObject GetActiveDifferenceObject(){
            return new QueryUserModelDifferenceObject(Session).GetActiveModelDifference(Application.ApplicationName);
        }

        protected internal IQueryable<ModelDifferenceObject> GetActiveDifferenceObjects(){
            IQueryable<UserModelDifferenceObject> modelDifferenceObjects = new QueryUserModelDifferenceObject(Session).GetActiveModelDifferences(Application.ApplicationName);
            List<RoleModelDifferenceObject> roleAspectObjects = new QueryRoleModelDifferenceObject(Session).GetActiveModelDifferences(Application.ApplicationName).ToList();
            IEnumerable<ModelDifferenceObject> roleAspectObjectsConcat = roleAspectObjects.Cast<ModelDifferenceObject>().Concat(modelDifferenceObjects.Cast<ModelDifferenceObject>());
            return roleAspectObjectsConcat.AsQueryable();
        }


        protected override Dictionary LoadDifferenceCore(Schema schema)
        {
            var dictionary = new Dictionary(new DictionaryNode("Application"), schema);

            var activeAspectObjects = GetActiveDifferenceObjects();
            if (activeAspectObjects.Count() == 0){
                SaveDifference(dictionary);
                return dictionary;
            }
            foreach (ModelDifferenceObject modelStoreObject in activeAspectObjects){
                var combiner = new DictionaryCombiner(dictionary);
                combiner.CombineWith(modelStoreObject);
            }
            return dictionary;
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(Session session){
            var aspectObject = new UserModelDifferenceObject(session);
            aspectObject.AssignToCurrentUser();
            return aspectObject;
        }

        protected internal override void OnAspectStoreObjectSaving(ModelDifferenceObject userModelDifferenceObject){
            var userStoreObject = ((UserModelDifferenceObject) userModelDifferenceObject);
            if (!userStoreObject.NonPersistent){
                base.OnAspectStoreObjectSaving(userModelDifferenceObject);
            }
            if (SecuritySystem.IsGranted(new ApplicationModelCombinePermission(ApplicationModelCombineModifier.Allow))){
                ModelDifferenceObject activeModelDifferenceObject =
                    new QueryModelDifferenceObject(Session).GetActiveModelDifference(userStoreObject.PersistentApplication.Name);
                if (activeModelDifferenceObject != null){
                    var dictionary = new Dictionary(activeModelDifferenceObject.Model.RootNode, Application.Model.Schema);
                    var combiner = new DictionaryCombiner(dictionary);
                    combiner.CombineWith(userStoreObject);
                    activeModelDifferenceObject.Model=dictionary;
                    activeModelDifferenceObject.Save();
                }
            }
            
        }


    }
}