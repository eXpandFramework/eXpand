using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public abstract class XpoDictionaryDifferenceStore : DictionaryDifferenceStore{
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


        public override void SaveDifference(Dictionary diffDictionary){
            if (diffDictionary != null){
                objectSpace = application.CreateObjectSpace();
                ModelDifferenceObject modelDifferenceObject = GetActiveDifferenceObject() ??
                                                              GetNewDifferenceObject(objectSpace).
                                                                  InitializeMembers(Application.Title, application.GetType().FullName);
                OnDifferenceObjectSaving(modelDifferenceObject,diffDictionary);
            }
        }


        protected internal abstract ModelDifferenceObject GetActiveDifferenceObject();

        protected internal abstract ModelDifferenceObject GetNewDifferenceObject(ObjectSpace objectSpace);

        protected internal virtual void OnDifferenceObjectSaving(ModelDifferenceObject modelDifferenceObject, Dictionary diffDictionary){
            Dictionary dictionary = modelDifferenceObject.GetCombinedModel(modelDifferenceObject is UserModelDifferenceObject);
            dictionary.CombineWith(diffDictionary);
            modelDifferenceObject.Model = dictionary.GetDiffs();
            objectSpace.CommitChanges();
        }

    }
}