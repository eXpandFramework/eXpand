using System;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Decorators;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web {
    public sealed partial class AdditionalViewControlsProviderAspNetModule : AdditionalViewControlsProviderModuleBase {
        public AdditionalViewControlsProviderAspNetModule() {
            InitializeComponent();
        }

        protected override Type GetDecoratorType() {
            return typeof(WebHintPanelDecorator);
        }

        protected override Type GetControlType() {
            return typeof(HintPanel);
        }

        protected override bool IsDefaultContext(ExecutionContext context) {
            return context == ExecutionContext.ViewControlAdding;
        }
    }
}