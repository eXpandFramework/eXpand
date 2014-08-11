using System;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Xpo;

namespace Xpand.Persistent.Base.General {
    public static class CriteriaOperatorExtensions {
        public static bool Fit(this CriteriaOperator criteriaOperator, object targetObject) {
            if (ReferenceEquals(criteriaOperator, null))
                return true;
            var evaluator = NewExpressionEvaluator(criteriaOperator, targetObject);
            return evaluator.Fit(targetObject);
        }

        public static ExpressionEvaluator NewExpressionEvaluator(this CriteriaOperator criteriaOperator, object targetObject){
            var objectType = targetObject.GetType();
            var wrapper = new LocalizedCriteriaWrapper(objectType, criteriaOperator);
            wrapper.UpdateParametersValues(targetObject);
            var descriptor = GetEvaluatorContextDescriptor(objectType, targetObject);
            return new ExpressionEvaluator(descriptor, wrapper.CriteriaOperator,
                XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.CustomFunctionOperators);
        }

        private static EvaluatorContextDescriptor GetEvaluatorContextDescriptor(Type objectType, object targetObject) {
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(targetObject);
            return objectSpace != null? objectSpace.GetEvaluatorContextDescriptor(objectType)
                : new EvaluatorContextDescriptorDefault(objectType);
        }
    }
}
