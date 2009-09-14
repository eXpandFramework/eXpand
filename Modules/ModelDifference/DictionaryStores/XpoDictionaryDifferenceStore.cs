using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Validation;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;
using eXpand.Utils.Helpers;

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
                var persistentApplication = modelDifferenceObject.PersistentApplication;
                persistentApplication.Name = applicationName;
                persistentApplication.UniqueName = application.GetType().FullName;
                OnAspectStoreObjectSaving(modelDifferenceObject,diffDictionary);
            }
        }


        protected internal abstract ModelDifferenceObject GetActiveDifferenceObject();

        protected internal abstract ModelDifferenceObject GetNewDifferenceObject(Session session);

        protected internal virtual void OnAspectStoreObjectSaving(ModelDifferenceObject modelDifferenceObject, Dictionary diffDictionary){
//            const DefaultContexts identifiers = DefaultContexts.Save;
//            var objectsToValidate = new SaveContextTargetObjectSelector().GetObjectsToValidate(session, modelDifferenceObject);
//            var ruleSet = Validator.RuleSet;
//            ruleSet.CustomValidateRule += RuleSetOnCustomValidateRule;
//            ruleSet.ValidateAll(objectsToValidate, identifiers,args => { });
            
            modelDifferenceObject.Save();
            if (session is UnitOfWork)
                ((UnitOfWork) session).CommitChanges();
        }

/*
        private void RuleSetOnCustomValidateRule(object sender, CustomValidateRuleEventArgs args){
            if (args.Rule is RuleRequiredField&&args.Target is PersistentApplication){
                var persistentApplication = ((PersistentApplication) args.Target);
                if (args.Rule.UsedProperties.Contains(persistentApplication.GetPropertyInfo(x=>persistentApplication.Model).Name))
                    args.RuleValidationResult=new RuleValidationResult(args.Rule, args.Target, ValidationState.Valid,null);
            }
        }
*/
    }
}