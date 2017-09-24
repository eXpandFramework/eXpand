using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelDifference;

namespace Xpand.ExpressApp.ModelDifference.DictionaryStores {
    public abstract class XpoDictionaryDifferenceStore : ModelDifferenceStore {
        private readonly XPObjectSpace _objectSpace;

        protected XpoDictionaryDifferenceStore(XafApplication application) {
            Application = application;
            _objectSpace = (XPObjectSpace)application.CreateObjectSpace(typeof(ModelDifferenceObject));
        }


        public XafApplication Application { get; }

        public XPObjectSpace ObjectSpace => _objectSpace;

        public override string Name => DifferenceType.ToString();

        public abstract DifferenceType DifferenceType { get; }

        public override void SaveDifference(ModelApplicationBase model) {
            if (model != null){
                var deviceCategories = new[] {DeviceCategory.All};
                if (DeviceModelsEnabled)
                    deviceCategories = deviceCategories.Concat(new[]{Application.GetDeviceCategory()}).ToArray();
                foreach (var deviceCategory in deviceCategories){
                    var modelId = $"{model.Id}-{deviceCategory}";
                    ModelDifferenceObject modelDifferenceObject =
                        GetActiveDifferenceObject(modelId,deviceCategory) ??
                        GetNewDifferenceObject(_objectSpace)
                            .InitializeMembers(modelId == "Application" ? Application.Title : modelId, Application.Title, Application.GetType().FullName,deviceCategory);
                    if (!_objectSpace.IsNewObject(modelDifferenceObject))
                        _objectSpace.ReloadObject(modelDifferenceObject);
                    OnDifferenceObjectSaving(modelDifferenceObject, model);
                }
            }
        }

        protected abstract bool DeviceModelsEnabled{ get; }

        protected internal abstract ModelDifferenceObject GetActiveDifferenceObject(string name,DeviceCategory deviceCategory);

        protected internal abstract ModelDifferenceObject GetNewDifferenceObject(IObjectSpace objectSpace);

        

        protected internal virtual void OnDifferenceObjectSaving(ModelDifferenceObject modelDifferenceObject, ModelApplicationBase model) {
            ObjectSpace.CommitChanges();
        }
    }
}