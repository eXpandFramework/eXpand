using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Web.ASPxGridView;
using Xpand.ExpressApp.Core.DynamicModel;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {

    public interface IModelListViewMainViewOptions : IModelListViewMainViewOptionsBase {
        IModelGridViewOptions GridViewOptions { get; }
    }
    public interface IModelGridViewOptions : IModelNode {
        bool? EnableCallBacks { get; set; }
        IModelGridViewSettings Settings { get; }
        IModelGridViewSettingsBehavior SettingsBehavior { get; }
        IModelGridViewSettingsPager SettingsPager { get; }
        IModelGridViewSettingsEditing SettingsEditing { get; }
        IModelGridViewSettingsText SettingsText { get; }
        IModelGridViewSettingsCookies SettingsCookies { get; set; }
        IModelGridViewSettingsCustomizationWindow SettingsCustomizationWindow { get; }
        IModelGridViewSettingsDetail SettingsDetail { get; }
        IModelGridViewSettingsLoadingPanel SettingsLoadingPanel { get; }
    }

    public interface IModelGridViewSettingsLoadingPanel : IModelNode {
    }

    public interface IModelGridViewSettingsDetail : IModelNode {
    }

    public interface IModelGridViewSettingsCustomizationWindow : IModelNode {
    }

    public interface IModelGridViewSettingsCookies : IModelNode {
    }

    public interface IModelGridViewSettingsText : IModelNode {
    }

    public interface IModelGridViewSettingsEditing : IModelNode {
    }

    public interface IModelGridViewSettingsPager : IModelNode {
    }

    public interface IModelGridViewSettingsBehavior : IModelNode {
    }

    public interface IModelGridViewSettings : IModelNode {
    }
    [Obsolete]
    public class GridOptionsController : ExpressApp.SystemModule.GridOptionsController {
        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelGridViewSettings), typeof(ASPxGridViewSettings));
            yield return new DynamicModelType(typeof(IModelGridViewSettingsBehavior), typeof(ASPxGridViewBehaviorSettings));
            yield return new DynamicModelType(typeof(IModelGridViewSettingsPager), typeof(ASPxGridViewPagerSettings));
            yield return new DynamicModelType(typeof(IModelGridViewSettingsEditing), typeof(ASPxGridViewEditingSettings));
            yield return new DynamicModelType(typeof(IModelGridViewSettingsText), typeof(ASPxGridViewTextSettings));
            yield return new DynamicModelType(typeof(IModelGridViewSettingsCookies), typeof(ASPxGridViewCookiesSettings));
            yield return new DynamicModelType(typeof(IModelGridViewSettingsCustomizationWindow), typeof(ASPxGridViewCustomizationWindowSettings));
            yield return new DynamicModelType(typeof(IModelGridViewSettingsDetail), typeof(ASPxGridViewDetailSettings));
            yield return new DynamicModelType(typeof(IModelGridViewSettingsLoadingPanel), typeof(ASPxGridViewLoadingPanelSettings));
        }


    }


}