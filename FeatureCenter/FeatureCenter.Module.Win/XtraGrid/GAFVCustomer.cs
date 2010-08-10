using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using FeatureCenter.Base;

namespace FeatureCenter.Module.Win.XtraGrid
{
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + BaseObjects.Captions.HeaderGuessAutoFilterRowValuesFromFilter, "1=1", "1=1",
            BaseObjects.Captions.ViewMessageGuessAutoFilterRowValuesFromFilter, Position.Bottom)]
    [AdditionalViewControlsRule(Captions.Header + " " + BaseObjects.Captions.HeaderGuessAutoFilterRowValuesFromFilter, "1=1", "1=1",
        BaseObjects.Captions.HeaderGuessAutoFilterRowValuesFromFilter, Position.Top)]
    public class GAFVCCustomer : CustomerBase
    {
        public GAFVCCustomer(Session session)
            : base(session)
        {
        }

    }
}
