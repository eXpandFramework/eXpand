using System;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Decorators;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controller
{
    public partial class UpdateModelViewController : Controllers.UpdateModelViewController
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
            return typeof(WebHintPanelDecorator);
        }
    }
}
