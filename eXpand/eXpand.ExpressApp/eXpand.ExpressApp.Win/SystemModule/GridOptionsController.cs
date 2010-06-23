using System;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public interface IModelGridViewOptionsLayout : IModelNode
    {
    }

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
    public interface IModelListViewMainViewOptions : IModelNode
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
        IModelGridViewOptionsLayout OptionsLayout { get; set; }
        IModelGridViewOptionsHint OptionsHint { set; get; }
    }

    public interface IModelGridViewOptionsHint:IModelNode {
    }

    public class GridOptionsController : GridOptionsController<GridView, IModelGridViewOptions, IModelListViewMainViewOptions,GridListEditor>{
        protected override Func<PropertyInfo, bool> ControlPropertiesFilterPredicate() {
            return info => info.PropertyType.Name.StartsWith("GridOptions");
        }

        protected override Func<PropertyInfo, bool> DynamicPropertiesFilterPredicate() {
            return info => info.PropertyType.Name!=typeof(NewItemRowPosition).Name;
        }

        protected override object GetControl() {
            var control = base.GetControl() as GridControl;
            if (control != null) return control.MainView;
            return null;
        }
    }
}