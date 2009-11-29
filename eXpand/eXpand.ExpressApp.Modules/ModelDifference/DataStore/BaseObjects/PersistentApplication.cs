using System.ComponentModel;
using System.Reflection;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects{
    public class PersistentApplication:DifferenceObject{
        private string _name;

        public PersistentApplication(Session session) : base(session){
            
        }

//        private Dictionary _model = new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema());
        //        [Delayed]
//        [Size(SizeAttribute.Unlimited)]
//        [ValueConverter(typeof(ValueConverters.DictionaryValueConverter))]
//        public Dictionary Model
//        {
//            get
//            {
//                return GetDelayedPropertyValue<Dictionary>("Model");
//            }
//            set
//            {
//                SetDelayedPropertyValue("Model",value);
//            }
//        }

        [DevExpress.Xpo.DisplayName("Application Name")]
        [RuleRequiredField(null,DefaultContexts.Save)]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _name, value);
            }
        }
        
        [Association(Associations.PersistentApplicationModelDifferenceObjects)]
        public XPCollection<ModelDifferenceObject> ModelDifferenceObjects
        {
            get
            {
                return GetCollection<ModelDifferenceObject>(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
        }
        
        private string uniqueName;
        [RuleUniqueValue(null,DefaultContexts.Save)]
        [Browsable(false)][MemberDesignTimeVisibility(false)]
        public string UniqueName
        {
            get
            {
                return uniqueName;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref uniqueName, value);
            }
        }
    }
}