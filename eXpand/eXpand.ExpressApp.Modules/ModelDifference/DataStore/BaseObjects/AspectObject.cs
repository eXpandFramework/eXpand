using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    public class AspectObject:XpandCustomObject {
        public AspectObject(Session session) : base(session) {
        }
        private string _name;

        [RuleUniqueValue(null, DefaultContexts.Save)]
        [RuleRequiredField]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
        private ModelDifferenceObject _modelDifferenceObject;

        [Association("ModelDifferenceObject-AspectObjects")]
        public ModelDifferenceObject ModelDifferenceObject {
            get { return _modelDifferenceObject; }
            set { SetPropertyValue("ModelDifferenceObject", ref _modelDifferenceObject, value); }
        }
        private string _xml;
        [Size(SizeAttribute.Unlimited)]
        public string Xml {
            get { return _xml; }
            set { SetPropertyValue("Xml", ref _xml, value); }
        }
    }
}