using System;
using System.Linq.Expressions;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.ExpressApp.MasterDetail.Model;

namespace Xpand.ExpressApp.MasterDetail.NodeUpdaters {
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