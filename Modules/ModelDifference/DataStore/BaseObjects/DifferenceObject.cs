using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.Persistent.BaseImpl;
using System.Linq;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects
{
    [NonPersistent]
    public abstract class DifferenceObject:eXpandBaseObject
    {
        private Dictionary _model = new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema());
        protected DifferenceObject(Session session) : base(session){
        }
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(ValueConverters.DictionaryValueConverter))]
        public Dictionary Model
        {
            get
            {
                return _model;
                //                if (!IsDeleted){
                //                    var dictionary = new Dictionary(_model.RootNode,PersistentApplication.Model.Schema);
                //                    var combiner = new DictionaryCombiner(dictionary);
                //                    combiner.AddAspects(_model);
                //                    dictionary.CurrentAspectProvider.CurrentAspect=_model.CurrentAspect;
                //                    return dictionary;
                //                }
                //                return null;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _model, value);
            }
        }

    }
}
