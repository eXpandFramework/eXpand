using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Xpand.Utils.Linq {
    public class ExpressionConverter {
        public Expression Convert(Type type, LambdaExpression expression) {
            if (expression == null)
                return null;
            if (expression.NodeType == ExpressionType.Lambda) {
                var parameterExpressions = expression.Parameters.Select(parameterExpression => Expression.Parameter(type, parameterExpression.Name)).ToArray();
                var parameter = parameterExpressions[0];
                Expression transform = Convert(type, expression.Body, parameter);
                return Expression.Lambda(transform, parameterExpressions);
            }
            throw new NotImplementedException(expression.NodeType.ToString());
        }
        public Expression Convert(Type type, Expression expression, ParameterExpression parameterExpression) {
            switch (expression.NodeType) {
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                    return TransformCore(type, (BinaryExpression)expression, expression.NodeType, parameterExpression);
                case ExpressionType.NotEqual:
                case ExpressionType.Equal:
                    return TransformCore(type, (BinaryExpression)expression, expression.NodeType, parameterExpression);
                case ExpressionType.MemberAccess:
                    return MemberAccess(type, (MemberExpression)expression, parameterExpression);
                case ExpressionType.Constant:
                    return Constant((ConstantExpression)expression);
                case ExpressionType.Parameter:
                    return Expression.Parameter(type, ((ParameterExpression)expression).Name);
            }
            throw new NotImplementedException(expression.NodeType.ToString());
        }

        Expression Constant(ConstantExpression expression) {
            return expression;
        }

        MemberExpression MemberAccess(Type type, MemberExpression expression, ParameterExpression parameterExpression1) {
            var parameterExpression = parameterExpression1;
            MemberInfo memberInfo = type.GetMember(expression.Member.Name).SingleOrDefault();
            MemberExpression memberAccess = Expression.MakeMemberAccess(parameterExpression, memberInfo);
            return memberAccess;
        }

        BinaryExpression TransformCore(Type type, BinaryExpression expression, ExpressionType expressionType, ParameterExpression parameterExpression) {
            Expression left = Convert(type, expression.Left, parameterExpression);
            Expression right = GetRight(expressionType, expression.Right, type, parameterExpression);
            MethodInfo methodInfo = GetMethodInfo(expressionType);
            return (BinaryExpression)methodInfo.Invoke(null, new[] { left, right });
        }

        Expression GetRight(ExpressionType expressionType, Expression right, Type type, ParameterExpression parameterExpression) {
            if (expressionType == ExpressionType.AndAlso || expressionType == ExpressionType.OrElse)
                return Convert(type, right, parameterExpression);
            if (expressionType == ExpressionType.Equal || expressionType == ExpressionType.NotEqual)
                return right;
            throw new NotImplementedException(expressionType.ToString());
        }

        MethodInfo GetMethodInfo(ExpressionType expressionType) {
            return typeof(Expression).GetMethod(expressionType.ToString(), new[] { typeof(Expression), typeof(Expression) });
        }
    }
}