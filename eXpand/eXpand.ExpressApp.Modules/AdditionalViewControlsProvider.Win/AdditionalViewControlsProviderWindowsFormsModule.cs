using System;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Decorators;
using eXpand.ExpressApp.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win
{
    public sealed partial class AdditionalViewControlsProviderWindowsFormsModule : AdditionalViewControlsProviderModuleBase
    {
        public AdditionalViewControlsProviderWindowsFormsModule()
        {
            InitializeComponent();
        }


        protected override Type GetDecoratorType() {
            return typeof(WinHintPanelDecorator);
        }

        protected override Type GetControlType() {
            return typeof(HintPanel);
        }

        protected override bool IsDefaultContext(ExecutionContext context) {
            return context == ExecutionContext.TemplateViewChanged || context == ExecutionContext.CurrentObjectChanged || context == ExecutionContext.ObjectChanged;
        }
    }
}
