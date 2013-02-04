using System;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo.MetaData;

namespace Xpand.Xpo {
    public static class IXPSimpleObjectExtensions {

        public static XpandCustomMemberInfo CreateCustomMember(this XPClassInfo classInfo, string propertyName, Type propertyType,
                                                               bool nonPersistent, params Attribute[] attributes) {
            var xpandCustomMemberInfo = new XpandCustomMemberInfo(classInfo, propertyName, propertyType, null, nonPersistent, false);
            foreach (Attribute attribute in attributes)
                xpandCustomMemberInfo.AddAttribute(attribute);
            return xpandCustomMemberInfo;
        }


        public static XpandCalcMemberInfo CreateCalculabeMember(this XPClassInfo classInfo, string propertyName, Type propertyType, string aliasExpression) {
            return new XpandCalcMemberInfo(classInfo, propertyName, propertyType, aliasExpression);
        }

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
