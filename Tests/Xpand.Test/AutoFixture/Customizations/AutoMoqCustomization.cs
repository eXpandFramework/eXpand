using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using DevExpress.Xpo;
using Fasterflect;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;
using MethodInvoker = Ploeh.AutoFixture.Kernel.MethodInvoker;

namespace Xpand.Test.AutoFixture.Customizations{
    public class AutoMoqCustomization : ICustomization {
        private ISpecimenBuilder _relay;

        public AutoMoqCustomization():this(new MockRelay()){
        }

        public AutoMoqCustomization(ISpecimenBuilder relay){
            _relay = relay;
        }

        public ISpecimenBuilder Relay => _relay;

        public void Customize(IFixture fixture) {
            fixture.Customizations.Add(
                new MemoizingBuilder(new PropertiesPostprocessor(new MockPostprocessor(
                        new MethodInvoker(new MockConstructorQuery())))));
            _relay = new MockRelay(new IsMockableSpecification());
            fixture.ResidueCollectors.Add(_relay);
        }
    }

    class IsMockableSpecification : IRequestSpecification {
        public bool IsSatisfiedBy(object request){
            var t = request as Type;
            return t != null && ((t.IsAbstract) || (t.IsInterface)) &&
                   (t.IsInterface || !typeof(IXPObject).IsAssignableFrom(t));
        }
    }

    public class PropertiesPostprocessor : ISpecimenBuilder {
        private readonly ISpecimenBuilder _builder;

        public PropertiesPostprocessor(ISpecimenBuilder builder) {
            _builder = builder;
        }
        public static bool CanOverride(MethodBase method) =>
            ((method.IsVirtual && !method.IsFinal) && !method.IsPrivate);

        public bool CanOverrideGet(PropertyInfo property) {
            if (property.CanRead) {
                MethodInfo getMethod = property.GetGetMethod(true);
                return ((getMethod != null) && CanOverride(getMethod));
            }
            return false;
        }

        public bool CanOverrideSet(PropertyInfo property) {
            if (property.CanWrite) {
                MethodInfo setMethod = property.GetSetMethod(true);
                return ((setMethod != null) && CanOverride(setMethod));
            }
            return false;
        }
        private Expression GetPropertyExpression(Type mockType, PropertyInfo property) {
            ParameterExpression expression = Expression.Parameter(mockType, "m");
            ParameterExpression[] parameters = { expression };
            return Expression.Lambda(Expression.MakeMemberAccess(expression, property), parameters);
        }

        public object Create(object request, ISpecimenContext context) {
            object s = _builder.Create(request, context);
            if (s is NoSpecimen)
                return s;
//            var mock = (Mock)s;
//            var objectSpace = context.Resolve(typeof(IObjectSpace));

//            var mockType = mock.GetType().GetGenericArguments().Single();
//            if (typeof(ITestData).IsAssignableFrom(mockType))
//                SetupProperties(mock, context);
            return s;
        }

        private void SetupProperties(Mock mock, ISpecimenContext context) {
            var mockType = mock.GetType().GetGenericArguments().Single();
            var setupPropertyMethod = mock.GetType().GetMethods()
                .First(m => m.Name == "SetupProperty" && m.GetParameters().Length == 2);
            var setupGetMethod = mock.GetType().GetMethods()
                .First(m => m.Name == "SetupGet" && m.GetParameters().Length == 1);
            var propertyInfos =GetPropertyInfos(mockType)
                .Select(info => new{info, Speciment= context.Resolve(info.PropertyType) })
                .Where(property => !(property.Speciment is NoSpecimen)&&!(property.Speciment is Mock));
            foreach (var property in propertyInfos) {
                var expression = GetPropertyExpression(mockType, property.info);
//                var initialValue =
//                    mock.GetPropertyValue("DefaultValueProvider").CallMethod("ProvideDefault", property.info.GetGetMethod());

//                var mocked = initialValue as IMocked;
//                if (mocked != null) {
//                    SetupAllProperties(mocked.Mock, property.Speciment);
//                }
                SetupProperty(mock, property.info, setupPropertyMethod, expression, setupGetMethod, property.Speciment);
            }
        }

