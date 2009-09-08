using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters;
using eXpand.Persistent.BaseImpl;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects{
    public class PersistentApplication:eXpandBaseObject{
        private string _name;

        public PersistentApplication(Session session) : base(session){
        }
        [RuleUniqueValue(null,DefaultContexts.Save)]
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
//        private Schema schema;
//        [RuleRequiredField(null,DefaultContexts.Save)]
//        [Size(SizeAttribute.Unlimited)]
//        [ValueConverter(typeof(SchemaValueConverter))]
//        public Schema Schema
//        {
//            get
//            {
//                return schema;
//            }
//            set
//            {
//                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref schema, value);
//            }
//        }
        private Dictionary _model=new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName),Schema.GetCommonSchema());
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(DictionaryValueConverter))]
        public Dictionary Model
        {
            get
            {
                return _model;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _model, value);
            }
        }
    }
}