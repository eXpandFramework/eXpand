using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Xpand.Test.AutoFixture.Customizations{
    public class PersistentObjectParameterCustomization:ICustomization {
        public void Customize(IFixture fixture){
            var persistentTypes = fixture.Create<IObjectSpace>().TypesInfo.PersistentTypes.Where(info => info.IsDomainComponent);
            foreach (var persistentType in persistentTypes){
                var memberInfos = persistentType.OwnMembers.Where(info => info.MemberTypeInfo.IsDomainComponent&& info.MemberType.IsAbstract);
                foreach (var memberInfo in memberInfos){
                    var nonAbstractDescTypeInfo = memberInfo.MemberTypeInfo.Descendants.First(info => !info.IsAbstract);
                    fixture.Customizations.Add(new TypeRelay(memberInfo.MemberType, nonAbstractDescTypeInfo.Type));
                }
            }
            fixture.Customizations.Add(new PersistentObjectParameterSpecimentBuilder());
            
        }
    }

    public class PersistentObjectParameterSpecimentBuilder:ISpecimenBuilder{
        
        public object Create(object request, ISpecimenContext context){
            var pi = request as ParameterInfo;
            if (pi == null){
                return new NoSpecimen();
            }
            var parameterType = GetParameterType(pi.ParameterType, context);
            if (parameterType==typeof(NoSpecimen))
                return new NoSpecimen();
            return CreateObject(parameterType, (SpecimenContext) context);
        }

        private Type GetParameterType(Type parameterType, ISpecimenContext context){
            if (parameterType.IsArray && IsParameterTypeValidCore(parameterType.GetElementType()) ||
                IsParameterTypeValidCore(parameterType))
                return parameterType;
            var type = GetBOType(parameterType, context);
            return type ?? typeof(NoSpecimen);
        }

        private object CreateObject(Type type, SpecimenContext context){
            if (type.IsArray){
                var fixture = (Fixture) context.Builder;
                var objects = new object[fixture.RepeatCount];
                for (int i = 0; i < fixture.RepeatCount; i++){
                    objects[i]=context.Resolve(type.GetElementType());
                }
                return objects;
            }
            return context.Resolve(type);
        }

        private Type GetBOType(Type parameterType, ISpecimenContext context){
            var typesInfo = context.Create<IObjectSpace>().TypesInfo;
            var typeInfos = typesInfo.FindTypeInfo(parameterType).Implementors.Where(info => info.IsPersistent).ToArray();
            return typeInfos.Length == 1 ? typeInfos.First().Type : null;
        }

        private bool IsParameterTypeValidCore(Type type){
            return typeof (IXPObject).IsAssignableFrom(type);
        }
    }
}