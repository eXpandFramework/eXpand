using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;
using Xpand.Xpo;

namespace FeatureCenter.Module.Miscellaneous.UpdateOnlyChangeFields {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderUpdateOnlyChangeFields, "1=1", "1=1",
        Captions.ViewMessageUpdateOnlyChangeFields, Position.Bottom, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderUpdateOnlyChangeFields, "1=1", "1=1",
        Captions.HeaderUpdateOnlyChangeFields, Position.Top, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule("UpdateOnlyChangeFields", "1=1", "1=1", null, Position.Bottom, MessageProperty = "ModificationStatements")]
    [XpandNavigationItem(Captions.Miscellaneous+"Update Only Changed Fields", "UOCFCustomer_DetailView")]
    [DisplayFeatureModel("UOCFCustomer_DetailView", "UpdateOnlyChangeFields")]
    public class UOCFCustomer : CustomerBase,ISupportChangedMembers, ISupportModificationStatements {
        ChangedMemberCollector _changedMemberCollector;


        public UOCFCustomer(Session session)
            : base(session) {
            _changedMemberCollector = _changedMemberCollector ?? new ChangedMemberCollector(this);
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            _changedMemberCollector = _changedMemberCollector ?? new ChangedMemberCollector(this);
        }
        protected override void OnSaved()
        {
            base.OnSaved();
            _changedMemberCollector.Collect();
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            _changedMemberCollector.Collect(propertyName);
        }
        [Browsable(false)]
        public ChangedMemberCollector ChangedMemberCollector
        {
            get { return _changedMemberCollector  ; }
        }

        [NonPersistent, Browsable(false)]
        public string ModificationStatements { get; set; }
    }
}