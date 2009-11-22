using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.ExpressionBuilder;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class PersistentClassInfoTypeBuilder : Builder<Type>, IAssemblyNameBuilder, ITypeDefineBuilder
    {
        
        private ModuleBuilder _moduleBuilder;
        private string _assemblyName;
        private AssemblyBuilder _assemblyBuilder;


        public static IAssemblyNameBuilder BuildClass()
        {
            return new PersistentClassInfoTypeBuilder();
        }


        ITypeDefineBuilder IAssemblyNameBuilder.WithAssemblyName(string assemblyName)
        {
            _assemblyName = assemblyName;
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName),AssemblyBuilderAccess.RunAndSave, @"C:\");
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");
            return this;
        }

        private void createConstructors(TypeBuilder type, Type parent){
            const MethodAttributes flags2 = MethodAttributes.SpecialName | MethodAttributes.HideBySig |MethodAttributes.Public;
            foreach (ConstructorInfo ci in parent.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)){
                ParameterInfo[] parameters = ci.GetParameters();
                if (parameters.Length == 0) {
                    type.DefineDefaultConstructor(flags2);
                }
                else
                {
                    Type[] types = GetConstructorParameterTypes(parameters);
                    ConstructorBuilder cb = type.DefineConstructor(flags2, ci.CallingConvention, types);
                    for (int i = 0; i < parameters.Length; i++) {
                        cb.DefineParameter(i, parameters[i].Attributes, parameters[i].Name);
                    }
                    emitConstructor(ci, parameters, cb);
                }
            }
        }

        private void emitConstructor(ConstructorInfo ci, ParameterInfo[] parameters, ConstructorBuilder cb) {
            ILGenerator generator = cb.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            for (int i = 0; i < parameters.Length; i++) {
                switch (i) {
                    case 0:
                        generator.Emit(OpCodes.Ldarg_1);
                        break;
                    case 1:
                        generator.Emit(OpCodes.Ldarg_2);
                        break;
                    case 2:
                        generator.Emit(OpCodes.Ldarg_3);
                        break;
                    default:
                        generator.Emit(OpCodes.Ldarg_S, i + 1);
                        break;
                }
            }
            generator.Emit(OpCodes.Call, ci);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ret);
        }

        private Type[] GetConstructorParameterTypes(ParameterInfo[] parameters) {
            var types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++) {
                types[i] = parameters[i].ParameterType;
            }
            return types;
        }

        public static string GetNamePrefix(INamePrefix persistentTypeNamePrefix)
        {
            if (persistentTypeNamePrefix != null)
                return persistentTypeNamePrefix.NamePrefix;
            return null;
        }

        private TypeBuilder GetTypeBuilder(IPersistentClassInfo classInfo) {
            var parent =classInfo.BaseType?? classInfo.GetDefaultBaseClass();
            var namePrefix = GetNamePrefix(classInfo as INamePrefix);
            var typeBuilder = _moduleBuilder.DefineType(string.Format("{0}.{1}{2}", _assemblyName, namePrefix, classInfo.Name), TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit, parent);
            addInterFaces(classInfo, typeBuilder);
            AttributeBuilder.BuildAttribute().Define(classInfo.TypeAttributes, customAttributeBuilder => typeBuilder.SetCustomAttribute(customAttributeBuilder));
//            defineAttributes(classInfo.TypeAttributes, builder => typeBuilder.SetCustomAttribute(builder));
            addCaptionAttribute(typeBuilder.Name, namePrefix,attributeBuilder => typeBuilder.SetCustomAttribute(attributeBuilder));
            createConstructors(typeBuilder, parent);
            return typeBuilder;
        }

