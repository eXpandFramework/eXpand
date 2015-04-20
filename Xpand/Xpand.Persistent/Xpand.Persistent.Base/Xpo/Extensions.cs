using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.Persistent.Base.Xpo {
    public static class Extensions {
        public static IDataStore ConnectionProvider(this IDataLayer dataLayer, object obj){
            return dataLayer.ConnectionProvider(obj.GetType());
        }

        public static IDataStore ConnectionProvider(this IDataLayer dataLayer, Type type){
            var baseDataLayer = (dataLayer) as BaseDataLayer;
            if (baseDataLayer != null){
                var proxy = baseDataLayer.ConnectionProvider as MultiDataStoreProxy;
                return proxy != null ? proxy.DataStoreManager.GetConnectionProvider(type) : baseDataLayer.ConnectionProvider;
            }
            return null;
        }

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

        public static void ValidateAndCommitChanges(this Session session) {
            var unitOfWork = ((UnitOfWork)session);
            var objectSpace = new XPObjectSpace(XafTypesInfo.Instance, XpoTypesInfoHelper.GetXpoTypeInfoSource(), () => unitOfWork);
            var result = Validator.RuleSet.ValidateAllTargets(objectSpace, session.GetObjectsToSave(), ContextIdentifier.Save);
            if (result.ValidationOutcome == ValidationOutcome.Error)
                throw new Exception(result.GetFormattedErrorMessage());
            unitOfWork.CommitChanges();
        }

        public static IObjectSpace XPObjectSpace(this object xpObject){
            return DevExpress.ExpressApp.Xpo.XPObjectSpace.FindObjectSpaceByObject(xpObject);
        }
    }
}
