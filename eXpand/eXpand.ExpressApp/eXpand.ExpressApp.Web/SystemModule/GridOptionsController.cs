using System;
using System.Reflection;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;

namespace eXpand.ExpressApp.Web.SystemModule {

    public interface IModelListViewMainViewOptions : IModelNode
    {
        IModelGridViewOptions GridViewOptions { get; set; }
    }
    public interface IModelGridViewOptions : IModelNode
    {
        IModelSettings Settings { get; set; }
        IModelSettingsBehavior SettingsBehavior { get; set; }
        IModelSettingsPager SettingsPager { get; set; }
        IModelSettingsEditing SettingsEditing { get; set; }
        IModelSettingsText SettingsText { get; set; }
        IModelSettingsCookies SettingsCookies { get; set; }
        IModelSettingsCustomizationWindow SettingsCustomizationWindow { get; set; }
        IModelSettingsDetail SettingsDetail { get; set; }
        IModelSettingsLoadingPanel SettingsLoadingPanel { get; set; }
    }

    public interface IModelSettingsLoadingPanel : IModelNode
    {
    }

    public interface IModelSettingsDetail : IModelNode
    {
    }

    public interface IModelSettingsCustomizationWindow : IModelNode
    {
    }

    public interface IModelSettingsCookies : IModelNode
    {
    }

    public interface IModelSettingsText : IModelNode
    {
    }

    public interface IModelSettingsEditing : IModelNode
    {
    }

    public interface IModelSettingsPager : IModelNode
    {
    }

    public interface IModelSettingsBehavior : IModelNode
    {
    }

    public interface IModelSettings:IModelNode {
    }

    public class GridOptionsController : ExpressApp.SystemModule.GridOptionsController<ASPxGridView, IModelGridViewOptions, IModelListViewMainViewOptions, ASPxGridListEditor>
    {
        protected override Func<PropertyInfo, bool> ControlPropertiesFilterPredicate() {
            return info => info.PropertyType.Name.EndsWith("Settings");
        }
        protected override Func<PropertyInfo, bool> DynamicPropertiesFilterPredicate() {
            return info => info.PropertyType!=typeof(Unit);
        }
    }
}