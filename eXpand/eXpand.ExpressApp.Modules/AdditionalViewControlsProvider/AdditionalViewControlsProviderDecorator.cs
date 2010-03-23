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

//        private void View_CurrentObjectChanged(object sender, EventArgs e)
//        {
//            if (view != null) calculator.CurrentObject = view.CurrentObject;
//        }

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
//            if (view is DetailView)
//                calculator.CurrentViewType = ViewType.DetailView;
//            if (view is ListView)
//                calculator.CurrentViewType = ViewType.ListView;
            calculator.HintChanged += calculator_HintChanged;
//            view.CurrentObjectChanged += View_CurrentObjectChanged;
            calculator.CurrentObject = view.CurrentObject;
        }

        public AdditionalViewControlsProviderCalculator Calculator
        {
            get { return calculator; }
        }

        public void Dispose()
        {
//            if (view != null)
//            {
//                view.CurrentObjectChanged -= View_CurrentObjectChanged;
//                view = null;
//            }
            if (calculator != null)
            {
                calculator.HintChanged -= calculator_HintChanged;
                calculator.Dispose();
                calculator = null;
            }
        }
    }
}