using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DictionaryStores {
    public abstract class XpoDictionaryDifferenceStore : ModelDifferenceStore {

        private readonly XafApplication _application;
        private ObjectSpace _objectSpace;

        protected XpoDictionaryDifferenceStore(XafApplication application) {
            _application = application;
            _objectSpace = application.CreateObjectSpace() as ObjectSpace;
        }

        public XafApplication Application {
            get { return _application; }
        }

        public ObjectSpace ObjectSpace {
            get { return _objectSpace; }
        }

        public override string Name {
            get { return DifferenceType.ToString(); }
        }

        public abstract DifferenceType DifferenceType { get; }

        public override void SaveDifference(ModelApplicationBase model) {
            if (model != null) {
                _objectSpace = _application.CreateObjectSpace() as ObjectSpace;
                ModelDifferenceObject modelDifferenceObject =
                    GetActiveDifferenceObject(model.Id) ??
                    GetNewDifferenceObject(_objectSpace)
                    .InitializeMembers(model.Id == "Application" ? Application.Title : model.Id, Application.Title, Application.GetType().FullName);

                OnDifferenceObjectSaving(modelDifferenceObject, model);
            }
        }

        protected internal abstract ModelDifferenceObject GetActiveDifferenceObject(string name);

        protected internal abstract ModelDifferenceObject GetNewDifferenceObject(IObjectSpace objectSpace);

        protected internal virtual void OnDifferenceObjectSaving(ModelDifferenceObject modelDifferenceObject, ModelApplicationBase model) {
            if (model.HasModification)
                ObjectSpace.SetModified(modelDifferenceObject);
            ObjectSpace.CommitChanges();
        }
    }
}