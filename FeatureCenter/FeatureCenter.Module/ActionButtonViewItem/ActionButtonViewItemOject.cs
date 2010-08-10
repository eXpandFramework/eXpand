using System;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace FeatureCenter.Module.ActionButtonViewItem {
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderActionButtonViewItem, "1=1", "1=1",
//        Captions.ViewMessageActionButtonViewItem, Position.Bottom)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderActionButtonViewItem, "1=1", "1=1",
//        Captions.HeaderActionButtonViewItem, Position.Top)]
//    [System.ComponentModel.DisplayName(Captions.HeaderWelcome)]
    public class ActionButtonViewItemOject : BaseObject {
        DateTime _date;

        public ActionButtonViewItemOject(Session session) : base(session) {
        }

        public DateTime Date {
            get { return _date; }
            set { SetPropertyValue("Date", ref _date, value); }
        }
    }
}