namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Logic {
    public class PessimisticLockingMessageAttribute : AdditionalViewControlsRuleAttribute {

        public PessimisticLockingMessageAttribute(string id)
            : base(id, "LockedUser!='@CurrentUserID'", "1=0", null, Position.Top) {
            MessageProperty = "LockedUserMessage";
        }
    }
}
