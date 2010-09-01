using System;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxGridView;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Web.Core;

namespace eXpand.ExpressApp.Web.SystemModule
{
    public interface IModelColumnOptions : IModelColumnOptionsBase
    {
        IModelGridColumnOptions GridColumnOptions { get; set; }
    }

    public interface IModelGridColumnOptions : IModelNode
    {
        IModelGridViewColumnSettings Settings { get; set; }
    }

    public interface IModelGridViewColumnSettings:IModelNode {
    }

    public class GridColumnOptionsController : ColumnOptionsController<GridViewDataColumn, IModelGridColumnOptions>
    {
        protected override Func<PropertyInfo, bool> ControlPropertiesFilterPredicate() {
            return info => info.PropertyType == typeof(GridViewDataColumnSettings);
        }

        public override Func<PropertyInfo, bool> DynamicPropertiesFilterPredicate() {
            return info => true;
        }
        protected override ExpressApp.Core.DynamicModel.DynamicDouplicateTypesMapper GetDouplicatesTypeMapper()
        {
            return new DynamicDouplicateTypesMapper();
        }

    }
}
