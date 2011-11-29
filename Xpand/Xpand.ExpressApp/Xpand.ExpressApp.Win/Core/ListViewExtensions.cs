using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.SystemModule;

namespace Xpand.ExpressApp.Win.Core {
    public static class ListViewExtensions {
        public static CriteriaOperator GetTotalCriteria(this XpandListView xpandListView) {
            xpandListView.SaveModel();
            List<CriteriaOperator> operators = xpandListView.CollectionSource.Criteria.GetValues();
            operators.Add(CriteriaOperator.Parse(((IModelListViewWin)xpandListView.Model).ActiveFilterString));
            return ObjectSpace.CombineCriteria(operators.ToArray());
        }

        public static bool IsNested(this XpandListView xpandListView, Frame frame) {
            return (frame.Template == null);
        }
    }
}