        private bool CanResolved(ISpecimenContext context, PropertyInfo info){
            var resolve = context.Resolve(info.PropertyType);
            return !(resolve is NoSpecimen)&&!(resolve is OmitSpecimen);
        }

        private void SetupAllProperties(Mock mock,object request){
            var mockType = mock.GetType().GetGenericArguments().Single();
            var setupPropertyMethod = mock.GetType().GetMethods()
                .First(m => m.Name == "SetupProperty" && m.GetParameters().Length == 2);
            var setupGetMethod = mock.GetType().GetMethods()
                .First(m => m.Name == "SetupGet" && m.GetParameters().Length == 1);
            var propertyInfos = GetPropertyInfos(mockType).Where(info => info.PropertyType.IsInstanceOfType(request));
            foreach (var property in propertyInfos) {
                var expression = GetPropertyExpression(mockType, property);
                var initialValue =
                    mock.GetPropertyValue("DefaultValueProvider").CallMethod("ProvideDefault", property.GetGetMethod());

                var mocked = initialValue as IMocked;
                if (mocked != null) {
                    SetupAllProperties(mocked.Mock, request);
                }
                SetupProperty(mock, property, setupPropertyMethod, expression, setupGetMethod,request);
            }
        }
//        private void SetupAllProperties(Mock mock,object request){
//            var mockType = mock.GetType().GetGenericArguments().Single();
//            var setupPropertyMethod = mock.GetType().GetMethods()
//                .First(m => m.Name == "SetupProperty" && m.GetParameters().Length == 2);
//            var setupGetMethod = mock.GetType().GetMethods()
//                .First(m => m.Name == "SetupGet" && m.GetParameters().Length == 1);
//            var propertyInfos = GetPropertyInfos(mockType).Where(info => info.PropertyType.IsInstanceOfType(request));
//            foreach (var property in propertyInfos) {
//                var expression = GetPropertyExpression(mockType, property);
//                var initialValue =
//                    mock.GetPropertyValue("DefaultValueProvider").CallMethod("ProvideDefault", property.GetGetMethod());
//
//                var mocked = initialValue as IMocked;
//                if (mocked != null) {
//                    SetupAllProperties(mocked.Mock, request);
//                }
//                SetupProperty(mock, property, setupPropertyMethod, expression, setupGetMethod,request);
//            }
//        }

        private void SetupProperty(object targetObject, PropertyInfo property, MethodInfo setupPropertyMethod, Expression expression, MethodInfo setupGetMethod, object request){
            if (property.CanWrite){
                setupPropertyMethod.MakeGenericMethod(property.PropertyType)
                    .Invoke(targetObject, new[]{expression, request });
            }
            else{
                var genericSetupGetMethod = setupGetMethod.MakeGenericMethod(property.PropertyType);
                var returnsMethod = genericSetupGetMethod.ReturnType.GetInterface("IReturnsGetter`2", false)
                    .GetMethod("Returns", new[]{property.PropertyType});

                var returnsGetter = genericSetupGetMethod.Invoke(targetObject, new object[]{expression});
                returnsMethod.Invoke(returnsGetter, new[]{ request });
            }
        }

        private IEnumerable<PropertyInfo> GetPropertyInfos(Type mockType){
            var properties = mockType.GetProperties()
                .Concat(mockType.GetInterfaces().SelectMany(i => i.GetProperties()))
                .Where(p =>
                    p.CanRead && CanOverrideGet(p) &&
                    p.GetIndexParameters().Length == 0 &&
                    p.PropertyType != mockType &&
                    !(p.CanWrite ^ (p.CanWrite & CanOverrideSet(p))))
                .Distinct();
            return properties;
        }
    }
}