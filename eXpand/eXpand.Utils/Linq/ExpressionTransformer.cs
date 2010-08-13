using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace eXpand.Utils.Linq {
    public class ExpressionTransformer {
        public Expression Transform(Type type, LambdaExpression expression) {
            if (expression== null)
                return null;
            if (expression.NodeType == ExpressionType.Lambda) {
                var parameterExpressions = expression.Parameters.Select(parameterExpression => Expression.Parameter(type, parameterExpression.Name)).ToArray();
                var parameter = parameterExpressions[0];
                Expression transform = Transform(type, expression.Body, parameter);
                return Expression.Lambda(transform, parameterExpressions);
            }
            throw new NotImplementedException(expression.NodeType.ToString());
        }
        public Expression Transform(Type type, Expression expression,ParameterExpression parameterExpression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                    return TransformCore(type, (BinaryExpression)expression, expression.NodeType,parameterExpression);
                case ExpressionType.NotEqual:
                case ExpressionType.Equal:
                    return TransformCore(type, (BinaryExpression)expression,expression.NodeType, parameterExpression);
                case ExpressionType.MemberAccess:
                    return MemberAccess(type, (MemberExpression)expression, parameterExpression);
                case ExpressionType.Constant:
                    return Constant((ConstantExpression)expression);
                case ExpressionType.Parameter:
                    return Expression.Parameter(type, ((ParameterExpression) expression).Name);
            }
            throw new NotImplementedException(expression.NodeType.ToString());
        }



        Expression Constant(ConstantExpression expression) {
            return expression;
        }

        MemberExpression MemberAccess(Type type, MemberExpression expression, ParameterExpression parameterExpression1) {
            var parameterExpression = parameterExpression1;
            MemberInfo memberInfo = type.GetMember(expression.Member.Name).Single();
            MemberExpression memberAccess = Expression.MakeMemberAccess(parameterExpression, memberInfo);
            return memberAccess;
        }

        BinaryExpression TransformCore(Type type, BinaryExpression expression, ExpressionType expressionType, ParameterExpression parameterExpression)
        {
            Expression left = Transform(type, expression.Left,parameterExpression);
            Expression right = Transform(type, expression.Right,parameterExpression);
            MethodInfo methodInfo = GetMethodInfo(expressionType);
            return (BinaryExpression)methodInfo.Invoke(null, new[] { left, right });
        }

        MethodInfo GetMethodInfo(ExpressionType expressionType) {
            return typeof(Expression).GetMethod(expressionType.ToString(),new[]{typeof(Expression),typeof(Expression)});
        }
    }
}