//        string getNamePrefix(INamePrefix persistentTypeNamePrefix) {
//            if (persistentTypeNamePrefix!= null)
//                return persistentTypeNamePrefix.NamePrefix;
//            return null;
//        }

        private void addInterFaces(IPersistentClassInfo classInfo,TypeBuilder typeBuilder) {
            foreach (var interfaceInfo in classInfo.Interfaces){
                typeBuilder.AddInterfaceImplementation(interfaceInfo.Type);
            }
        }

        Type CreateType(TypeBuilder typeBuilder) {
            return typeBuilder.CreateType();
        }

        public Type Define(IPersistentClassInfo classInfo) {
            TypeBuilder typeBuilder = GetTypeBuilder(classInfo);
            defineProperties(classInfo, typeBuilder);
//            createMissingPropertiesFromInterfaces(classInfo, typeBuilder);
//            createProperties(classInfo, typeBuilder);
            var define = CreateType(typeBuilder);
            return define;
        }

        void defineProperties(IPersistentClassInfo classInfo, TypeBuilder typeBuilder) {
            PersistentMemberInfoBuilder.GetBuildProperties().With(_moduleBuilder, typeBuilder).Define(classInfo, (builder, prefix) => {
                var namePrefix = GetNamePrefix(prefix);
                addCaptionAttribute(builder.Name, namePrefix, attributeBuilder => builder.SetCustomAttribute(attributeBuilder));
            });
        }


        public List<TypeInfo> Define(IList<IPersistentClassInfo> classInfos) {
            var builders = defineBuilders(classInfos);
            defineInheritance(builders);
            defineProperties(builders);
            return createTypes(builders);
        }

        private List<TypeInfo> createTypes(IEnumerable<TypeBuilderInfo> builders) {
            return builders.OrderBy(info => info.Order).Select(builder =>new TypeInfo(CreateType(builder.TypeBuilder),builder.IPersistentClassInfo)).ToList();
        }

        private void defineProperties(IEnumerable<TypeBuilderInfo> builders) {
            foreach (var builder in builders) {
                Define(builder.IPersistentClassInfo);
            }            
        }

        private void defineInheritance(IEnumerable<TypeBuilderInfo> builders) {
            foreach (var builder in builders.Where(pair => !string.IsNullOrEmpty(pair.IPersistentClassInfo.BaseTypeAssemblyQualifiedName))) {
                TypeBuilderInfo pair = builder;
                var parent = _moduleBuilder.GetTypes().Where(type => type.AssemblyQualifiedName==pair.IPersistentClassInfo.BaseTypeAssemblyQualifiedName).SingleOrDefault();
                builder.TypeBuilder.SetParent(builder.IPersistentClassInfo.BaseType?? parent);
                if (builder.TypeBuilder.BaseType == parent)
                    builders.Where(info => info.TypeBuilder == parent).Single().Order = builder.Order - 1;
            }
        }

        private class TypeBuilderInfo {
            public TypeBuilderInfo(int order, TypeBuilder typeBuilder, IPersistentClassInfo persistentClassInfo) {
                Order = order;
                TypeBuilder = typeBuilder;
                IPersistentClassInfo = persistentClassInfo;
            }

            public int Order { get; set; }
            public TypeBuilder TypeBuilder { get; private set; }
            public IPersistentClassInfo IPersistentClassInfo { get; private set; }
        }
        private IEnumerable<TypeBuilderInfo> defineBuilders(IEnumerable<IPersistentClassInfo> classInfos)
        {
            return classInfos.Select(classInfo => new TypeBuilderInfo(0, GetTypeBuilder(classInfo), classInfo)).ToList();
        }

        AssemblyBuilder ITypeDefineBuilder.AssemblyBuilder {
            get { return _assemblyBuilder; }
        }

        static void addCaptionAttribute(string name , string namePrefix, Action<CustomAttributeBuilder> afterCreated)
        {
            var constructor = typeof(CustomAttribute).GetConstructor(new[] { typeof(string), typeof(string) });
            if (namePrefix != null && name.StartsWith(namePrefix + "")) name = name.Substring(namePrefix.Length);
            var customAttributeBuilder = new CustomAttributeBuilder(constructor, new object[] { ClassInfoNodeWrapper.CaptionAttribute, name });
            afterCreated.Invoke(customAttributeBuilder);            
        }

        ModuleBuilder ITypeDefineBuilder.ModuleBuilder {
            get { return _moduleBuilder; }
        }


//        private void createProperties(IPersistentClassInfo persistentClassInfo, TypeBuilder typeBuilder)
//        {
//            foreach (IPersistentMemberInfo ownMember in persistentClassInfo.OwnMembers){
//                var memberInfoType = typeof(XPCollection);
//                if (ownMember is IPersistentCoreTypeMemberInfo)
//                    memberInfoType = Type.GetType("System." + ((IPersistentCoreTypeMemberInfo) ownMember).DataType, true);
//                else if (ownMember is IPersistentReferenceMemberInfo) {
//                    var persistentReferenceMemberInfo = ((IPersistentReferenceMemberInfo)ownMember);
//                    memberInfoType = persistentReferenceMemberInfo.ReferenceType ??
//                                     _moduleBuilder.GetTypes().Where(type =>type.AssemblyQualifiedName ==persistentReferenceMemberInfo.ReferenceTypeAssemblyQualifiedName).Single();
//                }
//                else if (ownMember is IPersistentCollectionMemberInfo) {
//                    memberInfoType= typeof(XPCollection);
//                    createCollectionProperty(typeBuilder, ownMember, memberInfoType);
//                    continue;
//                }
//                var namePrefix = getNamePrefix(ownMember as INamePrefix);
//                var property = createGetSetProperty(typeBuilder,namePrefix+ ownMember.Name,ownMember.TypeAttributes,memberInfoType,
//                                                    null);
//                addCaptionAttribute(ownMember.Name, namePrefix,builder => property.SetCustomAttribute(builder));
//            }
//            createMissingPropertiesFromInterfaces(persistentClassInfo,typeBuilder);
//        }

//        private void createMissingPropertiesFromInterfaces(IPersistentClassInfo info, TypeBuilder typeBuilder) {
//            foreach (var interfaceType in info.Interfaces){
//                foreach (var property in interfaceType.Type.GetProperties()) {
//                    PropertyInfo propertyInfo = property;
//                    if (info.OwnMembers.Where(memberInfo => memberInfo.Name==propertyInfo.Name).FirstOrDefault()== null) {
//                        var namePrefix = getNamePrefix(interfaceType as INamePrefix);
//                        var propertyBuilder = createGetSetProperty(typeBuilder, namePrefix + property.Name, null, property.PropertyType, info.Name + "." + interfaceType.Type.Name+".");
//                        addCaptionAttribute(propertyBuilder.Name,namePrefix,builder => propertyBuilder.SetCustomAttribute(builder));
//                    }
//                }
//            }
//        }

