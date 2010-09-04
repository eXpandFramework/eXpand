using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Xpand.ExpressApp.ConditionalActionState.Model;

namespace Xpand.ExpressApp.ConditionalActionState.DomainLogic
{
    [DomainLogic(typeof(IModelActionStateRule))]
    public static class ActionStateRuleDomainLogic{
        public static IEnumerable<string> Get_Actions(IModelActionStateRule modelActionStateRule) {
            return modelActionStateRule.Application.ActionDesign.Actions.Select(action => action.Id);
        }
    }
}
