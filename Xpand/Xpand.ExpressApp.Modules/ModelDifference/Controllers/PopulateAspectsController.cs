using System;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ModelDifference.Controllers {
    public class PopulateAspectsController:PopulateController<ModelDifferenceObject> {
        protected override string GetPredefinedValues(IModelMember wrapper){
            return string.Join(";", ModelEditorHelper.GetAspectNames((ModelApplicationBase) Application.Model));
        }

        protected override Expression<Func<ModelDifferenceObject, object>> GetPropertyName() {
            return o => o.PreferredAspect;
        }
    }
}
