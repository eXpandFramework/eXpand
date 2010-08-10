using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using eXpand.Xpo;
using FeatureCenter.Base;

namespace FeatureCenter.Module.UpdateOnlyChangeFields {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderUpdateOnlyChangeFields, "1=1", "1=1",
        Captions.ViewMessageUpdateOnlyChangeFields, Position.Bottom, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderUpdateOnlyChangeFields, "1=1", "1=1",
        Captions.HeaderUpdateOnlyChangeFields, Position.Top, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule("UpdateOnlyChangeFields", "1=1", "1=1", null, Position.Bottom, MessageProperty = "ModificationStatements")]
    [eXpand.ExpressApp.Attributes.NavigationItem("Update Only Changed Fields", "UOCFCustomer_DetailView")]
    [DisplayFeatureModel("UOCFCustomer_DetailView", "UpdateOnlyChangeFields")]
    public class UOCFCustomer : CustomerBase,ISupportChangedMembers {
        readonly ChangedMemberCollector _changedMemberCollector;


        public UOCFCustomer(Session session)
            : base(session) {
            _changedMemberCollector = new ChangedMemberCollector(this);
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