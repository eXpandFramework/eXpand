using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace Xpand.Xpo {
    public static class ExpressionExtensions {
        public static Expression Visit<T>(
            this Expression exp,
            Func<T, Expression> visitor) where T : Expression {
            return ExpressionVisitor<T>.Visit(exp, visitor);
        }

        public static TExp Visit<T, TExp>(
            this TExp exp,
            Func<T, Expression> visitor)
            where T : Expression
            where TExp : Expression {
            return (TExp)ExpressionVisitor<T>.Visit(exp, visitor);
        }

        public static Expression<TDelegate> Visit<T, TDelegate>(
            this Expression<TDelegate> exp,
            Func<T, Expression> visitor) where T : Expression {
            return ExpressionVisitor<T>.Visit(exp, visitor);
        }

        public static IQueryable<TSource> Visit<T, TSource>(
            this IQueryable<TSource> source,
            Func<T, Expression> visitor) where T : Expression {
            return source.Provider.CreateQuery<TSource>(ExpressionVisitor<T>.Visit(source.Expression, visitor));
        }
    }

    /// <summary>
    /// This class visits every Parameter expression in an expression tree and calls a delegate
    /// to optionally replace the parameter.  This is useful where two expression trees need to
    /// be merged (and they don't share the same ParameterExpressions).
    /// </summary>
    public class ExpressionVisitor<T> : ExpressionVisitor where T : Expression {
        readonly Func<T, Expression> _visitor;

        public ExpressionVisitor(Func<T, Expression> visitor) {
            _visitor = visitor;
        }

        public static Expression Visit(
            Expression exp,
            Func<T, Expression> visitor) {
            return new ExpressionVisitor<T>(visitor).Visit(exp);
        }

        public static Expression<TDelegate> Visit<TDelegate>(
            Expression<TDelegate> exp,
            Func<T, Expression> visitor) {
            return (Expression<TDelegate>)new ExpressionVisitor<T>(visitor).Visit(exp);
        }

        protected override Expression Visit(Expression exp) {
            if (exp is T && _visitor != null) exp = _visitor((T)exp);

            return base.Visit(exp);
        }
    }

    /// <summary>
    /// Expression visitor
    /// (from http://msdn.microsoft.com/en-us/library/bb882521.aspx)
    /// </summary>
    public abstract class ExpressionVisitor {
        protected virtual Expression Visit(Expression exp) {
            if (exp == null)
                return null;
            switch (exp.NodeType) {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    return VisitUnary((UnaryExpression)exp);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return VisitBinary((BinaryExpression)exp);
                case ExpressionType.TypeIs:
                    return VisitTypeIs((TypeBinaryExpression)exp);
                case ExpressionType.Conditional:
                    return VisitConditional((ConditionalExpression)exp);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)exp);
                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)exp);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)exp);
                case ExpressionType.Lambda:
                    return VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return VisitNew((NewExpression)exp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return VisitNewArray((NewArrayExpression)exp);
                case ExpressionType.Invoke:
                    return VisitInvocation((InvocationExpression)exp);
                case ExpressionType.MemberInit:
                    return VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.ListInit:
                    return VisitListInit((ListInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        protected virtual MemberBinding VisitBinding(MemberBinding binding) {
            switch (binding.BindingType) {
                case MemberBindingType.Assignment:
                    return VisitMemberAssignment((MemberAssignment)binding);
                case MemberBindingType.MemberBinding:
                    return VisitMemberMemberBinding((MemberMemberBinding)binding);
                case MemberBindingType.ListBinding:
                    return VisitMemberListBinding((MemberListBinding)binding);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
            }
        }

        protected virtual ElementInit VisitElementInitializer(ElementInit initializer) {
            ReadOnlyCollection<Expression> arguments = VisitExpressionList(initializer.Arguments);
            if (arguments != initializer.Arguments) {
                return Expression.ElementInit(initializer.AddMethod, arguments);
            }
            return initializer;
        }

        protected virtual Expression VisitUnary(UnaryExpression u) {
            Expression operand = Visit(u.Operand);
            if (operand != u.Operand) {
                return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
            }
            return u;
        }

        protected virtual Expression VisitBinary(BinaryExpression b) {
            Expression left = Visit(b.Left);
            Expression right = Visit(b.Right);
            Expression conversion = Visit(b.Conversion);
            if (left != b.Left || right != b.Right || conversion != b.Conversion) {
                return b.NodeType == ExpressionType.Coalesce
                           ? Expression.Coalesce(left, right, conversion as LambdaExpression)
                           : Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
            }
            return b;
        }

        protected virtual Expression VisitTypeIs(TypeBinaryExpression b) {
            Expression expr = Visit(b.Expression);
            if (expr != b.Expression) {
                return Expression.TypeIs(expr, b.TypeOperand);
            }
            return b;
        }

        protected virtual Expression VisitConstant(ConstantExpression c) {
            return c;
        }

        protected virtual Expression VisitConditional(ConditionalExpression c) {
            Expression test = Visit(c.Test);
            Expression ifTrue = Visit(c.IfTrue);
            Expression ifFalse = Visit(c.IfFalse);
            if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse) {
                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return c;
        }

        protected virtual Expression VisitParameter(ParameterExpression p) {
            return p;
        }

        protected virtual Expression VisitMemberAccess(MemberExpression m) {
            Expression exp = Visit(m.Expression);
            if (exp != m.Expression) {
                return Expression.MakeMemberAccess(exp, m.Member);
            }
            return m;
        }

        protected virtual Expression VisitMethodCall(MethodCallExpression m) {
            Expression obj = Visit(m.Object);
            IEnumerable<Expression> args = VisitExpressionList(m.Arguments);
            if (obj != m.Object || !Equals(args, m.Arguments)) {
                return Expression.Call(obj, m.Method, args);
            }
            return m;
        }

        protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original) {
            List<Expression> list = null;
            for (int i = 0, n = original.Count; i < n; i++) {
                Expression p = Visit(original[i]);
                if (list != null) {
                    list.Add(p);
                } else if (p != original[i]) {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++) {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }
            if (list != null) {
                return list.AsReadOnly();
            }
            return original;
        }

        protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment) {
            Expression e = Visit(assignment.Expression);
            if (e != assignment.Expression) {
                return Expression.Bind(assignment.Member, e);
            }
            return assignment;
        }

        protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding) {
            IEnumerable<MemberBinding> bindings = VisitBindingList(binding.Bindings);
            if (!Equals(bindings, binding.Bindings)) {
                return Expression.MemberBind(binding.Member, bindings);
            }
            return binding;
        }

        protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding) {
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(binding.Initializers);
            if (!Equals(initializers, binding.Initializers)) {
                return Expression.ListBind(binding.Member, initializers);
            }
            return binding;
        }

        protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original) {
            List<MemberBinding> list = null;
            for (int i = 0, n = original.Count; i < n; i++) {
                MemberBinding b = VisitBinding(original[i]);
                if (list != null) {
                    list.Add(b);
                } else if (b != original[i]) {
                    list = new List<MemberBinding>(n);
                    for (int j = 0; j < i; j++) {
                        list.Add(original[j]);
                    }
                    list.Add(b);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original) {
            List<ElementInit> list = null;
            for (int i = 0, n = original.Count; i < n; i++) {
                ElementInit init = VisitElementInitializer(original[i]);
                if (list != null) {
                    list.Add(init);
                } else if (init != original[i]) {
                    list = new List<ElementInit>(n);
                    for (int j = 0; j < i; j++) {
                        list.Add(original[j]);
                    }
                    list.Add(init);
                }
            }
            if (list != null)
                return list;
            return original;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda) {
            Expression body = Visit(lambda.Body);
            if (body != lambda.Body) {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }
            return lambda;
        }

        protected virtual NewExpression VisitNew(NewExpression nex) {
            IEnumerable<Expression> args = VisitExpressionList(nex.Arguments);
            if (!Equals(args, nex.Arguments)) {
                return Expression.New(nex.Constructor, args, nex.Members);
            }
            return nex;
        }

        protected virtual Expression VisitMemberInit(MemberInitExpression init) {
            NewExpression n = VisitNew(init.NewExpression);
            IEnumerable<MemberBinding> bindings = VisitBindingList(init.Bindings);
            if (n != init.NewExpression || !Equals(bindings, init.Bindings)) {
                return Expression.MemberInit(n, bindings);
            }
            return init;
        }

        protected virtual Expression VisitListInit(ListInitExpression init) {
            NewExpression n = VisitNew(init.NewExpression);
            IEnumerable<ElementInit> initializers = VisitElementInitializerList(init.Initializers);
            if (n != init.NewExpression || !Equals(initializers, init.Initializers)) {
                return Expression.ListInit(n, initializers);
            }
            return init;
        }

        protected virtual Expression VisitNewArray(NewArrayExpression na) {
            IEnumerable<Expression> exprs = VisitExpressionList(na.Expressions);
            if (!Equals(exprs, na.Expressions)) {
                Type elementType = na.Type.GetElementType();
                if (elementType != null) {
                    if (na.NodeType == ExpressionType.NewArrayInit) {
                        return Expression.NewArrayInit(elementType, exprs);
                    }
                    return Expression.NewArrayBounds(elementType, exprs);
                }
            }
            return na;
        }

        protected virtual Expression VisitInvocation(InvocationExpression iv) {
            IEnumerable<Expression> args = VisitExpressionList(iv.Arguments);
            Expression expr = Visit(iv.Expression);
            if (!Equals(args, iv.Arguments) || expr != iv.Expression) {
                return Expression.Invoke(expr, args);
            }
            return iv;
        }
    }
}
