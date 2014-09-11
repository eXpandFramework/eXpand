using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;

namespace Xpand.Persistent.Base.General{
    public class XpandCollectionSource : CollectionSource{
        protected internal XpandCollectionSource(IObjectSpace objectSpace, ITypeInfo objectTypeInfo, CollectionSourceDataAccessMode dataAccessMode, bool isAsyncServerMode, CollectionSourceMode mode) : base(objectSpace, objectTypeInfo, dataAccessMode, isAsyncServerMode, mode){
        }

        protected internal XpandCollectionSource(IObjectSpace objectSpace, Type objectType, CollectionSourceDataAccessMode dataAccessMode, bool isAsyncServerMode, CollectionSourceMode mode) : base(objectSpace, objectType, dataAccessMode, isAsyncServerMode, mode){
        }

        protected internal XpandCollectionSource(IObjectSpace objectSpace, ITypeInfo objectTypeInfo, bool isServerMode, bool isAsyncServerMode, CollectionSourceMode mode) : base(objectSpace, objectTypeInfo, isServerMode, isAsyncServerMode, mode){
        }

        protected internal XpandCollectionSource(IObjectSpace objectSpace, Type objectType, bool isServerMode, bool isAsyncServerMode, CollectionSourceMode mode) : base(objectSpace, objectType, isServerMode, isAsyncServerMode, mode){
        }

        public XpandCollectionSource(IObjectSpace objectSpace, Type objectType, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode) : base(objectSpace, objectType, dataAccessMode, mode){
        }

        public XpandCollectionSource(IObjectSpace objectSpace, Type objectType, CollectionSourceDataAccessMode dataAccessMode) : base(objectSpace, objectType, dataAccessMode){
        }

        public XpandCollectionSource(IObjectSpace objectSpace, Type objectType, bool isServerMode, CollectionSourceMode mode) : base(objectSpace, objectType, isServerMode, mode){
        }

        public XpandCollectionSource(IObjectSpace objectSpace, Type objectType, bool isServerMode) : base(objectSpace, objectType, isServerMode){
        }

        public XpandCollectionSource(IObjectSpace objectSpace, Type objectType) : base(objectSpace, objectType){
        }

        protected override void ApplySorting(IList<SortProperty> sorting){
            var xpServerCollectionSource = OriginalCollection as XPServerCollectionSource;
            if (xpServerCollectionSource != null){
                var defaultSorting = BaseObjectSpace.ConvertSortingToString(sorting);
                if (xpServerCollectionSource.DefaultSorting!=defaultSorting)
                    base.ApplySorting(sorting);
            }
        }
    }
}