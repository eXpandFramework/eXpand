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

    public class GridOptionsController : ExpressApp.SystemModule.GridOptionsController<ASPxGridView, IModelGridViewOptions, IModelListViewMainViewOptions, ASPxGridListEditor>
    {
        protected override Func<PropertyInfo, bool> ControlPropertiesFilterPredicate() {
            return info => info.PropertyType.Name.EndsWith("Settings");
        }
        protected override Func<PropertyInfo, bool> DynamicPropertiesFilterPredicate() {
            return info => info.PropertyType!=typeof(Unit);
        }
        protected override object GetControl() {
            return base.GetControl() as ASPxGridView;
        }
    }
}