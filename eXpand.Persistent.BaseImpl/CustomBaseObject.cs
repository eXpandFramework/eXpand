using System;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl
{
    [NonPersistent]
    [Obsolete("Use eXpandBaseObject")]
    public abstract class CustomBaseObject : eXpandBaseObject
    {
        protected CustomBaseObject(Session session) : base(session)
        {
        }
    }
}