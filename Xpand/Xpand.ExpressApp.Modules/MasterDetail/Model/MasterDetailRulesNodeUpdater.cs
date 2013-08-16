using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.MasterDetail.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.MasterDetail.Model {
    public class MasterDetailRulesNodeUpdater :LogicRulesNodeUpdater<IMasterDetailRule, IModelMasterDetailRule> {
        protected override void SetAttribute(IModelMasterDetailRule rule,IMasterDetailRule attribute) {
            rule.Attribute = attribute;
        }

        protected override Expression<Func<IModelApplication, IModelLogic>> ExecuteExpression() {
            return application => ((IModelApplicationMasterDetail) application).MasterDetail;
        }
    }
}