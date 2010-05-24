using System;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelOptionsLayout : IModelNode
    {
    }

    public interface IModelOptionsPrint : IModelNode
    {
    }

    public interface IModelOptionsMenu : IModelNode
    {
    }

    public interface IModelOptionsView : IModelNode
    {
    }
    public interface IModelGridOptionsBehaviour : IModelNode
    {

    }
    public interface IModelOptionsSelection : IModelNode
    {
    }

    public interface IModelOptionsNavigation : IModelNode
    {
    }

    public interface IModelOptionsCustomization : IModelNode
    {
    }

    
    public interface IModelGridOptionsDetail : IModelNode
    {
    }
    public interface IModelListViewMainViewOptions : IModelNode
    {
        IModelGridViewOptions GridViewOptions { get; set; }
    }

    public interface IModelGridViewOptions : IModelNode
    {
        IModelGridOptionsBehaviour OptionsBehavior { get; set; }
//                IModelGridOptionsDetail OptionsDetail { get; set; }
        IModelOptionsCustomization OptionsCustomization { get; set; }
        IModelOptionsNavigation OptionsNavigation { get; set; }
        IModelOptionsSelection OptionsSelection { get; set; }
        IModelOptionsView OptionsView { get; set; }
        IModelOptionsMenu OptionsMenu { get; set; }
        IModelOptionsPrint OptionsPrint { get; set; }
        IModelOptionsLayout OptionsLayout { get; set; }
//        IModelGridOptionsHint OptionsHint { set; get; }
    }

    public interface IModelGridOptionsHint:IModelNode {
    }

    public class GridOptionsController : GridOptionsController<GridView, IModelGridViewOptions, IModelListViewMainViewOptions>
    {
        protected override Func<PropertyInfo, bool> ControlPropertiesFilterPredicate() {
            return info => info.PropertyType.Name.StartsWith("GridOptions");
        }

        protected override Func<PropertyInfo, bool> DynamicPropertiesFilterPredicate() {
            return info => info.PropertyType.Name!=typeof(NewItemRowPosition).Name;
        }

        protected override object GetControl()
        {
            var control = (GridControl)base.GetControl();
            return control.MainView;
        }
    }
}