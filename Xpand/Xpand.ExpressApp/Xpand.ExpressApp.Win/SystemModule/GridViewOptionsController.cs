using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Core.DynamicModel;
using Xpand.ExpressApp.SystemModule;
using DynamicDouplicateTypesMapper = Xpand.ExpressApp.Win.Core.DynamicDouplicateTypesMapper;

namespace Xpand.ExpressApp.Win.SystemModule {
    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptionsPrint : IModelNode {
    }
    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptionsMenu : IModelNode {
    }
    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptionsView : IModelNode {
    }
    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptionsBehaviour : IModelNode {

    }
    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptionsSelection : IModelNode {
    }
    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptionsNavigation : IModelNode {
    }
    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptionsCustomization : IModelNode {
    }

    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptionsDetail : IModelNode {
    }
    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelListViewMainViewOptions : IModelListViewMainViewOptionsBase {
        IModelGridViewOptions GridViewOptions { get; }
    }
    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptions : IModelNode {
        IModelGridViewOptionsBehaviour OptionsBehavior { get; }
        IModelGridViewOptionsDetail OptionsDetail { get; }
        IModelGridViewOptionsCustomization OptionsCustomization { get; }
        IModelGridViewOptionsNavigation OptionsNavigation { get; }
        IModelGridViewOptionsSelection OptionsSelection { get; }
        IModelGridViewOptionsView OptionsView { get; }
        IModelGridViewOptionsMenu OptionsMenu { get; }
        IModelGridViewOptionsPrint OptionsPrint { get; }
        IModelGridViewOptionsHint OptionsHint { set; }
    }
    //    [Obsolete("Use IModelOptionsGridView", true)]
    public interface IModelGridViewOptionsHint : IModelNode {
    }
    [Obsolete("", true)]
    public class GridViewOptionsController : GridOptionsController {
        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelGridViewOptionsBehaviour), typeof(GridOptionsBehavior));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsDetail), typeof(GridOptionsDetail));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsCustomization), typeof(GridOptionsCustomization));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsNavigation), typeof(GridOptionsNavigation));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsSelection), typeof(GridOptionsSelection));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsView), typeof(GridOptionsView), null, null, new DynamicDouplicateTypesMapper());
            yield return new DynamicModelType(typeof(IModelGridViewOptionsMenu), typeof(GridOptionsMenu));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsPrint), typeof(GridOptionsPrint));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsHint), typeof(GridOptionsHint));
        }



    }

}