//        private void createCollectionProperty(TypeBuilder type, IPersistentMemberInfo memberInfo, Type memberInfoType) {
//
//            PropertyBuilder property = type.DefineProperty(memberInfo.Name, PropertyAttributes.HasDefault, memberInfoType,null);
//            const MethodAttributes GetSetAttr =MethodAttributes.SpecialName| MethodAttributes.Public | MethodAttributes.HideBySig;
//            MethodBuilder getmethod = type.DefineMethod("get_"+memberInfo.Name, GetSetAttr, memberInfoType, Type.EmptyTypes);
//            ILGenerator generator = getmethod.GetILGenerator();
//            generator.Emit(OpCodes.Ldarg_0);
//            generator.Emit(OpCodes.Nop);
//            generator.Emit(OpCodes.Ldstr, memberInfo.Name);
//            generator.Emit(OpCodes.Call,typeof(XPBaseObject).GetMethod("GetCollection",BindingFlags.Instance | BindingFlags.NonPublic,new GetCollectionBinder(), new[] { typeof(string) }, null));
//            generator.Emit(OpCodes.Ret);
//            property.SetGetMethod(getmethod);
//            AttributeBuilder.BuildAttribute().Define(memberInfo.TypeAttributes, attributeBuilder => property.SetCustomAttribute(attributeBuilder));
//            defineAttributes(memberInfo.TypeAttributes, builder => property.SetCustomAttribute(builder));
//            
//        }

//        private static PropertyBuilder createGetSetProperty(TypeBuilder type, string memberName, IEnumerable<IPersistentAttributeInfo> attributeInfos, Type memberInfoType, string explicitName)
//        {
//            
//            FieldBuilder field = type.DefineField("_" + memberName, memberInfoType, FieldAttributes.Private);
//            PropertyBuilder property = type.DefineProperty(explicitName+memberName, PropertyAttributes.HasDefault, memberInfoType, null);
//            const MethodAttributes GetSetAttr = MethodAttributes.Public |MethodAttributes.HideBySig|MethodAttributes.Virtual;
//            definePropertyGetMethod(type, memberInfoType, GetSetAttr, field, property,memberName);
//            definePropertySetMethod(type, memberInfoType, GetSetAttr, field, property, memberName);
//            if (attributeInfos != null) {
//                defineAttributes(attributeInfos, builder => property.SetCustomAttribute(builder));
//                AttributeBuilder.BuildAttribute().Define(attributeInfos, builder => property.SetCustomAttribute(builder));
//            }
//
//            return property;
//        }

//        private static void defineAttributes(IEnumerable<IPersistentAttributeInfo> persistentAttributeInfos,Action<CustomAttributeBuilder> afterCreated) {
//            foreach (IPersistentAttributeInfo attributeInfo in persistentAttributeInfos) {
//                AttributeInfo attribute = attributeInfo.Create();
//                afterCreated.Invoke(new CustomAttributeBuilder(attribute.Constructor,attribute.InitializedArgumentValues));
//            }
//        }

//        private static void definePropertySetMethod(TypeBuilder type, Type memberInfoType, MethodAttributes GetSetAttr, FieldBuilder field, PropertyBuilder property, string memberName) {
//            MethodBuilder currSetPropMthdBldr =type.DefineMethod("set_"+memberName,GetSetAttr,null,new[] { memberInfoType });
//            ILGenerator generator = currSetPropMthdBldr.GetILGenerator();
//            generator.Emit(OpCodes.Ldarg_0);
//            generator.Emit(OpCodes.Ldarg_1);
//            generator.Emit(OpCodes.Stfld, field);
//
//            generator.Emit(OpCodes.Ldarg_0);
//            generator.Emit(OpCodes.Nop);
//            generator.Emit(OpCodes.Ldstr, memberName);
//            var methodInfo = typeof(XPBaseObject).GetMethod("OnChanged",
//                                                           BindingFlags.Instance | BindingFlags.NonPublic,
//                                                           null, new[] { typeof(string) }, null);
//            generator.Emit(OpCodes.Call,methodInfo);
//
//            generator.Emit(OpCodes.Ret);
//            property.SetSetMethod(currSetPropMthdBldr);
//        }

//        private static void definePropertyGetMethod(TypeBuilder type, Type memberInfoType, MethodAttributes GetSetAttr, FieldBuilder field, PropertyBuilder property, string memberName) {
//            MethodBuilder defineMethod =type.DefineMethod("get_"+memberName,GetSetAttr,memberInfoType,Type.EmptyTypes);
//            ILGenerator currGetIL = defineMethod.GetILGenerator();
//            currGetIL.Emit(OpCodes.Ldarg_0);
//            currGetIL.Emit(OpCodes.Ldfld, field);
//            currGetIL.Emit(OpCodes.Ret);
//            property.SetGetMethod(defineMethod);
//        }
    }
}