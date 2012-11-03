using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using XXXVideoRental.Module.Win.BusinessObjects;

namespace XXXVideoRental.Module.Win.Controllers {
    public class StatisticsTopCustomersController : ViewController<ListView> {
        const string TopCustomersPeriod = "TopCustomersPeriod";
        readonly SingleChoiceAction _topCustomersPeriodAction;
        const string ThisMonth = "This Month";
        const string LastMonth = "Last Month";
        const string Last12Months = "Last 12 Months";
        const string TopCustomersCount = "TopCustomersCount";

        public StatisticsTopCustomersController() {
            TargetViewId = ViewIdProvider.StatisticsTopCustomers;
            _topCustomersPeriodAction = new SingleChoiceAction(this, TopCustomersPeriod, PredefinedCategory.Filters);
            _topCustomersPeriodAction.Items.Add(new ChoiceActionItem(ThisMonth, ThisMonth));
            _topCustomersPeriodAction.Items.Add(new ChoiceActionItem(LastMonth, LastMonth));
            _topCustomersPeriodAction.Items.Add(new ChoiceActionItem(Last12Months, Last12Months));

            _topCustomersPeriodAction.Caption = "Period";
            _topCustomersPeriodAction.Execute += TopCustomersPeriodOnExecute;

            var topCustomersCountAction = new ParametrizedAction(this, TopCustomersCount, PredefinedCategory.Filters, typeof(int));
            topCustomersCountAction.Execute += TopCustomersCountActionOnExecute;
        }

        void TopCustomersPeriodOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            CriteriaOperator criteriaOperator = null;
            switch (_topCustomersPeriodAction.SelectedItem.Id) {
                case ThisMonth:
                    criteriaOperator = ThisMonthCriteriaOperator();
                    break;
                case LastMonth:
                    criteriaOperator = LastMonthCriteriaOperator();
                    break;
                case Last12Months:
                    criteriaOperator = new BinaryOperator("Date", DateTime.Now.AddYears(-1), BinaryOperatorType.GreaterOrEqual);
                    break;
            }
            View.CollectionSource.Criteria[TopCustomersPeriod] = criteriaOperator;
        }


        CriteriaOperator LastMonthCriteriaOperator() {
            var dt = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1);
            var binaryOperator = new BinaryOperator(new FunctionOperator(FunctionOperatorType.GetYear, new OperandProperty("Date")), dt.Year, BinaryOperatorType.Equal);
            var operands = new BinaryOperator(new FunctionOperator(FunctionOperatorType.GetMonth, new OperandProperty("Date")), dt.Month, BinaryOperatorType.Equal);
            return new GroupOperator(GroupOperatorType.And, binaryOperator, operands);
        }

        GroupOperator ThisMonthCriteriaOperator() {
            var binaryOperator = new BinaryOperator(new FunctionOperator(FunctionOperatorType.GetYear, new OperandProperty("Date")), DateTime.Now.Year, BinaryOperatorType.Equal);
            var operands = new BinaryOperator(new FunctionOperator(FunctionOperatorType.GetMonth, new OperandProperty("Date")), DateTime.Now.Month, BinaryOperatorType.Equal);
            return new GroupOperator(GroupOperatorType.And, binaryOperator, operands);
        }

        void TopCustomersCountActionOnExecute(object sender, ParametrizedActionExecuteEventArgs parametrizedActionExecuteEventArgs) {
            UpdateTopCustomersCount(parametrizedActionExecuteEventArgs.ParameterCurrentValue);
        }

        protected virtual void UpdateTopCustomersCount(object parameterCurrentValue) {
            var pivotGridControl = ((PivotGridListEditor)View.Editor).PivotGridControl;
            pivotGridControl.Fields.GetFieldByName("Field_Customer").TopValueCount = (int)parameterCurrentValue;
            pivotGridControl.RefreshData();
        }
    }

}
