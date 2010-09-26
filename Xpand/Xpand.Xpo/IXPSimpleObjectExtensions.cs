using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;

namespace Xpand.Xpo {
    public static class IXPSimpleObjectExtensions {
        public static bool IsUniqueConstrauntViolated<T>(this T prmValue, params string[] prmUniqueFields) where T : XPObject {
            var operands = new CriteriaOperatorCollection();
            Array.ForEach(prmUniqueFields, property => operands.Add(new BinaryOperator(new OperandProperty(property),
                new OperandValue(prmValue.ClassInfo.GetMember(property).GetValue(prmValue)), BinaryOperatorType.Equal)));
            CriteriaOperator keyCondition = new NotOperator(new BinaryOperator(prmValue.ClassInfo.KeyProperty.Name,
            prmValue.ClassInfo.KeyProperty.GetValue(prmValue)));
            return prmValue.Session.FindObject<T>(PersistentCriteriaEvaluationBehavior.InTransaction,
                new GroupOperator(new GroupOperator(GroupOperatorType.And, operands), keyCondition)) != null;
        }

    }
}
