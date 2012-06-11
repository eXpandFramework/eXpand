using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace Xpand.Xpo {
    public class XpandUnitOfWork : UnitOfWork {
        public XpandUnitOfWork() {
        }

        public XpandUnitOfWork(XPDictionary dictionary)
            : base(dictionary) {
        }

        public XpandUnitOfWork(IDataLayer layer, params IDisposable[] disposeOnDisconnect)
            : base(layer, disposeOnDisconnect) {
        }

        public XpandUnitOfWork(IObjectLayer layer, params IDisposable[] disposeOnDisconnect)
            : base(layer, disposeOnDisconnect) {
        }

        protected override MemberInfoCollection GetPropertiesListForUpdateInsert(object theObject, bool isUpdate) {
            if (theObject is ISupportChangedMembers && !IsNewObject(theObject)) {
                XPClassInfo ci = GetClassInfo(theObject);
                var changedMembers = new MemberInfoCollection(ci);
                changedMembers.AddRange(base.GetPropertiesListForUpdateInsert(theObject, isUpdate).Where(m =>
                        m.HasAttribute(typeof(PersistentAttribute)) || m.IsKey || m is ServiceField ||
                        ((ISupportChangedMembers)theObject).ChangedMemberCollector.MemberInfoCollection.Contains(m)));
                return changedMembers;
            }

            return base.GetPropertiesListForUpdateInsert(theObject, isUpdate);
        }
    }
}