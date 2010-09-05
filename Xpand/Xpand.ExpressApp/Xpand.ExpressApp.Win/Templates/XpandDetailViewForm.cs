using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Win.Templates;

namespace Xpand.ExpressApp.Win.Templates
{
    public class XpandDetailViewForm : DetailViewForm{
        public override void ReloadSettings(){
            var list = new List<ModelApplicationBase>();
            var applicationBase = ((ModelApplicationBase) XpandModuleBase.Application.Model);
            while (applicationBase.LastLayer.Id != "UserDiff" && applicationBase.LastLayer.Id != "AfterSetupLayerId"){
                var modelApplicationBase = applicationBase.LastLayer;
                list.Add(modelApplicationBase);
                applicationBase.RemoveLayer(modelApplicationBase);
            }
            base.ReloadSettings();
            foreach (var modelApplicationBase in list){
                applicationBase.AddLayer(modelApplicationBase);
            }
        }
    }
}
