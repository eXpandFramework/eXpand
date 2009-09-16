﻿using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.Persistent.BaseImpl;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects
{
    [NonPersistent]
    public abstract class DifferenceObject:eXpandBaseObject
    {
        private Dictionary _model ;
        protected DifferenceObject(Session session) : base(session){
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _model = new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema());
        }
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(ValueConverters.DictionaryValueConverter))]
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
