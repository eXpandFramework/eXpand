using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Xpand.ExpressApp.ImportWizard.Core {

    public static class EnumerableExtension {
        private static readonly MethodInfo GetValueMethod =
            (from m in typeof(PropertyInfo).GetMethods()
             where m.Name == "GetValue" && !m.IsAbstract
             select m).First();

        private static readonly ConstantExpression NullObjectArrayExpression =
            Expression.Constant(null, typeof(object[]));

        public static IEnumerable Transpose<T>(this IEnumerable<T> source) {
            if (source == null)
                throw new ArgumentNullException("source");
            return TransposeCore(source);
        }

        private static Delegate CreateSelectorFunc<T>(IEnumerable<T> source) {
            T[] list = source.ToArray();
            DynamicProperty[] dynamicProperties =
                list.Select(i => new DynamicProperty(i.ToString(), typeof(object))).ToArray();

            Type transposedType = ClassFactory.Instance.GetDynamicClass(dynamicProperties);
            ParameterExpression propParam = Expression.Parameter(typeof(PropertyInfo), "prop");

            var bindings = new MemberBinding[list.Length];
            for (int i = 0; i < list.Length; i++) {
                MethodCallExpression getter =
                    Expression.Call(
                        propParam,
                        GetValueMethod,
                        Expression.Constant(list[i]),
                        NullObjectArrayExpression
                        );

                bindings[i] = Expression.Bind(transposedType.GetProperty(dynamicProperties[i].Name), getter);
            }

            LambdaExpression selector =
                Expression.Lambda(
                    Expression.MemberInit(
                        Expression.New(transposedType),
                        bindings),
                    propParam);

            return selector.Compile();
        }



        private static IEnumerable TransposeCore<T>(IEnumerable<T> source) {
            var properties = typeof(T).GetProperties().ToList();
            var selector = CreateSelectorFunc(source);

            return properties.Select(property => selector.DynamicInvoke(property));
        }

    }
}