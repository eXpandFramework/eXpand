using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraTreeList;
using Xpand.ExpressApp.Core.DynamicModel;
using Xpand.ExpressApp.SystemModule;
using DynamicDouplicateTypesMapper = Xpand.ExpressApp.Win.Core.DynamicDouplicateTypesMapper;

namespace Xpand.ExpressApp.TreeListEditors.Win.Controllers
{

    public interface IModelTreeViewOptionsBehavior : IModelNode
    {
    }

    public interface IModelTreeViewListOptionsLayout : IModelNode
    {
    }

    public interface IModelTreeViewListOptionsMenu : IModelNode
    {
    }

    public interface IModelTreeViewListOptionsPrint : IModelNode
    {
    }

    public interface IModelTreeViewListOptionsSelection : IModelNode
    {
    }

    public interface IModelTreeViewListOptionsView : IModelNode
    {
    }

    public interface IModelTreeViewMainOptions : IModelTreeViewOptionsBase
    {
        IModelTreeViewOptions TreeListOptions { get; set; }
    }

    public interface IModelTreeViewOptionsBase : IModelNode
    {
    }

    public interface IModelTreeViewOptions : IModelNode
    {
        string PreviewFieldName { get; set; }
        int? PreviewLineCount { get; set; }
        int? RowHeight { get; set; }
        IModelTreeViewOptionsBehavior OptionsBehavior { get; set; }
        IModelTreeViewListOptionsLayout OptionsLayout { get; set; }
        IModelTreeViewListOptionsMenu OptionsMenu { get; set; }
        IModelTreeViewListOptionsPrint OptionsPrint { get; set; }
        IModelTreeViewListOptionsSelection OptionsSelection { get; set; }
        IModelTreeViewListOptionsView OptionsView { get; set; }
    }

    public class TreeListOptionsController : OptionsController<IModelListView>
    {
        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes()
        {
            yield return new DynamicModelType(typeof(IModelTreeViewOptionsBehavior), typeof(TreeListOptionsBehavior));
            yield return new DynamicModelType(typeof(IModelTreeViewListOptionsLayout), typeof(OptionsLayoutTreeList));
            yield return new DynamicModelType(typeof(IModelTreeViewListOptionsMenu), typeof(TreeListOptionsMenu));
            yield return new DynamicModelType(typeof(IModelTreeViewListOptionsPrint), typeof(TreeListOptionsPrint));
            yield return new DynamicModelType(typeof(IModelTreeViewListOptionsSelection), typeof(TreeListOptionsSelection));
            yield return new DynamicModelType(typeof(IModelTreeViewListOptionsView), typeof(TreeListOptionsView), null, null, new DynamicDouplicateTypesMapper());
        }

        protected override Type GetExtenderType()
        {
            return typeof(IModelTreeViewMainOptions);
        }
    }
}