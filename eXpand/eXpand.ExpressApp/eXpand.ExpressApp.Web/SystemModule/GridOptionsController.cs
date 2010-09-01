using System;
using System.Reflection;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxGridView;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.Web.Core;

namespace eXpand.ExpressApp.Web.SystemModule {

    public interface IModelListViewMainViewOptions : IModelListViewMainViewOptionsBase
    {
        IModelGridViewOptions GridViewOptions { get; set; }
    }
    public interface IModelGridViewOptions : IModelNode
    {
        IModelGridViewSettings Settings { get; set; }
        IModelGridViewSettingsBehavior SettingsBehavior { get; set; }
        IModelGridViewSettingsPager SettingsPager { get; set; }
        IModelGridViewSettingsEditing SettingsEditing { get; set; }
        IModelGridViewSettingsText SettingsText { get; set; }
        IModelGridViewSettingsCookies SettingsCookies { get; set; }
        IModelGridViewSettingsCustomizationWindow SettingsCustomizationWindow { get; set; }
        IModelGridViewSettingsDetail SettingsDetail { get; set; }
        IModelGridViewSettingsLoadingPanel SettingsLoadingPanel { get; set; }
    }

    public interface IModelGridViewSettingsLoadingPanel : IModelNode
    {
    }

    public interface IModelGridViewSettingsDetail : IModelNode
    {
    }

    public interface IModelGridViewSettingsCustomizationWindow : IModelNode
    {
    }

    public interface IModelGridViewSettingsCookies : IModelNode
    {
    }

    public interface IModelGridViewSettingsText : IModelNode
    {
    }

    public interface IModelGridViewSettingsEditing : IModelNode
    {
    }

    public interface IModelGridViewSettingsPager : IModelNode
    {
    }

    public interface IModelGridViewSettingsBehavior : IModelNode
    {
    }

    public interface IModelGridViewSettings:IModelNode {
    }

    public class GridOptionsController : GridOptionsController<ASPxGridView, IModelGridViewOptions>
    {
        protected override Func<PropertyInfo, bool> ControlPropertiesFilterPredicate() {
            return info => info.PropertyType.Name.EndsWith("Settings");
        }

        public override Func<PropertyInfo, bool> DynamicPropertiesFilterPredicate() {
            return info => info.PropertyType!=typeof(Unit);
        }
    }

    
}