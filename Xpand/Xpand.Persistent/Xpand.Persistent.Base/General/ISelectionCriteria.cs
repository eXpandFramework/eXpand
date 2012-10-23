using System.Collections.Generic;
using DevExpress.Data.Filtering;

namespace Xpand.Persistent.Base.General {
    public interface ISelectionCriteria {
        void AddSelectedObjects(IEnumerable<object> objects);
        CriteriaOperator SelectionCriteria { get; set; }
    }
}