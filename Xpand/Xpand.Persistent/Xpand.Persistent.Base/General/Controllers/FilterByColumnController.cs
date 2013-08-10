using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.Controllers {
    public class FilterByColumnController : ViewController<ListView> {
        readonly SimpleAction _cellFilterAction;
        static readonly string _actionActiveContext = typeof(FilterByColumnController).Name;

        public FilterByColumnController() {
            _cellFilterAction = new SimpleAction(this, "CellFilter", PredefinedCategory.ObjectsCreation);
            _cellFilterAction.Active[_actionActiveContext] = false;
            _cellFilterAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
        }

        public SimpleAction CellFilterAction {
            get { return _cellFilterAction; }
        }

        public void UpdateAction(bool cellFilter) {
            _cellFilterAction.Active[_actionActiveContext] = cellFilter;
        }

        public CriteriaOperator GetCriteria(IModelColumn modelColumn, object parameters, CriteriaOperator activeFilterCriteria) {
            var criteriaOperator = CriteriaOperator.Parse(modelColumn.PropertyName + "=?", parameters);
            return new GroupOperator(GroupOperatorType.And, criteriaOperator, activeFilterCriteria);
        }
    }
}