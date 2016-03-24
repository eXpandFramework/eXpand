using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General.Model;

namespace WizardUITester.Module.Win.FunctionalTests{
    [DefaultClassOptions]
    [CloneView(CloneViewType.DetailView, "WizardObject_Page1_DetailView")]
    [CloneView(CloneViewType.DetailView, "WizardObject_Page2_DetailView")]
    public class WizardObject : BaseObject{
        private int _page1;
        private string _page2;

        public WizardObject(Session session) : base(session){
        }

        [RuleValueComparison(ValueComparisonType.Equals, 1)]
        public int Page1{
            get { return _page1; }
            set { SetPropertyValue("Page1", ref _page1, value); }
        }
        [RuleRequiredField]
        public string Page2{
            get { return _page2; }
            set { SetPropertyValue("Page2", ref _page2, value); }
        }
    }
}