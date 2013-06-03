using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using Xpand.ExpressApp.PivotChart.Core;

namespace Xpand.ExpressApp.PivotChart.ShowInAnalysis {
    public class InplaceAnalysisCacheController : WindowController {
        readonly LightDictionary<Type, List<object>> analysisCache = new LightDictionary<Type, List<object>>();
        ITypeInfoContainer _typeInfoContainer;

        public InplaceAnalysisCacheController() {
            TargetWindowType = WindowType.Main;
        }

        protected LightDictionary<Type, List<object>> AnalysisCache {
            get { return analysisCache; }
        }

        void Window_ViewChanging(object sender, EventArgs e) {
            ClearCache();
        }

        protected override void OnActivated() {
            base.OnActivated();
            Window.ViewChanging += Window_ViewChanging;
            
        }

        public void ClearCache() {
            analysisCache.Clear();
        }

        public virtual List<object> GetAnalysisDataList(Type targetObjectType) {
            List<object> cachedReports;
            if (analysisCache.TryGetValue(targetObjectType, out cachedReports)) {
                return cachedReports;
            }
            _typeInfoContainer = (ITypeInfoContainer)Application.Modules.Single(@base => @base is ITypeInfoContainer);
            using (var objectSpace = Application.CreateObjectSpace(_typeInfoContainer.TypesInfo.AnalysisType)) {
                List<string> targetObjectTypeNames = GetTargetObjectTypeNames(targetObjectType);
                var result = new List<object>();
                if (targetObjectTypeNames.Count > 0) {
                    IList reports = objectSpace.CreateCollection(_typeInfoContainer.TypesInfo.AnalysisType,
                                                                 new InOperator("ObjectTypeName", targetObjectTypeNames));
                    result.AddRange(reports.Cast<object>().Select(objectSpace.GetKeyValue));
                }
                analysisCache.Add(targetObjectType, result);
                return result;
            }
        }

        List<string> GetTargetObjectTypeNames(Type targetObjectType) {
            var targetObjectTypeNames = new List<string>();
            Type currentType = targetObjectType;
            while ((currentType != typeof (object)) && (currentType != null)) {
                if (XafTypesInfo.Instance.FindTypeInfo(currentType).IsPersistent) {
                    targetObjectTypeNames.Add(currentType.FullName);
                }
                currentType = currentType.BaseType;
            }
            return targetObjectTypeNames;
        }

        public static List<object> GetAnalysisDataList(XafApplication xafApplication, Type targetObjectType) {
            Guard.ArgumentNotNull(xafApplication, "xafApplication");
            if (xafApplication.MainWindow != null) {
                var cacheController = xafApplication.MainWindow.GetController<InplaceAnalysisCacheController>();
                if (cacheController != null) {
                    return cacheController.GetAnalysisDataList(targetObjectType);
                }
            }
            return new List<object>();
        }
    }
}