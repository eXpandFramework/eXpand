using System;
using System.ComponentModel;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Validation;
using System.Linq;

namespace Xpand.ExpressApp.Validation.Web {


    public interface IModelActionsOnErrorAutoScrollPageToTop : IModelNode {
        [Category("eXpand")]
        [DefaultValue(true)]
        bool OnExceptionAutoScrollPageToTop { get; set; }
    }

    public interface IModelValidationOnErrorAutoScrollPageToTop : IModelNode {
        [Category("eXpand")]
        [DefaultValue(true)]
        bool OnErrorAutoScrollPageToTop { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelValidationOnErrorAutoScrollPageToTop), "Validation")]
    public interface IModelDetailViewValidationOnErrorAutoScrollPageToTop : IModelValidationOnErrorAutoScrollPageToTop {
        [Browsable(false)]
        [ModelValueCalculator("((DevExpress.ExpressApp.Validation.IModelApplicationValidation)Application).Validation")]
        IModelValidation Validation { get; }
    }
    [ModelInterfaceImplementor(typeof(IModelActionsOnErrorAutoScrollPageToTop), "ActionDesign")]
    public interface IModelDetailViewOnActionExceptionAutoScrollPageToTop : IModelActionsOnErrorAutoScrollPageToTop {
        [Browsable(false)]
        [ModelValueCalculator("((DevExpress.ExpressApp.Model.IModelApplication) Application).ActionDesign")]
        IModelActionDesign ActionDesign { get; }
    }

    public class OnErrorAutoScrollPageToTopController : ViewController<DetailView>, IModelExtender {
        protected override void OnActivated() {
            base.OnActivated();
            if (ValidationScrollEnabled()) {
                Validator.RuleSet.ValidationCompleted += RuleSetOnValidationCompleted;
            }
            if (ActionExceptionScroll()) {
                foreach (var action in Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions)) {
                    action.HandleException += ActionOnHandleException;
                }

            }
        }

        bool ActionExceptionScroll() {
            return ((IModelDetailViewOnActionExceptionAutoScrollPageToTop)View.Model).OnExceptionAutoScrollPageToTop;
        }

        void ActionOnHandleException(object sender, HandleExceptionEventArgs handleExceptionEventArgs) {
            ScrollWindowToTop();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (ValidationScrollEnabled()) {
                if (Validator.RuleSet != null) Validator.RuleSet.ValidationCompleted -= RuleSetOnValidationCompleted;
            }
            if (ActionExceptionScroll()) {
                foreach (var action in Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions)) {
                    action.HandleException -= ActionOnHandleException;
                }
            }
        }

        bool ValidationScrollEnabled() {
            return ((IModelDetailViewValidationOnErrorAutoScrollPageToTop)View.Model).OnErrorAutoScrollPageToTop;
        }

        void RuleSetOnValidationCompleted(object sender, ValidationCompletedEventArgs validationCompletedEventArgs) {
            if (validationCompletedEventArgs.Exception != null) {
                ScrollWindowToTop();
            }
        }

        void ScrollWindowToTop() {
            var scriptKey = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            const string script = "window.scrollTo(0, 0);";
            WebWindow.CurrentRequestWindow.RegisterClientScript(scriptKey, script);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelValidation, IModelValidationOnErrorAutoScrollPageToTop>();
            extenders.Add<IModelActionDesign, IModelActionsOnErrorAutoScrollPageToTop>();
            extenders.Add<IModelDetailView, IModelDetailViewValidationOnErrorAutoScrollPageToTop>();
            extenders.Add<IModelDetailView, IModelDetailViewOnActionExceptionAutoScrollPageToTop>();
        }
    }
}
