using System;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid.Columns;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule {
    public interface IModelColumnOptions : IModelColumnOptionsBase {
        IModelGridColumnOptions GridColumnOptions { get; set; }
    }

    public interface IModelGridColumnOptions : IModelNode {
        IModelGridColumnOptionsColumn OptionsColumn { get; set; }
        IModelGridColumnOptionsColumnFilter OptionsFilter { get; set; }
    }

    public interface IModelGridColumnOptionsColumn : IModelNode {
    }

    public interface IModelGridColumnOptionsColumnFilter : IModelNode {
    }

    public class GridColumnOptionsController : ColumnOptionsController<GridColumn, IModelGridColumnOptions>
    {
        protected override Func<PropertyInfo, bool> ControlPropertiesFilterPredicate() {
            return info => info.PropertyType == typeof(OptionsColumnFilter) || info.PropertyType == typeof(OptionsColumn);
        }

        public override Func<PropertyInfo, bool> DynamicPropertiesFilterPredicate() {
            return info => true;
        }
    }
}