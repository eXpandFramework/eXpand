using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

using FeatureCenter.Base;
using Xpand.ExpressApp.Attributes;
using Xpand.Xpo;
using Xpand.Xpo.Converters.ValueConverters;

namespace FeatureCenter.Module.Miscellaneous.UpdateOnlyChangeFields {
    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderUpdateOnlyChangeFields, "1=1", "1=1",
        Captions.ViewMessageUpdateOnlyChangeFields, Position.Bottom, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderUpdateOnlyChangeFields, "1=1", "1=1",
        Captions.HeaderUpdateOnlyChangeFields, Position.Top, ViewType = ViewType.DetailView)]
    [AdditionalViewControlsRule("UpdateOnlyChangeFields", "1=1", "1=1", null, Position.Bottom, MessageProperty = "ModificationStatements")]
    [XpandNavigationItem(Captions.Miscellaneous + "Update Only Changed Fields", "UOCFCustomer_DetailView")]
    [DisplayFeatureModel("UOCFCustomer_DetailView", "UpdateOnlyChangeFields")]
    public class UOCFCustomer : CustomerBase, ISupportChangedMembers, ISupportModificationStatements {

        public UOCFCustomer(Session session)
            : base(session) {

        }
        protected override void OnSaved() {
            base.OnSaved();
            ChangedMemberCollector.CollectOnSave(this);
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            ChangedMemberCollector.Collect(this, propertyName);
        }

        [NonPersistent, Browsable(false)]
        public string ModificationStatements { get; set; }

        [ValueConverter(typeof(NullValueConverter)), Persistent, Browsable(false)]
        [Size(1)]
        public HashSet<string> ChangedProperties { get; set; }
    }
}