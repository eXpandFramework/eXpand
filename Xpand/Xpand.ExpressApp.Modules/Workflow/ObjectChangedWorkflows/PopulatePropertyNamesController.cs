using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Controllers;

namespace Xpand.ExpressApp.Workflow.ObjectChangedWorkflows {
    public class PopulatePropertyNamesController : PopulateController<ObjectChangedWorkflow> {

        protected override IEnumerable<string> RefreshingProperties() {
            return new[] { GetPropertyName(workflow => workflow.TargetObjectType) };
        }

        protected override string GetPredefinedValues(IModelMember wrapper) {
            IMemberInfo memberInfo = View.ObjectTypeInfo.FindMember(GetPropertyName(workflow => workflow.TargetObjectType));
            var value = memberInfo.GetValue(View.CurrentObject) as Type;
            return value != null? string.Join(";", Application.TypesInfo.FindTypeInfo(value).Members.Select(info => info.Name))
                       : string.Join(";", "");
        }

        protected override Expression<Func<ObjectChangedWorkflow, object>> GetPropertyName() {
            return workflow => workflow.PropertyName;
        }
    }
}
