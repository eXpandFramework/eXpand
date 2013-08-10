using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.ModelDifference.Web {
    public class PopulateAspectsController:PopulateController<ModelDifferenceObject> {
        protected override string GetPredefinedValues(IModelMember wrapper) {
            return ModelEditorHelper.GetAspectNames((ModelApplicationBase)Application.Model).Aggregate("", (s, s1) => s + (s1 + ";")).TrimEnd(';');
        }

        protected override Expression<Func<ModelDifferenceObject, object>> GetPropertyName() {
            return o => o.PreferredAspect;
        }
    }
}
