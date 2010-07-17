using System;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.Conditional.Logic;

namespace eXpand.ExpressApp.MasterDetail.Logic {
    public class MasterDetailRule : ConditionalLogicRule, IMasterDetailRule
    {
        readonly IMasterDetailRule _masterDetailRule;

        public MasterDetailRule(IMasterDetailRule masterDetailRule)
            : base(masterDetailRule)
        {
            _masterDetailRule = masterDetailRule;
        }

        public IModelListView ChildListView {
            get { return _masterDetailRule.ChildListView; }
            set { _masterDetailRule.ChildListView=value; }
        }

        public IModelMember CollectionMember {
            get { return _masterDetailRule.CollectionMember; }

            set { throw new NotImplementedException();}
        }
    }
}