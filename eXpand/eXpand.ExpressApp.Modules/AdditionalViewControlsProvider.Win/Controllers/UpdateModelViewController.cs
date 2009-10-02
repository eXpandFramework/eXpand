using System;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Decorators;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controllers
{
    public partial class UpdateModelViewController : AdditionalViewControlsProvider.Controllers.UpdateModelViewController
    {
        public UpdateModelViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override Type GetDefaultControlType()
        {
            return typeof(HintPanel);
        }

        protected override Type GetDefaultDecoratorType()
        {
            return typeof(WinHintPanelDecorator);
        }
    }
}
