using System;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.PivotChart.Win.Core;

namespace eXpand.ExpressApp.PivotChart.Win.Options {
    public class PivotGridOptionMapper {
        static IValueManager<PivotGridOptionMapper> _instanceManager;
        readonly Dictionary<Type, Type> _dictionary = new Dictionary<Type, Type>();

        PivotGridOptionMapper() {
            TypesInfo instance = TypesInfo.Instance;
            _dictionary.Add(typeof (PivotGridOptionsBehavior), instance.PivotOptionsBehaviorType);
            _dictionary.Add(typeof (PivotGridOptionsChartDataSource), instance.PivotOptionsChartDataSourceType);
            _dictionary.Add(typeof (PivotGridOptionsCustomizationEx), instance.PivotOptionsCustomizationType);
            _dictionary.Add(typeof (PivotGridOptionsData), instance.PivotOptionsDataType);
            _dictionary.Add(typeof (PivotGridOptionsDataField), instance.PivotOptionsDataFieldType);
            _dictionary.Add(typeof (PivotGridOptionsFilterPopup), instance.PivotOptionsFilterPopupType);
            _dictionary.Add(typeof (PivotGridOptionsHint), instance.PivotOptionsHintType);
            _dictionary.Add(typeof (PivotGridOptionsMenu), instance.PivotOptionsMenuType);
            _dictionary.Add(typeof (PivotGridOptionsSelection), instance.PivotOptionsSelectionType);
            _dictionary.Add(typeof (PivotGridOptionsView), instance.PivotOptionsViewType);
        }

        public Dictionary<Type, Type> Dictionary {
            get { return _dictionary; }
        }

        public Type this[Type type] {
            get { return _dictionary[type]; }
        }


        public static PivotGridOptionMapper Instance {
            get {
                if (_instanceManager == null) {
                    _instanceManager = ValueManager.CreateValueManager<PivotGridOptionMapper>();
                }
                return _instanceManager.Value ?? (_instanceManager.Value = new PivotGridOptionMapper());
            }
        }
    }
}