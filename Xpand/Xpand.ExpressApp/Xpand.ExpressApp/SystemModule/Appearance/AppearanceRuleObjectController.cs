using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule.Appearance {
    public interface IModelOptionAppearanceRule{
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool EndUserCustomizeAppearanceRule { get; set; }
    }

    public class AppearanceRuleObjectController : ViewController,IModelExtender {
        IList<AppearanceRuleObject> _formattingRules;
        private DevExpress.ExpressApp.ConditionalAppearance.AppearanceController _appearanceController;

        private DevExpress.ExpressApp.ConditionalAppearance.AppearanceController AppearanceController {
            get{return _appearanceController =_appearanceController?? Frame.GetController<DevExpress.ExpressApp.ConditionalAppearance.AppearanceController>();}
        }

        private bool Enabled(){
            return ((IModelOptionAppearanceRule) Application.Model.Options).EndUserCustomizeAppearanceRule;
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (Enabled()){
                _formattingRules = ObjectSpace.GetObjects<AppearanceRuleObject>();
                if (_formattingRules != null && _formattingRules.Count > 0 && AppearanceController != null) {
                    AppearanceController.AppearanceBeginUpdate();
                    AppearanceController.CollectAppearanceRules += AppearanceController_CollectAppearanceRules;
                }
            }
        }

        void AppearanceController_CollectAppearanceRules(object sender, CollectAppearanceRulesEventArgs e) {
            e.AppearanceRules.AddRange(_formattingRules);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Enabled()&&AppearanceController != null)
                AppearanceController.AppearanceEndUpdate();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (AppearanceController != null) {
                AppearanceController.CollectAppearanceRules -= AppearanceController_CollectAppearanceRules;
                AppearanceController.AppearanceEndUpdate();
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelOptions,IModelOptionAppearanceRule>();
        }
    }
}
