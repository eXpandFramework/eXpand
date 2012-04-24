using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Templates;

namespace Xpand.ExpressApp.ModelDifference.Win.Templates {
    public class ModelEditorDetailViewForm : DetailViewForm {
        public Func<XafApplication> Application { get; set; }
        public override void SetSettings(IModelTemplate modelTemplate) {
            var list = new List<ModelApplicationBase>();
            var applicationBase = ((ModelApplicationBase)Application().Model);
            while (applicationBase.LastLayer.Id != "UserDiff" && applicationBase.LastLayer.Id != "After Setup") {
                var modelApplicationBase = applicationBase.LastLayer;
                list.Add(modelApplicationBase);
                ModelApplicationHelper.RemoveLayer(modelApplicationBase);
            }
            base.SetSettings(modelTemplate);
            foreach (var modelApplicationBase in list) {
                ModelApplicationHelper.AddLayer(applicationBase,modelApplicationBase);
            }
        }

    }
}
