using DevExpress.ExpressApp.Model.Core;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [NonPersistent]
    public abstract class DifferenceObject : eXpandCustomObject {

        protected DifferenceObject(Session session) : base(session) {
        }

        [Persistent, Delayed, Size(SizeAttribute.Unlimited), ValueConverter(typeof(ModelValueConverter))]
        public ModelApplicationBase Model {
            get { return GetDelayedPropertyValue<ModelApplicationBase>("Model"); }
            set {
                SetDelayedPropertyValue("Model", value);
            }
        }
    }
}