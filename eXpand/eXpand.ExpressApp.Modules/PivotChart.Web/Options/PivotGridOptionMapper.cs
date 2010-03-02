using System;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.PivotChart.Web.Core;

namespace eXpand.ExpressApp.PivotChart.Web.Options {
    public class PivotGridOptionMapper {
        static IValueManager<PivotGridOptionMapper> _instanceManager;
        readonly Dictionary<Type, Type> _dictionary = new Dictionary<Type, Type>();

        PivotGridOptionMapper() {
            TypesInfo instance = TypesInfo.Instance;

            _dictionary.Add(typeof(PivotGridWebOptionsChartDataSource), instance.PivotGridWebOptionsChartDataSourceType);
            _dictionary.Add(typeof(PivotGridWebOptionsCustomization), instance.PivotGridWebOptionsCustomizationType);
            _dictionary.Add(typeof(PivotGridWebOptionsLoadingPanel), instance.PivotGridWebOptionsLoadingPanelType);
            _dictionary.Add(typeof (PivotGridOptionsData), instance.PivotOptionsDataType);
            _dictionary.Add(typeof (PivotGridOptionsDataField), instance.PivotOptionsDataFieldType);
            _dictionary.Add(typeof(PivotGridOptionsOLAP), instance.PivotGridOptionsOLAPType);
            _dictionary.Add(typeof(PivotGridWebOptionsPager), instance.PivotGridWebOptionsPagerType);
            _dictionary.Add(typeof(PivotGridWebOptionsView), instance.PivotGridWebOptionsViewType);
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