using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.PivotChart.Web.Editors;

namespace Xpand.ExpressApp.PivotChart.Web.Options {
    public class PivotOptionsController : PivotChart.PivotOptionsController {
        SyncronizeInfo _syncronizeInfo;

        protected override Dictionary<Type, Type> GetActionChoiceItems() {
            return PivotGridOptionMapper.Instance.Dictionary;
        }

        protected override Type GetPersistentType(Type type) {
            return PivotGridOptionMapper.Instance[type];
        }
        protected override void Synchonize(XPClassInfo classInfo, Type optionType, object currentObject) {
            _syncronizeInfo = new SyncronizeInfo(classInfo, optionType, currentObject);
            base.Synchonize(classInfo, optionType, currentObject);
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            foreach (var analysisEditor in AnalysisEditors) {
                analysisEditor.ValueRead += (sender, args) => { Synchronize(); };
            }
            
        }

        void Synchronize() {
            if (_syncronizeInfo != null) {
                Synchonize(_syncronizeInfo.ClassInfo, _syncronizeInfo.Type, _syncronizeInfo.CurrentObject);
            }
            _syncronizeInfo = null;
        }

        protected override IEnumerable<object> GetGridOptionInstance(Type type) {
            var asPxPivotGrids = AnalysisEditors.Where(analysisEditor => analysisEditor.Control != null).Select(analysisEditor => ((AnalysisControlWeb)analysisEditor.Control).PivotGrid);
            return asPxPivotGrids.Select(pivotGridControl => {
                var propertyInfos = PropertyInfos(type);
                var single = propertyInfos.ToList().Select(info1 => info1.GetValue(pivotGridControl, null)).Single();
                return single;
            }).ToList();
        }

        class SyncronizeInfo {
            public SyncronizeInfo(XPClassInfo classInfo, Type type, object currentObject) {
                ClassInfo = classInfo;
                Type = type;
                CurrentObject = currentObject;
            }

            public XPClassInfo ClassInfo { get; private set; }

            public Type Type { get; private set; }

            public object CurrentObject { get; private set; }
        }

        IEnumerable<PropertyInfo> PropertyInfos(Type type) {
            return typeof(ASPxPivotGrid).GetProperties().Where(propertyInfo => propertyInfo.PropertyType == type);
        }
    }
}


