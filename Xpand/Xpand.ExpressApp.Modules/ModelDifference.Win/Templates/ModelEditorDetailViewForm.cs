using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Templates;

namespace Xpand.ExpressApp.ModelDifference.Win.Templates {
    public class ModelEditorDetailViewForm : DetailViewForm {
        public Func<XafApplication> Application { get; set; }

        public override void ReloadSettings() {
            var list = new List<ModelApplicationBase>();
            var applicationBase = ((ModelApplicationBase)Application().Model);
            while (applicationBase.LastLayer.Id != "UserDiff" && applicationBase.LastLayer.Id != "AfterSetupLayerId") {
                var modelApplicationBase = applicationBase.LastLayer;
                list.Add(modelApplicationBase);
                applicationBase.RemoveLayer(modelApplicationBase);
            }
            base.ReloadSettings();
            foreach (var modelApplicationBase in list) {
                applicationBase.AddLayer(modelApplicationBase);
            }
        }
    }
}
