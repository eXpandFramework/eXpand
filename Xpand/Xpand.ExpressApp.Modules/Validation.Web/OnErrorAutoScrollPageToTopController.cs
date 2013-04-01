using System;
using System.ComponentModel;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Validation;

namespace Xpand.ExpressApp.Validation.Web {
    public interface IModelValidationOnErrorAutoScrollPageToTop : IModelNode {
        [Category("eXpand")]
        bool OnErrorAutoScrollPageToTop { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelValidationOnErrorAutoScrollPageToTop), "Validation")]
    public interface IModelDetailViewValidationOnErrorAutoScrollPageToTop : IModelValidationOnErrorAutoScrollPageToTop {
        [Browsable(false)]
        [ModelValueCalculator("((DevExpress.ExpressApp.Validation.IModelApplicationValidation)Application).Validation")]
        IModelValidation Validation { get; }
    }

    public class OnErrorAutoScrollPageToTopController : ViewController<DetailView>, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            if (AutoScrollEnabled())
                Validator.RuleSet.ValidationCompleted += RuleSetOnValidationCompleted;
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (AutoScrollEnabled())
                Validator.RuleSet.ValidationCompleted -= RuleSetOnValidationCompleted;
        }

        bool AutoScrollEnabled() {
            return ((IModelDetailViewValidationOnErrorAutoScrollPageToTop)View.Model).OnErrorAutoScrollPageToTop;
        }

        void RuleSetOnValidationCompleted(object sender, ValidationCompletedEventArgs validationCompletedEventArgs) {
            if (validationCompletedEventArgs.Exception != null) {
                var scriptKey = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                const string script = "window.scrollTo(0, 0);";
                WebWindow.CurrentRequestWindow.RegisterClientScript(scriptKey, script);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelValidation, IModelValidationOnErrorAutoScrollPageToTop>();
            extenders.Add<IModelDetailView, IModelDetailViewValidationOnErrorAutoScrollPageToTop>();
        }
    }
}
