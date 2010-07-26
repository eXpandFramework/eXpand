using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ModelDifference.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public abstract class XpoDictionaryDifferenceStore : ModelDifferenceStore{

        private readonly XafApplication application;
        private ObjectSpace objectSpace;

        protected XpoDictionaryDifferenceStore(XafApplication application){
            this.application = application;
            objectSpace = application.CreateObjectSpace();
        }

        public XafApplication Application{
            get { return application; }
        }

        public ObjectSpace ObjectSpace{
            get { return objectSpace; }
        }

        public override string Name{
            get { return DifferenceType.ToString(); }
        }

        public abstract DifferenceType DifferenceType { get; }

        public override void SaveDifference(ModelApplicationBase model){
            if (model != null){
                objectSpace = application.CreateObjectSpace();
                ModelDifferenceObject modelDifferenceObject = 
                    GetActiveDifferenceObject(model.Id) ?? 
                    GetNewDifferenceObject(objectSpace)
                    .InitializeMembers(model.Id);
                
                OnDifferenceObjectSaving(modelDifferenceObject, model);
            }
        }
        
        protected internal abstract ModelDifferenceObject GetActiveDifferenceObject(string modelId);

        protected internal abstract ModelDifferenceObject GetNewDifferenceObject(ObjectSpace objectSpace);

        protected internal virtual void OnDifferenceObjectSaving(ModelDifferenceObject modelDifferenceObject, ModelApplicationBase model){
            modelDifferenceObject.Model = model;
            objectSpace.CommitChanges();
        }
    }
}