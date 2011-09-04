using System;
using DevExpress.ExpressApp;

namespace Xpand.Persistent.Base.General {
    public interface IXpandObjectSpace:IObjectSpace {
        object FindObject(Type objectType, DevExpress.Data.Filtering.CriteriaOperator criteria, bool inTransaction,
                          bool selectDeleted);
    }
}