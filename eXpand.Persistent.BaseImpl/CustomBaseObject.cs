using System;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl
{
    [NonPersistent]
    [Obsolete("Use XFPBaseObject")]
    public abstract class CustomBaseObject : XFPBaseObject
    {
        protected CustomBaseObject(Session session) : base(session)
        {
        }
    }
}