using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Logic;
using System.Linq;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.WorldCreator.System;
using Xpand.Persistent.Base.AdditionalViewControls;

namespace FeatureCenter.Module.Web {
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class FeatureCenterAspNetModule : FeatureCenterModuleBase {
        public FeatureCenterAspNetModule() {
            InitializeComponent();
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            return base.GetModuleUpdaters(objectSpace, versionFromDB).Where(updater => !(updater is WorldCreatorModuleUpdater));
        }


        protected override void AdditionalViewControlsModuleOnRulesCollected(object sender, EventArgs e) {
            foreach (var typeInfo in XafTypesInfo.Instance.PersistentTypes) {
                var additionalViewControlsRules = LogicRuleManager.Instance[typeInfo].OfType<IAdditionalViewControlsRule>();
                foreach (var additionalViewControlsRule in additionalViewControlsRules) {
                    if (additionalViewControlsRule.Id.StartsWith(Module.Captions.Header)) {
                        additionalViewControlsRule.FontStyle = FontStyle.Bold;
                        additionalViewControlsRule.FontSize = 18;
                    } else if (additionalViewControlsRule.Id.StartsWith(Module.Captions.ViewMessage))
                        additionalViewControlsRule.FontStyle = FontStyle.Italic;
                }
            }
        }

    }

}
