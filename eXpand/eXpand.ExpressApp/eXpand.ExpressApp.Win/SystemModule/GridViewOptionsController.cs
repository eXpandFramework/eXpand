using System;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
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

    public class GridViewOptionsController : GridOptionsController<GridView, IModelGridViewOptions>
    {
        protected override Func<PropertyInfo, bool> ControlPropertiesFilterPredicate()
        {
            return info => info.PropertyType.Name.StartsWith("GridOptions");
        }

        public override Func<PropertyInfo, bool> DynamicPropertiesFilterPredicate()
        {
            return info => info.PropertyType.Name != typeof(NewItemRowPosition).Name;
        }

    }

}