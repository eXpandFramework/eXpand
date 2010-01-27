using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PivotChart;

namespace eXpand.Persistent.BaseImpl.PivotChart {
    [DefaultClassOptions]
    [NonPersistent]
    public class PivotOption : BaseObject, IPivotOption {
        string _name;
        PivotOptionView _pivotOptionView;

        public PivotOption(Session session) : base(session) {
        }

        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
        public PivotOptionView PivotOptionView
        {
            get { return _pivotOptionView; }
            set { SetPropertyValue("PivotOptionView", ref _pivotOptionView, value); }
        }

        IPivotOptionView IPivotOption.PivotOptionView {
            get { return _pivotOptionView; }
            set { _pivotOptionView=value as PivotOptionView; }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            PivotOptionView=new PivotOptionView(Session);
        }
    }
}