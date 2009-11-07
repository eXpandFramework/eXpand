using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Utils.ExpressionBuilder;
using PropertyAttributes=System.Reflection.PropertyAttributes;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator.ClassTypeBuilder {
    public class PersistentTypeBuilder : Builder<Type>, IAssemblyNameBuilder, ITypeDefineBuilder
    {
        
        private ModuleBuilder _moduleBuilder;
        private string _assemblyName;
        private AssemblyBuilder _assemblyBuilder;


        public static IAssemblyNameBuilder BuildClass()
        {
            return new PersistentTypeBuilder();
        }


        ITypeDefineBuilder IAssemblyNameBuilder.WithAssemblyName(string assemblyName)
        {
            _assemblyName = assemblyName;
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName),AssemblyBuilderAccess.RunAndSave, @"C:\");
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");
            var typeBuilder = _moduleBuilder.DefineType(_assemblyName + ".Module" ,
                                                    TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit,
                                                    typeof (ModuleBase));
            typeBuilder.CreateType();
            return this;
        }

        private void createConstructors(TypeBuilder type, Type parent){
            const MethodAttributes flags2 = MethodAttributes.SpecialName | MethodAttributes.HideBySig |MethodAttributes.Public;
            foreach (ConstructorInfo ci in parent.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
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

        private TypeBuilder GetTypeBuilder(IPersistentClassInfo classInfo) {
            var parent =classInfo.BaseType?? classInfo.GetDefaultBaseClass();
            var typeBuilder = _moduleBuilder.DefineType(_assemblyName + "." + classInfo.Name, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit, parent);
            addInterFaces(classInfo, typeBuilder);
            defineAttributes(classInfo.TypeAttributes, builder => typeBuilder.SetCustomAttribute(builder));
            createConstructors(typeBuilder, parent);
            return typeBuilder;
        }

        public Type Define(IPersistentClassInfo classInfo) {
            TypeBuilder typeBuilder = GetTypeBuilder(classInfo);
            createProperties(classInfo, typeBuilder);
            
            var define = typeBuilder.CreateType();
            return define;
        }

        private void addInterFaces(IPersistentClassInfo classInfo,TypeBuilder typeBuilder) {
            foreach (var interfaceInfo in classInfo.Interfaces){
                IInterfaceInfo info = interfaceInfo;
                var assembly1 = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => new AssemblyName(assembly.FullName+"").Name == info.Assembly).SingleOrDefault();
                var type = assembly1.GetType(info.Name);
                typeBuilder.AddInterfaceImplementation(type);
            }
        }

        public List<TypeInfo> Define(IList<IPersistentClassInfo> classInfos) {
            var builders = defineBuilders(classInfos);
            defineInheritance(builders);
            defineProperties(builders);
            return createTypes(builders);
        }

        private List<TypeInfo> createTypes(List<TypeBuilderInfo> builders) {
            var types = new List<TypeInfo>();
            foreach (var builder in builders.OrderBy(info => info.Order)) {
                types.Add(new TypeInfo(builder.TypeBuilder.CreateType(),builder.IPersistentClassInfo));
            }
            return types;
        }

        private void defineProperties(List<TypeBuilderInfo> builders) {
            foreach (var builder in builders) {
                createProperties(builder.IPersistentClassInfo, builder.TypeBuilder);
            }
        }

        private void defineInheritance(List<TypeBuilderInfo> builders) {
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
        private List<TypeBuilderInfo> defineBuilders(IList<IPersistentClassInfo> classInfos)
        {
            var typeBuilders = new List<TypeBuilderInfo>();
            foreach (var classInfo in classInfos) {
                typeBuilders.Add(new TypeBuilderInfo(0, GetTypeBuilder(classInfo),classInfo));
            }
            return typeBuilders;
        }

        AssemblyBuilder ITypeDefineBuilder.AssemblyBuilder {
            get { return _assemblyBuilder; }
        }

        ModuleBuilder ITypeDefineBuilder.ModuleBuilder {
            get { return _moduleBuilder; }
        }


        private void createProperties(IPersistentClassInfo classInfo, TypeBuilder typeBuilder) {
            foreach (var ownMember in classInfo.OwnMembers){
                var memberInfoType = typeof(XPCollection);
                if (ownMember is IPersistentCoreTypeMemberInfo)
                    memberInfoType = Type.GetType("System." + ((IPersistentCoreTypeMemberInfo) ownMember).DataType, true);
                else if (ownMember is IPersistentReferenceMemberInfo) {
                    var persistentReferenceMemberInfo = ((IPersistentReferenceMemberInfo)ownMember);
                    memberInfoType = persistentReferenceMemberInfo.ReferenceType ??
                                     _moduleBuilder.GetTypes().Where(type =>type.AssemblyQualifiedName ==persistentReferenceMemberInfo.ReferenceTypeAssemblyQualifiedName).Single();
                }
                else if (ownMember is IPersistentCollectionMemberInfo) {
                    memberInfoType= typeof(XPCollection);
                    createCollectionProperty(typeBuilder, ownMember, memberInfoType);
                    continue;
                }
                createGetSetProperty(typeBuilder, ownMember,memberInfoType);    
            }
        }

        private void createCollectionProperty(TypeBuilder type, IPersistentMemberInfo memberInfo, Type memberInfoType) {

            PropertyBuilder property = type.DefineProperty(memberInfo.Name, PropertyAttributes.HasDefault, memberInfoType,
                                                           null);
            const MethodAttributes GetSetAttr =MethodAttributes.SpecialName| MethodAttributes.Public | MethodAttributes.HideBySig;
            MethodBuilder getmethod = type.DefineMethod("get_"+memberInfo.Name, GetSetAttr, memberInfoType, Type.EmptyTypes);
            ILGenerator generator = getmethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ldstr, memberInfo.Name);
            generator.Emit(OpCodes.Call,
                           typeof(XPBaseObject).GetMethod("GetCollection",
                                                           BindingFlags.Instance | BindingFlags.NonPublic,
                                                          new MyBinder(), new[] { typeof(string) }, null));
            generator.Emit(OpCodes.Ret);
            property.SetGetMethod(getmethod);
            defineAttributes(memberInfo.TypeAttributes, builder => property.SetCustomAttribute(builder));
        }

        private static void createGetSetProperty(TypeBuilder type, IPersistentMemberInfo memberInfo, Type memberInfoType) {
            
            FieldBuilder field = type.DefineField("_" + memberInfo.Name, memberInfoType, FieldAttributes.Private);
            PropertyBuilder property = type.DefineProperty(memberInfo.Name, PropertyAttributes.HasDefault, memberInfoType, null);
            const MethodAttributes GetSetAttr = MethodAttributes.Public |MethodAttributes.HideBySig|MethodAttributes.Virtual;
            definePropertyGetMethod(type, memberInfoType, GetSetAttr, field, property);
            definePropertySetMethod(type, memberInfoType, GetSetAttr, field, property);
            defineAttributes(memberInfo.TypeAttributes,builder => property.SetCustomAttribute(builder));            
        }

        private static void defineAttributes(IList<IPersistentAttributeInfo> persistentAttributeInfos,Action<CustomAttributeBuilder> afterCreated) {
            foreach (IPersistentAttributeInfo attributeInfo in persistentAttributeInfos) {
                AttributeInfo attribute = attributeInfo.Create();
                afterCreated.Invoke(new CustomAttributeBuilder(attribute.Constructor,attribute.InitializedArgumentValues));
            }
        }

        private static void definePropertySetMethod(TypeBuilder type, Type memberInfoType, MethodAttributes GetSetAttr, FieldBuilder field, PropertyBuilder property) {
            MethodBuilder currSetPropMthdBldr =type.DefineMethod("set_"+property.Name,GetSetAttr,null,new[] { memberInfoType });
            ILGenerator generator = currSetPropMthdBldr.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, field);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ldstr, property.Name);
            var methodInfo = typeof(XPBaseObject).GetMethod("OnChanged",
                                                           BindingFlags.Instance | BindingFlags.NonPublic,
                                                           null, new[] { typeof(string) }, null);
            generator.Emit(OpCodes.Call,methodInfo);

            generator.Emit(OpCodes.Ret);
            property.SetSetMethod(currSetPropMthdBldr);
        }

        private static void definePropertyGetMethod(TypeBuilder type, Type memberInfoType, MethodAttributes GetSetAttr, FieldBuilder field, PropertyBuilder property) {
            MethodBuilder defineMethod =type.DefineMethod("get_"+property.Name,GetSetAttr,memberInfoType,Type.EmptyTypes);
            ILGenerator currGetIL = defineMethod.GetILGenerator();
            currGetIL.Emit(OpCodes.Ldarg_0);
            currGetIL.Emit(OpCodes.Ldfld, field);
            currGetIL.Emit(OpCodes.Ret);
            property.SetGetMethod(defineMethod);
        }
    }

    internal class MyBinder : Binder
    {
        public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value,
                                              CultureInfo culture)
        {
            return null;
        }

        public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args,
                                                ParameterModifier[] modifiers, CultureInfo culture, string[] names,
                                                out object state)
        {
            var myBinderState = new BinderState();
            var arguments = new Object[args.Length];
            args.CopyTo(arguments, 0);
            state = myBinderState;
            return null;
        }

        public override object ChangeType(object value, Type type, CultureInfo culture)
        {
            return Convert.ChangeType(value, type);
        }

        public override void ReorderArgumentArray(ref object[] args, object state)
        {
        }

        public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types,
                                                ParameterModifier[] modifiers)
        {
            if (match == null) {
                throw new NoNullAllowedException();
            }
            for (int i = 0; i < match.Length; i++) {
                if (!match[i].IsGenericMethod) {
                    return match[i];
                }
            }
            return null;
        }

        public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType,
                                                    Type[] indexes, ParameterModifier[] modifiers)
        {
            return null;
        }
        #region Nested type: BinderState
        private class BinderState
        {
        }
        #endregion
    }
}