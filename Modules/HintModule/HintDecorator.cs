using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp
{
    public abstract class HintDecorator : IDisposable
    {
        private HintCalculator calculator;
        private View view;
        private void View_CurrentObjectChanged(object sender, EventArgs e)
        {
            calculator.CurrentObject = view.CurrentObject;
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
        public HintDecorator(View view)
        {
            this.view = view;
            calculator = new HintCalculator(view.ObjectTypeInfo.FindAttribute<HintAttribute>());
            if (view is DetailView)
            {
                calculator.CurrentViewType = ViewType.DetailView;
            }
            if (view is ListView)
            {
                calculator.CurrentViewType = ViewType.ListView;
            }
            calculator.HintChanged += new EventHandler<EventArgs>(calculator_HintChanged);
            view.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
            calculator.CurrentObject = view.CurrentObject;
        }
        public void Dispose()
        {
            if (view != null)
            {
                view.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
                view = null;
            }
            if (calculator != null)
            {
                calculator.HintChanged -= new EventHandler<EventArgs>(calculator_HintChanged);
                calculator.Dispose();
                calculator = null;
            }
        }
    }
}
