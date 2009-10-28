using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Utils.ExpressionBuilder;
using PropertyAttributes=System.Reflection.PropertyAttributes;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator.ClassTypeBuilder {
    public class PersistentClassTypeBuilder : Builder<Type>, IClassAssemblyNameBuilder, IClassDefineBuilder
    {
        
        private ModuleBuilder _moduleBuilder;
//        readonly Dictionary<string,ModuleBuilder> assemblies=new Dictionary<string, ModuleBuilder>();
        private string _assemblyName;
        private AssemblyBuilder _assemblyBuilder;


        public static IClassAssemblyNameBuilder BuildClass()
        {
            return new PersistentClassTypeBuilder();
        }


        IClassDefineBuilder IClassAssemblyNameBuilder.WithAssemblyName(string assemblyName)
        {
            _assemblyName = assemblyName;
//            if (!assemblies.ContainsKey(assemblyName)) {
                _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave,@"C:\");
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");
//                assemblies[assemblyName] = _moduleBuilder;
                
//            }
//            _moduleBuilder = assemblies[assemblyName];
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
                    var types = new Type[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++) {
                        types[i] = parameters[i].ParameterType;
                    }
                    ConstructorBuilder cb = type.DefineConstructor(flags2, ci.CallingConvention, types);
                    for (int i = 0; i < parameters.Length; i++) {
                        cb.DefineParameter(i, parameters[i].Attributes, parameters[i].Name);
                    }
                    
                    ILGenerator generator = cb.GetILGenerator();
                    generator.Emit(OpCodes.Ldarg_0);
                    for (int i = 0; i < parameters.Length; i++) {
                        if (i == 0)
                            generator.Emit(OpCodes.Ldarg_1);
                        else if (i == 1)
                            generator.Emit(OpCodes.Ldarg_2);
                        else if (i == 2)
                            generator.Emit(OpCodes.Ldarg_3);
                        else
                            generator.Emit(OpCodes.Ldarg_S, i + 1);
                    }
                    generator.Emit(OpCodes.Call, ci);
                    generator.Emit(OpCodes.Nop);
                    generator.Emit(OpCodes.Ret);
                }
            }
        }

        private TypeBuilder GetTypeBuilder(IPersistentClassInfo classInfo) {
            var parent = classInfo.GetDefaultBaseClass();
            var typeBuilder = _moduleBuilder.DefineType(_assemblyName + "." + classInfo.Name, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit, parent);
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

        public List<Type> Define(IList<IPersistentClassInfo> classInfos) {
            var builders = new Dictionary<IPersistentClassInfo, TypeBuilder>();
            foreach (var classInfo in classInfos) {
                builders[classInfo] = GetTypeBuilder(classInfo);
            }
            foreach (var builder in builders) {
                createProperties(builder.Key, builder.Value);
            }
            var types = new List<Type>();
            foreach (var builder in builders) {
                types.Add(builder.Value.CreateType());
            }
            return types;
        }

        AssemblyBuilder IClassDefineBuilder.AssemblyBuilder {
            get { return _assemblyBuilder; }
        }

        ModuleBuilder IClassDefineBuilder.ModuleBuilder {
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
                                     _moduleBuilder.GetTypes().Where(type =>type.AssemblyQualifiedName ==persistentReferenceMemberInfo.AssemblyQualifiedName).Single();
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
            const MethodAttributes GetSetAttr = MethodAttributes.Public |MethodAttributes.HideBySig;
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
            ILGenerator currSetIL = currSetPropMthdBldr.GetILGenerator();
            currSetIL.Emit(OpCodes.Ldarg_0);
            currSetIL.Emit(OpCodes.Ldarg_1);
            currSetIL.Emit(OpCodes.Stfld, field);
            currSetIL.Emit(OpCodes.Ret);
            property.SetSetMethod(currSetPropMthdBldr);
        }

        private static void definePropertyGetMethod(TypeBuilder type, Type memberInfoType, MethodAttributes GetSetAttr, FieldBuilder field, PropertyBuilder property) {
            MethodBuilder currGetPropMthdBldr =type.DefineMethod("get_"+property.Name,GetSetAttr,memberInfoType,Type.EmptyTypes);
            ILGenerator currGetIL = currGetPropMthdBldr.GetILGenerator();
            currGetIL.Emit(OpCodes.Ldarg_0);
            currGetIL.Emit(OpCodes.Ldfld, field);
            currGetIL.Emit(OpCodes.Ret);
            property.SetGetMethod(currGetPropMthdBldr);
        }

//        private static void createGetSetProperty(TypeBuilder type, IPersistentMemberInfo memberInfo, bool IsExtended)
//        {
//            
//            ConstructorInfo sizeConstructorInfo = typeof(SizeAttribute).GetConstructor(new[] { typeof(int) });
//            var sizectorArgs = new object[] { SizeAttribute.Unlimited };
//
//            ConstructorInfo associationctor;
//            object[] associationctorArgs;
//
//            ConstructorInfo aggregatedctor = typeof(AggregatedAttribute).GetConstructor(Type.EmptyTypes);
//
//            const MethodAttributes flags2 = MethodAttributes.SpecialName | MethodAttributes.HideBySig |MethodAttributes.Public;
//            //Se define la propiedad
//            PropertyBuilder prop = type.DefineProperty(memberInfo.Name, PropertyAttributes.HasDefault, memberInfo.Type, null);
//            FieldBuilder field = null;
//            var memberInfoType = Type.GetType("System." + ((IPersistentCoreTypeMemberInfo)memberInfo).DataType, true);
//            var isCollection = (memberInfo is IPersistentCollectionMemberInfo);
//            if (!isCollection)
//            {
//                //Se define el campo que se usa para obtener y regresar valores a traves de la propiedad
//                field = type.DefineField("_" + memberInfo.Name, memberInfoType, FieldAttributes.Private);
//            }
//            //Se define el metodo get de la propiedad
//            MethodBuilder getmethod = type.DefineMethod("get_" + memberInfo.Name, flags2, memberInfoType, Type.EmptyTypes);
//
//            //Se generan las instrucciones para el metodo get
//            ILGenerator generator = getmethod.GetILGenerator();
//            generator.Emit(OpCodes.Ldarg_0);
//            if (memberInfo.Type != typeof(XPCollection))
//            {
//                if (field != null) generator.Emit(OpCodes.Ldfld, field);
//            }
//            else
//            {
//                generator.Emit(OpCodes.Nop);
//                generator.Emit(OpCodes.Ldstr, memberInfo.Name);
//                generator.Emit(OpCodes.Call,
//                               typeof(XPBaseObject).GetMethod("GetCollection",
//                                                               BindingFlags.Instance | BindingFlags.NonPublic,
//                                                               new MyBinder(), new[] { typeof(string) }, null));
//            }
//            generator.Emit(OpCodes.Ret);
//
//            if (memberInfo.Type != typeof(XPCollection))
//            {
//                //Se define el metodo set de la propiedad
//                MethodBuilder setmethod = type.DefineMethod("set_" + memberInfo.Name, flags2, null, new[] { memberInfo.Type });
//
//                //Se generan las instrucciones para el metodo set
//                generator = setmethod.GetILGenerator();
//                if (memberInfo.SingleEdition)
//                {
//                    Label IsNotNull = generator.DefineLabel();
//                    generator.Emit(OpCodes.Ldarg_0);
//                    generator.Emit(OpCodes.Call,
//                                   typeof(Functions).GetMethod("IsNewOrIsLoading",
//                                                                BindingFlags.Static | BindingFlags.NonPublic));
//                    generator.Emit(OpCodes.Brfalse_S, IsNotNull);
//
//                    generator.Emit(OpCodes.Ldarg_0);
//                    generator.Emit(OpCodes.Ldarg_1);
//                    if (field != null) generator.Emit(OpCodes.Stfld, field);
//                    generator.Emit(OpCodes.Ret);
//
//                    generator.MarkLabel(IsNotNull);
//                    ConstructorInfo NewException = typeof(Exception).GetConstructor(new[] { typeof(string) });
//                    generator.Emit(OpCodes.Ldstr, memberInfo.Label + " no puede ser editado porque es de edicion unica.");
//                    generator.Emit(OpCodes.Newobj, NewException);
//                    generator.Emit(OpCodes.Throw);
//                }
//                else
//                {
//                    generator.Emit(OpCodes.Ldarg_0);
//                    generator.Emit(OpCodes.Ldarg_1);
//                    if (field != null) generator.Emit(OpCodes.Stfld, field);
//                    generator.Emit(OpCodes.Ret);
//                }
//                prop.SetSetMethod(setmethod);
//            }
//            prop.SetGetMethod(getmethod);
//
//
//            //Si la propiedad es string, se le genera un atributo personalizado con las mismas caracteristicas del
//            //SizeAttribute
//            if (memberInfo.Type == typeof(string))
//            {
//                prop.SetCustomAttribute(new CustomAttributeBuilder(sizeConstructorInfo, sizectorArgs));
//            }
//            if (IsExtended)
//            {
//                ConstructorInfo ctor = typeof(ExtendedPropertyAttribute).GetConstructor(Type.EmptyTypes);
//                prop.SetCustomAttribute(new CustomAttributeBuilder(ctor, new object[] { }));
//            }
//            //Si la propiedad es una relacion se agrega el atributo association
//            if (memberInfo.RelationType == Property.relationType.Muchos_a_Uno)
//            {
//                associationctor = typeof(AssociationAttribute).GetConstructor(new[] { typeof(string) });
//                associationctorArgs = new object[] { memberInfo.RelationName };
//                prop.SetCustomAttribute(new CustomAttributeBuilder(associationctor, associationctorArgs));
//            }
//            if ((memberInfo.RelationType == Property.relationType.Muchos_a_Muchos) ||
//                (memberInfo.RelationType == Property.relationType.Uno_a_Muchos))
//            {
//                associationctor = typeof(AssociationAttribute).GetConstructor(new[] { typeof(string), typeof(Type) });
//                associationctorArgs = new object[] { memberInfo.RelationName, memberInfo.XPCollectionType };
//                prop.SetCustomAttribute(new CustomAttributeBuilder(associationctor, associationctorArgs));
//                if (memberInfo.DataType == Property.dataType.Tabla)
//                {
//                    prop.SetCustomAttribute(new CustomAttributeBuilder(aggregatedctor, new object[] { }));
//                }
//            }
//        }


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