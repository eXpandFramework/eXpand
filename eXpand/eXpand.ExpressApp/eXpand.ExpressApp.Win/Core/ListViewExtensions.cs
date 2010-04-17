using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.Core
{
    public static class ListViewExtensions
    {
        public static CriteriaOperator GetTotalCriteria(this ListView listView)
        {
            listView.SynchronizeInfo();
            List<CriteriaOperator> operators = listView.CollectionSource.Criteria.GetValues();
            operators.Add(CriteriaOperator.Parse(((IModelListViewWin)listView.Model).ActiveFilterString));
            return ObjectSpace.CombineCriteria(operators.ToArray());
        }

        public static bool IsNested(this ListView listView, Frame frame)
        {
            return (frame.Template == null);
        }
    }
}
