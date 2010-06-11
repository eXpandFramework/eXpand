using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.Security;
using eXpand.Persistent.Base;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model;

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
            List<ModelDifferenceObject> allLayers =
                new List<ModelDifferenceObject>(new QueryRoleModelDifferenceObject(ObjectSpace.Session)
                    .GetActiveModelDifferences(Application.GetType().FullName).Cast<ModelDifferenceObject>());

            allLayers.AddRange(new QueryUserModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(Application.GetType().FullName).Cast<ModelDifferenceObject>());
            return allLayers.AsQueryable();
        }

        public override void Load(ModelApplicationBase model)
        {
            base.Load(model);
            
            var modelDifferenceObjects = GetActiveDifferenceObjects().ToList();
            if (modelDifferenceObjects.Count() == 0)
            {
                SaveDifference(model);
                return;
            }
            
            CombineWithActiveDifferenceObjects(model, modelDifferenceObjects);
        }

        public void CombineWithActiveDifferenceObjects(ModelApplicationBase model, List<ModelDifferenceObject> modelDifferenceObjects)
        {
            ModelXmlReader reader = new ModelXmlReader();
            ModelXmlWriter writer = new ModelXmlWriter();
            foreach (var modelDifferenceObject in modelDifferenceObjects){
                for (int i = 0; i < modelDifferenceObject.Model.AspectCount; i++)
                {
                    var xml = writer.WriteToString(modelDifferenceObject.Model, i);
                    if (!string.IsNullOrEmpty(xml))
                        reader.ReadFromString(model, modelDifferenceObject.Model.GetAspect(i), xml);
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
                base.OnDifferenceObjectSaving(userModelDifferenceObject, model);
            }

            if (SecuritySystem.Instance is ISecurityComplex && IsGranted()){
                var reader = new ModelXmlReader();
                var writer = new ModelXmlWriter();
                var space = Application.CreateObjectSpace();
                IQueryable<ModelDifferenceObject> differences = GetDifferences(space);
                foreach (var difference in differences){
                    for (int i = 0; i < model.AspectCount; i++)
                    {
                        var xml = writer.WriteToString(model, i);
                        if (!string.IsNullOrEmpty(xml))
                            reader.ReadFromString(difference.Model, model.GetAspect(i), xml);
                    }

                    space.SetModified(difference);
                }
                space.CommitChanges();
            }
        }

        private IQueryable<ModelDifferenceObject> GetDifferences(ObjectSpace space){
            return new QueryModelDifferenceObject(space.Session).GetModelDifferences(
                ((IUser) SecuritySystem.CurrentUser).Permissions.OfType<ModelCombinePermission>().Select(
                    permission => permission.Difference));
        }

        private bool IsGranted(){            
            var securityComplex = ((SecurityComplex) SecuritySystem.Instance);
            bool permission = securityComplex.IsGrantedForNonExistentPermission;
            securityComplex.IsGrantedForNonExistentPermission = false;
            bool granted = SecuritySystem.IsGranted(new ModelCombinePermission(ApplicationModelCombineModifier.Allow));
            securityComplex.IsGrantedForNonExistentPermission=permission;
            return granted;
        }
    }
}