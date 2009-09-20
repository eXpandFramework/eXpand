using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public abstract class XpoDictionaryDifferenceStore : DictionaryDifferenceStore{
        private readonly XafApplication application;
        private readonly Session session;


        protected XpoDictionaryDifferenceStore(Session session, XafApplication application){
            this.session = session;
            this.application = application;
        }

        public XafApplication Application{
            get { return application; }
        }


        public Session Session{
            get { return session; }
        }

        public override string Name{
            get { return DifferenceType.ToString(); }
        }

        public abstract DifferenceType DifferenceType { get; }


        public override void SaveDifference(Dictionary diffDictionary){
            if (diffDictionary != null){
                var applicationName = Application.Title;
                ModelDifferenceObject modelDifferenceObject = GetActiveDifferenceObject() ??
                                                              GetNewDifferenceObject(session).
                                                                  InitializeMembers(applicationName, application.GetType().FullName);
                OnAspectStoreObjectSaving(modelDifferenceObject,diffDictionary);
            }
        }


        protected internal abstract ModelDifferenceObject GetActiveDifferenceObject();

        protected internal abstract ModelDifferenceObject GetNewDifferenceObject(Session session);

        protected internal virtual void OnAspectStoreObjectSaving(ModelDifferenceObject modelDifferenceObject, Dictionary diffDictionary){
            var combiner = new DictionaryCombiner(modelDifferenceObject.Model);
            combiner.AddAspects(diffDictionary);
            modelDifferenceObject.Save();
            if (session is UnitOfWork)
                ((UnitOfWork) session).CommitChanges();
        }

    }
}