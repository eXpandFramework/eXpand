using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Core.DynamicModel;
using DynamicDouplicateTypesMapper = Xpand.ExpressApp.Win.Core.DynamicDouplicateTypesMapper;

namespace Xpand.ExpressApp.Win.SystemModule
{

    public interface IModelGridViewOptionsPrint : IModelNode
    {
    }

    public interface IModelGridViewOptionsMenu : IModelNode
    {
    }

    public interface IModelGridViewOptionsView : IModelNode
    {
    }
    public interface IModelGridViewOptionsBehaviour : IModelNode
    {

    }
    public interface IModelGridViewOptionsSelection : IModelNode
    {
    }

    public interface IModelGridViewOptionsNavigation : IModelNode
    {
    }

    public interface IModelGridViewOptionsCustomization : IModelNode
    {
    }

    
    public interface IModelGridViewOptionsDetail : IModelNode
    {
    }
    public interface IModelListViewMainViewOptions : IModelListViewMainViewOptionsBase
    {
        IModelGridViewOptions GridViewOptions { get; set; }
    }

    public interface IModelGridViewOptions : IModelNode
    {
        IModelGridViewOptionsBehaviour OptionsBehavior { get; set; }
        IModelGridViewOptionsDetail OptionsDetail { get; set; }
        IModelGridViewOptionsCustomization OptionsCustomization { get; set; }
        IModelGridViewOptionsNavigation OptionsNavigation { get; set; }
        IModelGridViewOptionsSelection OptionsSelection { get; set; }
        IModelGridViewOptionsView OptionsView { get; set; }
        IModelGridViewOptionsMenu OptionsMenu { get; set; }
        IModelGridViewOptionsPrint OptionsPrint { get; set; }
        IModelGridViewOptionsHint OptionsHint { set; get; }
    }

    public interface IModelGridViewOptionsHint:IModelNode {
    }

    public class GridViewOptionsController : GridOptionsController
    {
        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelGridViewOptionsBehaviour), typeof(GridOptionsBehavior));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsDetail), typeof(GridOptionsDetail));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsCustomization), typeof(GridOptionsCustomization));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsNavigation), typeof(GridOptionsNavigation));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsSelection), typeof(GridOptionsSelection));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsView), typeof(GridOptionsView), null, null,new DynamicDouplicateTypesMapper());
            yield return new DynamicModelType(typeof(IModelGridViewOptionsMenu), typeof(GridOptionsMenu));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsPrint), typeof(GridOptionsPrint));
            yield return new DynamicModelType(typeof(IModelGridViewOptionsHint), typeof(GridOptionsHint));
        }



    }

}