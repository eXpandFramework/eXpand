using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Demos;
using System.Collections.ObjectModel;

namespace DevExpress.ExpressApp.Demos {
    public interface IHintTemplate {
        string Hint { get; set; }
    }
    public class ShowHintController : ViewController {
        private HintCalculator calculator;
        public ShowHintController() {
            TargetViewNesting = Nesting.Root;
        }
        protected override void OnActivated() {
            base.OnActivated();
            Frame.TemplateChanged += new EventHandler(Frame_TemplateChanged);
            IHintProvider hintProvider = HintsRepository.Instance.FindHintProvider(View);
            if(hintProvider == null && View is ObjectView) {
                hintProvider = ((ObjectView)View).ObjectTypeInfo.FindAttribute<HintAttribute>();
            }
            calculator = new HintCalculator(hintProvider);
            if(View is DetailView) {
                calculator.CurrentViewType = ViewType.DetailView;
            }
            if(View is ListView) {
                calculator.CurrentViewType = ViewType.ListView;
            }
            calculator.HintChanged += new EventHandler<EventArgs>(calculator_HintChanged);
            View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
            calculator.CurrentObject = View.CurrentObject;
            SetText();
        }
        private void Frame_TemplateChanged(object sender, EventArgs e) {
            SetText();
        }
		protected override void OnDeactivated() {
            Frame.TemplateChanged -= new EventHandler(Frame_TemplateChanged);
            View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
            calculator.HintChanged -= new EventHandler<EventArgs>(calculator_HintChanged);
            calculator.Dispose();
            base.OnDeactivated();
        }
        private void View_CurrentObjectChanged(object sender, EventArgs e) {
            calculator.CurrentObject = View.CurrentObject;
        }
        private void calculator_HintChanged(object sender, EventArgs e) {
            SetText();
        }
        private void SetText() {
            if(Frame.Template is IHintTemplate) {
                ((IHintTemplate)Frame.Template).Hint = calculator.Hint;
            }
        }
    }

}
