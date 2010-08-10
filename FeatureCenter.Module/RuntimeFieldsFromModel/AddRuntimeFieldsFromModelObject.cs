using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace FeatureCenter.Module.AddRuntimeFieldsFromModel {
//    [AdditionalViewControlsRule(Captions.ViewMessage + " " + Captions.HeaderRuntimeMemberFromModel, "1=1", "1=1", Captions.ViewMessageRuntimeMemberFromModel, Position.Bottom, ViewType = ViewType.ListView)]
//    [AdditionalViewControlsRule(Captions.Header + " " + Captions.HeaderRuntimeMemberFromModel, "1=1", "1=1", Captions.HeaderRuntimeMemberFromModel, Position.Top)]
    public class AddRuntimeFieldsFromModelObject:BaseObject
    {
        public AddRuntimeFieldsFromModelObject(Session session) : base(session) {
        }
    }
}