using System;
using System.Linq.Expressions;
using eXpand.ExpressApp.Logic.NodeUpdaters;
using eXpand.ExpressApp.MasterDetail.Logic;
using eXpand.ExpressApp.MasterDetail.Model;

namespace eXpand.ExpressApp.MasterDetail.NodeUpdaters {
    public class MasterDetailRulesNodeUpdater :
        LogicRulesNodeUpdater<IMasterDetailRule, IModelMasterDetailRule, IModelApplicationMasterDetail>
    {
        protected override void SetAttribute(IModelMasterDetailRule rule,
                                             IMasterDetailRule attribute)
        {
            rule.Attribute = attribute;
        }


        protected override Expression<Func<IModelApplicationMasterDetail, object>> ExecuteExpression()
        {
            return controls => controls.MasterDetail;
        }
        }
}