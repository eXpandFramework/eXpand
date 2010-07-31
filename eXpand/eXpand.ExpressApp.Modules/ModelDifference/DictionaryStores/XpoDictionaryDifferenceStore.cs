using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public abstract class XpoDictionaryDifferenceStore : ModelDifferenceStore{

        private readonly XafApplication _application;
        private readonly ObjectSpace _objectSpace;

        protected XpoDictionaryDifferenceStore(XafApplication application){
            _application = application;
            _objectSpace = application.CreateObjectSpace();
        }

        public XafApplication Application{
            get { return _application; }
        }

        public ObjectSpace ObjectSpace{
            get { return _objectSpace  ; }
        }

        public override string Name{
            get { return DifferenceType.ToString(); }
        }

        public abstract DifferenceType DifferenceType { get; }

        public override void SaveDifference(ModelApplicationBase model){
            if (model != null){
                ModelDifferenceObject modelDifferenceObject = 
                    GetActiveDifferenceObject(model.Id) ?? 
                    GetNewDifferenceObject(ObjectSpace)
                    .InitializeMembers(model.Id=="Application"?Application.Title:model.Id, Application.Title, Application.GetType().FullName);
                
                OnDifferenceObjectSaving(modelDifferenceObject, model);
            }
        }
        
        protected internal abstract ModelDifferenceObject GetActiveDifferenceObject(string name);

        protected internal abstract ModelDifferenceObject GetNewDifferenceObject(ObjectSpace objectSpace);

        protected internal virtual void OnDifferenceObjectSaving(ModelDifferenceObject modelDifferenceObject, ModelApplicationBase model){
            if (model.HasModification)
                ObjectSpace.SetModified(modelDifferenceObject);
            ObjectSpace.CommitChanges();
        }
    }
}