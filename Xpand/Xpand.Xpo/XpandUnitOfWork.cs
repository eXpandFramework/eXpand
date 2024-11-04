using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;

namespace Xpand.Xpo {
    public class XpandUnitOfWork : UnitOfWork {
        public XpandUnitOfWork() {
        }

        public XpandUnitOfWork(XPDictionary dictionary)
            : base(dictionary)
            => TrackPropertiesModifications = true;

        public XpandUnitOfWork(IDataLayer layer, params IDisposable[] disposeOnDisconnect)
            : base(layer, disposeOnDisconnect)
            => TrackPropertiesModifications = true;

        public XpandUnitOfWork(IServiceProvider serviceProvider) : base(serviceProvider) => TrackPropertiesModifications = true;

        public XpandUnitOfWork(IObjectLayer layer, params IDisposable[] disposeOnDisconnect)
            : base(layer, disposeOnDisconnect)
            => TrackPropertiesModifications = true;

        protected override MemberInfoCollection GetPropertiesListForUpdateInsert(object theObject, bool isUpdate,
            bool addDelayedReference){
            var defaultMembers = base.GetPropertiesListForUpdateInsert(theObject, isUpdate, addDelayedReference);
            if (TrackPropertiesModifications && isUpdate){
                var members = new MemberInfoCollection(GetClassInfo(theObject));
                foreach (var mi in base.GetPropertiesListForUpdateInsert(theObject, true, addDelayedReference))
                    if (mi is ServiceField || mi.GetModified(theObject))
                        members.Add(mi);
                return members;
            }

            return defaultMembers;
        }

    }
}