using System;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Xpo;

namespace Xpand.ExpressApp.Core {
    public static class CriteriaOperatorExtensions {
        public static bool Fit(this CriteriaOperator criteriaOperator, object targetObject) {
            if (ReferenceEquals(criteriaOperator, null))
                return true;
            Type objectType = targetObject.GetType();
            var wrapper = new LocalizedCriteriaWrapper(objectType, criteriaOperator);
            wrapper.UpdateParametersValues(targetObject);
            var objectSpace = XPObjectSpace.FindObjectSpaceByObject(targetObject);
            EvaluatorContextDescriptor descriptor = objectSpace != null ? objectSpace.GetEvaluatorContextDescriptor(objectType) : new EvaluatorContextDescriptorDefault(objectType);
            var evaluator = new ExpressionEvaluator(descriptor, wrapper.CriteriaOperator, XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.CustomFunctionOperators);
            return evaluator.Fit(targetObject);
        }

    }
}
