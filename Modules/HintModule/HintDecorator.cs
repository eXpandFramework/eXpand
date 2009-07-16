using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.HintModule
{
    public abstract class HintDecorator : IDisposable
    {
        private HintCalculator calculator;
        private View view;
        private void View_CurrentObjectChanged(object sender, EventArgs e)
        {
            if (view != null) calculator.CurrentObject = view.CurrentObject;
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

        protected HintDecorator(View view)
        {
            this.view = view;
            calculator = new HintCalculator(view.ObjectTypeInfo.FindAttribute<HintAttribute>());
            if (view is DetailView)
                calculator.CurrentViewType = ViewType.DetailView;
            if (view is ListView)
                calculator.CurrentViewType = ViewType.ListView;
            calculator.HintChanged += calculator_HintChanged;
            view.CurrentObjectChanged += View_CurrentObjectChanged;
            calculator.CurrentObject = view.CurrentObject;
        }
        public void Dispose()
        {
            if (view != null)
            {
                view.CurrentObjectChanged -= View_CurrentObjectChanged;
                view = null;
            }
            if (calculator != null)
            {
                calculator.HintChanged -= calculator_HintChanged;
                calculator.Dispose();
                calculator = null;
            }
        }
    }
}