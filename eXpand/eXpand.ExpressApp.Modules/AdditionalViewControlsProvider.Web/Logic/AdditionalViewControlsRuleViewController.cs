using System;
using System.Web.UI;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Logic {
    public class AdditionalViewControlsRuleViewController : AdditionalViewControlsProvider.Logic.AdditionalViewControlsRuleViewController{

        protected override void InitializeControl(object control, LogicRuleInfo<IAdditionalViewControlsRule> logicRuleInfo, AdditionalViewControlsProviderCalculator additionalViewControlsProviderCalculator, ExecutionContext executionContext)
        {
            throw new NotImplementedException();

//            ControlCollection collection = ((Control)viewSiteControl).Controls;
//            ((Control)control).Visible = true;
//            switch (additionalViewControlsRule.Rule.Position) {
//                case Position.Top:
//                    collection.AddAt(0, ((Control)control));
//                    break;
//                default:
//                    collection.Add(((Control)control));
//                    break;
//            }
        }
    }
}