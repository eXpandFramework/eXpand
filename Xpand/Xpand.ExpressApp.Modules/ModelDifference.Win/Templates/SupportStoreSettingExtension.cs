using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;

namespace Xpand.ExpressApp.ModelDifference.Win.Templates{
    static class SupportStoreSettingExtension{
        public static void SetSettings(this ISupportStoreSettings supportStoreSettings,Action baseCall) {
            var list = new List<ModelApplicationBase>();
            var applicationBase = (ModelApplicationBase)CaptionHelper.ApplicationModel;
            while (applicationBase.LastLayer.Id != "UserDiff" && applicationBase.LastLayer.Id != "After Setup") {
                var modelApplicationBase = applicationBase.LastLayer;
                list.Add(modelApplicationBase);
                ModelApplicationHelper.RemoveLayer(modelApplicationBase);
            }
            baseCall();
            foreach (var modelApplicationBase in list) {
                ModelApplicationHelper.AddLayer(applicationBase, modelApplicationBase);
            }

        }
    }
}