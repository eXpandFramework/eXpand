using System;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.AdditionalViewControls;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider{
    public abstract class AdditionalViewControlsProviderDecorator : IDisposable{
        protected AdditionalViewControlsProviderDecorator(){
        }

        protected AdditionalViewControlsProviderDecorator(View view, object control,
            IAdditionalViewControlsRule controlsRule){
            Control = control;
            Calculator = new AdditionalViewControlsProviderCalculator(controlsRule, view.ObjectTypeInfo.Type);
            Calculator.HintChanged += calculator_HintChanged;
            Calculator.CurrentObject = view.CurrentObject;
        }

        public object Control { get; }

        public AdditionalViewControlsProviderCalculator Calculator { get; private set; }

        public void Dispose(){
            if (Calculator != null){
                Calculator.HintChanged -= calculator_HintChanged;
                Calculator.Dispose();
                Calculator = null;
            }
        }


        private void calculator_HintChanged(object sender, EventArgs e){
            UpdateText();
        }

        protected void UpdateText(){
            SetText(Calculator.Hint);
        }

        protected abstract void SetText(string text);
    }
}