using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Chart.Win;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Chart.Win {
    public class XpandChartListEditor : ChartListEditor, ISelectionCriteria {
        readonly List<object> _selectedObjects = new List<object>();

        public XpandChartListEditor(IModelListView model)
            : base(model) {

        }
        public override IList GetSelectedObjects() {
            return _selectedObjects;
        }

        #region Implementation of ICustomSelectedObjects
        void ISelectionCriteria.AddSelectedObjects(IEnumerable<object> objects) {
            _selectedObjects.Clear();
            _selectedObjects.AddRange(objects);
            OnSelectionChanged();
        }

        CriteriaOperator ISelectionCriteria.SelectionCriteria { get; set; }
        #endregion
    }

}
