using DevExpress.Xpo;

namespace Xpand.Xpo{
    public static class Extensions{
        public static bool IsOutdated(this XPBaseObject obj) {
            using (UnitOfWork uow = new UnitOfWork(obj.Session.DataLayer)) {
                XPBaseObject obj1 = (XPBaseObject)uow.GetObjectByKey(obj.GetType(), obj.Session.GetKeyValue(obj));
                return !Equals(obj1.ClassInfo.OptimisticLockField.GetValue(obj1), obj.ClassInfo.OptimisticLockField.GetValue(obj));
            }
        }
    }
}
