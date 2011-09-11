using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid.Columns;
using Xpand.ExpressApp.Core.DynamicModel;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelColumnOptions : IModelColumnOptionsBase {
        IModelGridColumnOptions GridColumnOptions { get; }
    }

    public interface IModelGridColumnOptions : IModelNode {
        bool ShowUnboundExpressionMenu { get; set; }
        string UnboundExpression { get; set; }
        IModelGridColumnOptionsColumn OptionsColumn { get; }
        IModelGridColumnOptionsColumnFilter OptionsFilter { get; }
    }

    public interface IModelGridColumnOptionsColumn : IModelNode {
    }

    public interface IModelGridColumnOptionsColumnFilter : IModelNode {
    }

    public class GridColumnOptionsController : ColumnOptionsController {
        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelGridColumnOptionsColumn), typeof(OptionsColumn));
            yield return new DynamicModelType(typeof(IModelGridColumnOptionsColumnFilter), typeof(OptionsColumnFilter));
        }


    }
}