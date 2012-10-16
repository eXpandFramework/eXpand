using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraCharts;
using DevExpress.XtraPivotGrid;
using Xpand.ExpressApp.PivotGrid.Win.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.PivotGrid.Win {
    [ModelAbstractClass]
    public interface IModelPivotTopObject : IModelOptionsPivotGrid {
        IModelTopObject TopObject { get; }
    }

    public interface IModelTopObject : IModelNode {
        bool Enabled { get; set; }
        string FieldName { get; set; }
        string FieldRowName { get; set; }
        string FieldDataName { get; set; }
    }

    public class TopObjectController : PivotGridControllerBase, IPivotCustomSummaryEvent, IDataSourceSelectionChanged, IPivotFieldSortEvent, IModelExtender {
        object _topObject;
        object _currentObject;

        public new IModelPivotTopObject Model {
            get { return (IModelPivotTopObject)base.Model.OptionsPivotGrid; }
        }

        #region Implementation of IDataSourceSelectionChanged
        IList _selectedObjects;

        IList IDataSourceSelectionChanged.SelectedObjects {
            get { return _selectedObjects; }
            set {
                _selectedObjects = value;
                _currentObject = _selectedObjects.Count > 0 ? _selectedObjects[0] : null;
                if (_currentObject != null) {
                    _currentObject = ObjectSpace.GetObject(_currentObject);
                }
                CalcFilter();
            }
        }
        #endregion
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Enabled()) {
                CalcFilter();
                PivotGridListEditor.ChartControl.BoundDataChanged += ChartControlOnBoundDataChanged;
            }
        }

        protected override void AttachToControlEvents() {

        }
        void ChartControlOnBoundDataChanged(object sender, EventArgs eventArgs) {
            StyleTopCustomerSeries();
        }

        void StyleTopCustomerSeries() {
            var compareSeries = GetTopCustomerSeries();
            if (compareSeries != null) {
                var view = (LineSeriesView)compareSeries.View;
                view.LineStyle.DashStyle = DashStyle.Dash;
                view.LineMarkerOptions.FillStyle.FillMode = FillMode.Solid;
            }
        }

        Series GetTopCustomerSeries() {
            return _topObject == null ? null : PivotGridListEditor.ChartControl.Series.Cast<Series>().FirstOrDefault(SeriesMatch);
        }

        bool SeriesMatch(Series series) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(_topObject.GetType());
            var member = typeInfo.FindMember(Model.TopObject.FieldName);
            return series.Name.Contains(string.Format("{0}", member.GetValue(_topObject)));
        }

        void CalcFilter() {
            if (PivotGridListEditor != null && PivotGridListEditor.PivotGridControl != null && Enabled()) {
                var pivotGrid = PivotGridListEditor.PivotGridControl;
                var fieldRow = pivotGrid.Fields[Model.TopObject.FieldRowName];
                var fieldData = pivotGrid.Fields[Model.TopObject.FieldDataName];
                decimal topCustomerRevenues = 0;
                foreach (var customer in fieldRow.GetUniqueValues()) {
                    var cellValue = pivotGrid.GetCellValue(null, new[] { customer }, fieldData);
                    decimal customerRevenues = Convert.ToDecimal(cellValue);
                    if (customerRevenues > topCustomerRevenues) {
                        _topObject = customer;
                        topCustomerRevenues = customerRevenues;
                    }
                }
                var customersToShow = new List<object> { _topObject };
                if (!ReferenceEquals(_topObject, _currentObject))
                    customersToShow.Add(_currentObject);
                fieldRow.FilterValues.ValuesIncluded = customersToShow.OfType<object>().ToArray();
                pivotGrid.BestFit();
                StyleTopCustomerSeries();
            }
        }

        bool Enabled() {
            var topObject = Model.TopObject;
            return topObject.Enabled && !string.IsNullOrEmpty(topObject.FieldName) &&
                   !string.IsNullOrEmpty(topObject.FieldDataName) && !string.IsNullOrEmpty(topObject.FieldRowName);
        }
        #region Implementation of IPivotCustomSummaryEvent
        void IPivotCustomSummaryEvent.Calculate(PivotGridCustomSummaryEventArgs e) {
            if (e.RowField == null) {
                decimal currentSum = 0, topSum = 0;
                PivotDrillDownDataSource ds = e.CreateDrillDownDataSource();
                for (int i = 0; i < ds.RowCount; i++) {
                    decimal val = Convert.ToDecimal(ds[i][Model.TopObject.FieldDataName]);
                    if (ReferenceEquals(ds[i][Model.TopObject.FieldRowName], _topObject))
                        topSum += val;
                    else
                        currentSum += val;
                }
                e.CustomValue = currentSum - topSum;
            } else
                e.CustomValue = e.SummaryValue.Summary;
        }
        #endregion
        #region Implementation of IPivotFieldSort
        void IPivotFieldSortEvent.Calculate(PivotGridCustomFieldSortEventArgs e) {
            int value1 = ReferenceEquals(e.Value1, _topObject) ? 1 : 0,
                value2 = ReferenceEquals(e.Value2, _topObject) ? 1 : 0;
            int res = Comparer<int>.Default.Compare(value1, value2);
            if (res != 0) {
                e.Result = res;
                e.Handled = true;
            }
        }
        #endregion
        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptionsPivotGrid, IModelPivotTopObject>();
        }
        #endregion
    }
}