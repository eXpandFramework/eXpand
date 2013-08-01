using DevExpress.ExpressApp;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public class PessimisticLockingMessageAttribute : AdditionalViewControlsRuleAttribute {
        public PessimisticLockingMessageAttribute(string id)
            : base(id, PessimisticLockingViewController.LockedUser + "!='@CurrentUserID' AND " + PessimisticLockingViewController.LockedUser + " Is Not Null", "1=0", "Record is locked by user {0}", Position.Top) {
            MessageProperty = "LockedUserMessage";
        }
    }
    public class LockedUserMessageXpMemberInfo : XPCustomMemberInfo, ISupportCancelModification {
        string _theValue;

        public LockedUserMessageXpMemberInfo(XPClassInfo owner)
            : base(owner, "LockedUserMessage", typeof(string), null, true, false) {
        }
        public override object GetValue(object theObject) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(theObject.GetType());
            var memberValue = typeInfo.FindMember(PessimisticLockingViewController.LockedUser).GetValue(theObject);
            return _theValue != null ? (memberValue != null ? string.Format(_theValue, memberValue) : null) : null;
        }
        public override void SetValue(object theObject, object theValue) {
            _theValue = theValue as string;
            base.SetValue(theObject, theValue);
        }
    }

}
