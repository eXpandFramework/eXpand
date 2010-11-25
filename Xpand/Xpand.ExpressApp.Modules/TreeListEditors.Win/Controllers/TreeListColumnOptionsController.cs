using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraTreeList.Columns;
using Xpand.ExpressApp.Core.DynamicModel;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.TreeListEditors.Win.Controllers {
    public interface IModelTreeViewColumnMainOptions : IModelTreeViewColumnOptionsBase {
        IModelTreeViewColumnOptions TreeListColumnOptions { get; }
    }

    public interface IModelTreeViewColumnOptionsBase : IModelNode {
    }

    public interface IModelTreeViewColumnOptions : IModelNode {
        IModelTreeViewOptionsColumn OptionsColumn { get; }
        FixedStyle? Fixed { get; set; }
    }

    public interface IModelTreeViewOptionsColumn : IModelNode {
    }

    public class TreeListColumnOptionsController : OptionsController<IModelColumn> {
        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelTreeViewOptionsColumn), typeof(TreeListOptionsColumn));
        }

        protected override System.Type GetExtenderType() {
            return typeof(IModelTreeViewColumnMainOptions);
        }
    }
}