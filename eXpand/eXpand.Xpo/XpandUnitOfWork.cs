using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace eXpand.Xpo
{
    public class XpandUnitOfWork : UnitOfWork
    {
        public XpandUnitOfWork()
        {
        }

        public XpandUnitOfWork(XPDictionary dictionary)
            : base(dictionary)
        {
        }

        public XpandUnitOfWork(IDataLayer layer, params IDisposable[] disposeOnDisconnect)
            : base(layer, disposeOnDisconnect)
        {
        }

        protected override MemberInfoCollection GetPropertiesListForUpdateInsert(object theObject, bool isUpdate)
        {
            if (theObject is ISupportChangedMembers && !this.IsNewObject(theObject))
            {
                XPClassInfo ci = GetClassInfo(theObject);
                MemberInfoCollection changedMembers = new MemberInfoCollection(ci);

                foreach (XPMemberInfo m in base.GetPropertiesListForUpdateInsert(theObject, isUpdate))
                {
                    //If it is a servicefield this is required (OptimisticLockingField, GCRecord etc)
                    if (m.HasAttribute(typeof(PersistentAttribute)) || m.IsKey || m is ServiceField || ((ISupportChangedMembers)theObject).ChangedMembers.Contains(m))
                    {
                        changedMembers.Add(m);
                    }
                }

                return changedMembers;
            }

            return base.GetPropertiesListForUpdateInsert(theObject, isUpdate);
        }
    }
}
