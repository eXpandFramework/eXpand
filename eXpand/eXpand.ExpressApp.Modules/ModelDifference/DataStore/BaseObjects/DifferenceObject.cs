using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [NonPersistent]
    public abstract class DifferenceObject : eXpandCustomObject {
        protected DifferenceObject(Session session) : base(session) {
        }

        [Delayed]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof (DictionaryValueConverter))]
        public Dictionary Model {
            get { return GetDelayedPropertyValue<Dictionary>("Model"); }
            set { SetDelayedPropertyValue("Model", value); }
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            Model = new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema());
        }
    }
}