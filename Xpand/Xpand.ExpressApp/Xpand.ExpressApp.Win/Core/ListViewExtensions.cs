using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;

namespace Xpand.ExpressApp.Win.Core {
    public static class ListViewExtensions {
        public static CriteriaOperator GetTotalCriteria(this ListView xpandListView) {
            xpandListView.SaveModel();
            List<CriteriaOperator> operators = xpandListView.CollectionSource.Criteria.GetValues();
            operators.Add(CriteriaOperator.Parse(xpandListView.Model.Filter));
            return XPObjectSpace.CombineCriteria(operators.ToArray());
        }

        public static bool IsNested(this ListView xpandListView, Frame frame) {
            return (frame.Template == null);
        }
    }
}
