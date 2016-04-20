using System;
using System.Linq.Expressions;

namespace Xpand.Utils.Linq {
    public class ExpressionConverter {
        public class Visitor : ExpressionVisitor {
            private readonly ParameterExpression _parameter;

            public Visitor(ParameterExpression parameter) {
                _parameter = parameter;
            }

            protected override Expression VisitParameter(ParameterExpression node) {
                return _parameter;
            }
        }

        public static Expression Tranform<TFrom>(Expression<Func<TFrom, bool>> expression, Type targeType) {
            ParameterExpression parameter = Expression.Parameter(targeType);
            Expression body = new Visitor(parameter).Visit(expression.Body);
            return Expression.Lambda(body, parameter);
        }
    }
}