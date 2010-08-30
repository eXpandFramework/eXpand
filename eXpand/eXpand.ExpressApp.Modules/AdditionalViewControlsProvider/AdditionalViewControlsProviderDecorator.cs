using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider
{
    public abstract class AdditionalViewControlsProviderDecorator : IDisposable
    {
        private AdditionalViewControlsProviderCalculator calculator;
        
        private readonly object control;

        protected AdditionalViewControlsProviderDecorator()
        {
        }

        public object Control
        {
            get { return control; }
        }


        private void calculator_HintChanged(object sender, EventArgs e)
        {
            UpdateText();
        }
        protected void UpdateText()
        {
            SetText(calculator.Hint);
        }
        protected abstract void SetText(string text);

        protected AdditionalViewControlsProviderDecorator(View view,object control, IAdditionalViewControlsRule controlsRule)
        {
            this.control = control;
            calculator = new AdditionalViewControlsProviderCalculator(controlsRule,view.ObjectTypeInfo.Type);
            calculator.HintChanged += calculator_HintChanged;
            calculator.CurrentObject = view.CurrentObject;
        }

        public AdditionalViewControlsProviderCalculator Calculator
        {
            get { return calculator; }
        }

        public void Dispose()
        {
            if (calculator != null)
            {
                calculator.HintChanged -= calculator_HintChanged;
                calculator.Dispose();
                calculator = null;
            }
        }
    }
}