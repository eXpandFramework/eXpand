using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.Persistent.Base.Xpo {
    public static class Extensions {
        public static void Register(this ICustomFunctionOperator customFunctionOperator) {
            if (!(XafTypesInfo.Instance is TypesInfoBuilder.TypesInfo)) {
                ICustomFunctionOperator registeredItem = CriteriaOperator.GetCustomFunction(customFunctionOperator.Name);
                if (registeredItem != null && registeredItem != customFunctionOperator && InterfaceBuilder.RuntimeMode){
                    throw new InvalidOperationException();
                }
                if (registeredItem == null){
                    CriteriaOperator.RegisterCustomFunction(customFunctionOperator);
                }
            }
        }

        public static IObjectSpace XPObjectSpace(this object xpObject){
            return DevExpress.ExpressApp.Xpo.XPObjectSpace.FindObjectSpaceByObject(xpObject);
        }
    }
}
