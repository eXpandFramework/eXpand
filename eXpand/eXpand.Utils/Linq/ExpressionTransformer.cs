using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace eXpand.Utils.Linq {
    public class ExpressionTransformer {
        public Expression Transform(Type type, Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                    return TransformCore(type, (BinaryExpression)expression, expression.NodeType);
                case ExpressionType.Lambda:
                    return Lambda((LambdaExpression) expression, type);
                case ExpressionType.NotEqual:
                case ExpressionType.Equal:
                    return TransformCore(type, (BinaryExpression)expression,expression.NodeType);
                case ExpressionType.MemberAccess:
                    return MemberAccess(type, (MemberExpression)expression);
                case ExpressionType.Constant:
                    return Constant((ConstantExpression)expression);
                case ExpressionType.Parameter:
                    return Expression.Parameter(type, ((ParameterExpression) expression).Name);
            }
            throw new NotImplementedException(expression.NodeType.ToString());
        }

        Expression Lambda(LambdaExpression expression, Type type) {
            Expression body = Transform(type, expression.Body);
            return Expression.Lambda(body, expression.Parameters.Select(parameterExpression => Expression.Parameter(type, parameterExpression.Name)).ToArray());
        }


        Expression Constant(ConstantExpression expression) {
            return expression;
        }

        MemberExpression MemberAccess(Type type, MemberExpression expression) {
            var parameterExpression = (ParameterExpression) Transform(type, expression.Expression);
            MemberInfo memberInfo = type.GetMember(expression.Member.Name).Single();
            MemberExpression memberAccess = Expression.MakeMemberAccess(parameterExpression, memberInfo);
            return memberAccess;
        }

        BinaryExpression TransformCore(Type type, BinaryExpression expression, ExpressionType expressionType )
        {
            Expression left = Transform(type, expression.Left);
            Expression right = Transform(type, expression.Right);
            MethodInfo methodInfo = GetMethodInfo(expressionType);
            return (BinaryExpression)methodInfo.Invoke(null, new[] { left, right });
        }

        MethodInfo GetMethodInfo(ExpressionType expressionType) {
            return typeof(Expression).GetMethod(expressionType.ToString(),new[]{typeof(Expression),typeof(Expression)});
        }
    }
}