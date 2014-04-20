using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Fasterflect;

namespace Xpand.Xpo.Filtering {
    public static class CriteriaOperatorExtensions {
        public static IEnumerable<CriteriaOperator> GetOperands(this CriteriaOperator criteriaOperator) {
            if (criteriaOperator is FunctionOperator)
                return (IEnumerable<CriteriaOperator>) criteriaOperator.GetPropertyValue("Operands");
            var binaryOperator = criteriaOperator as BinaryOperator;
            if (binaryOperator != null) 
                return new[]{binaryOperator.LeftOperand, binaryOperator.RightOperand};
            var unaryOperator = criteriaOperator as UnaryOperator;
            if (unaryOperator != null) 
                return unaryOperator.Operand.GetOperands();
            var betweenOperator = criteriaOperator as BetweenOperator;
            if (betweenOperator != null)
                return new[] { betweenOperator.TestExpression,betweenOperator.BeginExpression, betweenOperator.EndExpression };
            return Enumerable.Empty<CriteriaOperator>();
        }

        /// <summary>
        /// Defines an extension method to <see cref="String"/> that converts the string value into a <see cref="criteria"/> by calling its <see cref="CriteriaOperator.Parse(string,out DevExpress.Data.Filtering.OperandValue[])"/> method.
        /// </summary>
        /// <param name="criteria">A <see cref="String"/> value that represents the expression to convert</param>
        /// <param name="args">The values that are substituted into the expression in place of question mark characters. These parameters can be omitted. </param>
        /// <returns>A <see cref="CriteriaOperator"/> equivalent to the expression contained in criteria.</returns>
        public static CriteriaOperator ToCriteria(this string criteria, params object[] args) {
            return CriteriaOperator.Parse(criteria, args);
        }

        public static CriteriaOperator GetClassTypeFilter(this Type type, Session session, string path) {
            path = path.TrimEnd('.');
            XPClassInfo xpClassInfo = session.GetClassInfo(type);
            XPObjectType xpObjectType = session.GetObjectType(xpClassInfo);
            string propertyName = path + "." + XPObject.Fields.ObjectType.PropertyName;
            return
                new GroupOperator(GroupOperatorType.Or, new NullOperator(propertyName),
                                  new BinaryOperator(propertyName, xpObjectType));
        }

        public static CriteriaOperator GetClassTypeFilter(this Type type, Session session) {
            XPClassInfo xpClassInfo = session.GetClassInfo(type);
            XPObjectType xpObjectType = session.GetObjectType(xpClassInfo);

            return XPObject.Fields.ObjectType.IsNull() |
                   XPObject.Fields.ObjectType == new OperandValue(xpObjectType.Oid);
        }
        public static CriteriaOperator Parse(string propertyPath, CriteriaOperator criteriaOperator) {
            while (propertyPath.IndexOf(".", StringComparison.Ordinal) > -1) {
                propertyPath = propertyPath.Substring(0, propertyPath.IndexOf(".", StringComparison.Ordinal)) + "[" +
                               propertyPath.Substring(propertyPath.IndexOf(".", StringComparison.Ordinal) + 1) + "]";
            }
            for (int i = propertyPath.Length - 1; i > -1; i--)
                if (propertyPath[i] != ']') {
                    propertyPath = propertyPath.Substring(0, i + 1) + "[" + criteriaOperator.ToString() + "]" +
                                   new string(']', propertyPath.Length - i - 1);
                    break;
                }

            return CriteriaOperator.Parse(propertyPath);
        }
    }
}
