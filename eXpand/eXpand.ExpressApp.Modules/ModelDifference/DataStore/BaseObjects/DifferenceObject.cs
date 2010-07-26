using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [NonPersistent]
    public abstract class DifferenceObject : eXpandCustomObject {

        protected DifferenceObject(Session session) : base(session) {
        }

        [Delayed, Size(SizeAttribute.Unlimited), ValueConverter(typeof(ModelValueConverter))]
        [Persistent("Model")]
        public ModelApplication ModelApplication {
            get { return GetDelayedPropertyValue<ModelApplication>("ModelApplication"); }
            set {
                SetDelayedPropertyValue("ModelApplication", value);
            }
        }
    }
}