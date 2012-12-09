using System;
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
    [Obsolete]
    public class TreeListColumnOptionsController : OptionsController {
        protected override List<ModelExtenderPair> GetModelExtenderPairs() {
            return new List<ModelExtenderPair> { new ModelExtenderPair(typeof(IModelColumn), typeof(IModelTreeViewColumnMainOptions)) };
        }

        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelTreeViewOptionsColumn), typeof(TreeListOptionsColumn));
        }

    }
}