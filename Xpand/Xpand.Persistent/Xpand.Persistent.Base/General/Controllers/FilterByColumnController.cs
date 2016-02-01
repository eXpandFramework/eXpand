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

        class CriteriaContainsProcessor: CriteriaProcessorBase {
            private readonly CriteriaOperator _criteria;
            private bool _match;

            public CriteriaContainsProcessor(CriteriaOperator criteria){
                _criteria = criteria;
            }

            public bool Contains(CriteriaOperator criteriaOperator) {
                Process(criteriaOperator);
                return _match;
            }

            protected override void Process(BinaryOperator theOperator){
                if (!ReferenceEquals(_criteria, null) && !ReferenceEquals(theOperator, null) &&
                    (_criteria.ToString() == theOperator.ToString()))
                    _match = true;
                base.Process(theOperator);
            }
        }
        public CriteriaOperator GetCriteria(IModelColumn modelColumn, object parameters, CriteriaOperator activeFilterCriteria) {
            var binaryOperator = new BinaryOperator(modelColumn.PropertyName,parameters);
            var criteriaProcessor = new CriteriaContainsProcessor(activeFilterCriteria);
            if (!criteriaProcessor.Contains(binaryOperator)){
                if (ReferenceEquals(activeFilterCriteria,null))
                    return binaryOperator;
                return new GroupOperator(GroupOperatorType.And,activeFilterCriteria,binaryOperator);
            }
            return activeFilterCriteria;
        }
    }
}