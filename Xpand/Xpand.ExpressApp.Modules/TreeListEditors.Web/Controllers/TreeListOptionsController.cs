using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Web.ASPxTreeList;
using Xpand.ExpressApp.Core.DynamicModel;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.TreeListEditors.Web.Controllers {

    public interface IModelTreeListSettings : IModelNode {
    }

    public interface IModelTreeListSettingsBehavior : IModelNode {
    }

    public interface IModelTreeListSettingsPager : IModelNode {
    }

    public interface IModelTreeListSettingsCustomizationWindow : IModelNode {
    }

    public interface IModelTreeListSettingsSelection : IModelNode {
    }

    public interface IModelTreeListSettingsCookies : IModelNode {
    }
    public interface IModelTreeListSettingsLoadingPanel : IModelNode {
    }
    public interface IModelTreeListSettingsEditing : IModelNode {
    }
    public interface IModelTreeListSettingsPopupEditForm : IModelNode {
    }
    public interface IModelTreeListSettingsText : IModelNode {
    }

    public interface IModelTreeViewMainSettings : IModelTreeViewOptionsBase {
        IModelTreeViewOptions TreeListSettings { get; }
    }
    [Obsolete]
    public class TreeListOptionsController : OptionsController {
        protected override List<ModelExtenderPair> GetModelExtenderPairs() {
            return new List<ModelExtenderPair> {
                                                   new ModelExtenderPair(typeof (IModelListView), typeof (IModelTreeViewMainSettings)),
                                                   new ModelExtenderPair(typeof (IModelRootNavigationItems), typeof (IModelTreeViewMainSettings))
                                               };
        }

        protected override IEnumerable<DynamicModelType> GetDynamicModelTypes() {
            yield return new DynamicModelType(typeof(IModelTreeListSettings), typeof(TreeListSettings));
            yield return new DynamicModelType(typeof(IModelTreeListSettingsBehavior), typeof(TreeListSettingsBehavior));
            yield return new DynamicModelType(typeof(IModelTreeListSettingsPager), typeof(TreeListSettingsPager));
            yield return new DynamicModelType(typeof(IModelTreeListSettingsCustomizationWindow), typeof(TreeListSettingsCustomizationWindow));
            yield return new DynamicModelType(typeof(IModelTreeListSettingsSelection), typeof(TreeListSettingsSelection));
            yield return new DynamicModelType(typeof(IModelTreeListSettingsCookies), typeof(TreeListSettingsCookies));
            yield return new DynamicModelType(typeof(IModelTreeListSettingsLoadingPanel), typeof(TreeListSettingsLoadingPanel));
            yield return new DynamicModelType(typeof(IModelTreeListSettingsEditing), typeof(TreeListSettingsEditing));
            yield return new DynamicModelType(typeof(IModelTreeListSettingsPopupEditForm), typeof(TreeListSettingsPopupEditForm));
            yield return new DynamicModelType(typeof(IModelTreeListSettingsText), typeof(TreeListSettingsText));
        }

    }

    public interface IModelTreeViewOptions : IModelNode {
        IModelTreeListSettings Settings { get; }
        IModelTreeListSettingsBehavior SettingsBehavior { get; }
        IModelTreeListSettingsPager SettingsPager { get; }
        IModelTreeListSettingsCustomizationWindow SettingsCustomizationWindow { get; }
        IModelTreeListSettingsSelection SettingsSelection { get; }
        IModelTreeListSettingsCookies SettingsCookies { get; }
    }
}