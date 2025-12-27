using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelAdapter;
using CriteriaOperatorExtensions = Xpand.Xpo.Filtering.CriteriaOperatorExtensions;

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
                    if (CriteriaOperatorExtensions.ThirdPartyCustomFunctionOperators.All(op => op.Name != customFunctionOperator.Name))
                        throw new InvalidOperationException();
                }else if (registeredItem == null){
                    CriteriaOperator.RegisterCustomFunction(customFunctionOperator);
                }
            }
        }

        public static void ValidateAndCommitChanges(this Session session) {
            var unitOfWork = ((UnitOfWork)session);
            var objectSpace = new XPObjectSpace(XafTypesInfo.Instance, XpoTypesInfoHelper.GetXpoTypeInfoSource(), () => unitOfWork);
#pragma warning disable CS0618
            var result = Validator.GetService(objectSpace.ServiceProvider).ValidateAllTargets(objectSpace, session.GetObjectsToSave(), ContextIdentifier.Save);
#pragma warning restore CS0618
            if (result.ValidationOutcome == ValidationOutcome.Error)
                throw new Exception(result.GetFormattedErrorMessage());
            unitOfWork.CommitChanges();
        }

        public static IObjectSpace XPObjectSpace(this object xpObject){
            return DevExpress.ExpressApp.Xpo.XPObjectSpace.FindObjectSpaceByObject(xpObject);
        }
    }
}
