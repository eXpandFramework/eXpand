using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.ExpressionBuilder;

namespace eXpand.ExpressApp.WorldCreator.Core
{
    public class PersistentMemberInfoBuilder : Builder<List<PropertyBuilder>>, IWithTypeBuilder, IDefinePropertyBuilder {
        ModuleBuilder _moduleBuilder;
        TypeBuilder _typeBuilder;
//        IAttributeBuilder _attributeBuilder;
        public event EventHandler PropertiesDefined;

        protected void InvokePropertiesDefined(EventArgs e) {
            EventHandler handler = PropertiesDefined;
            if (handler != null) handler(this, e);
        }

        public IDefinePropertyBuilder With(ModuleBuilder moduleBuilder, TypeBuilder typeBuilder) {
            _moduleBuilder = moduleBuilder;
            _typeBuilder = typeBuilder;
            return this;
        }

        public List<PropertyBuilder> Define(IPersistentClassInfo persistentClassInfo, Action<PropertyBuilder, INamePrefix> created)
        {
            var propertyBuilders = new List<PropertyBuilder>();
            foreach (IPersistentMemberInfo ownMember in persistentClassInfo.OwnMembers)
            {
                var memberInfoType = typeof(XPCollection);
                if (ownMember is IPersistentCoreTypeMemberInfo)
                    memberInfoType = Type.GetType("System." + ((IPersistentCoreTypeMemberInfo)ownMember).DataType, true);
                else if (ownMember is IPersistentReferenceMemberInfo){
                    var persistentReferenceMemberInfo = ((IPersistentReferenceMemberInfo)ownMember);
                    memberInfoType = persistentReferenceMemberInfo.ReferenceType ??
                                     _moduleBuilder.GetTypes().Where(type => type.AssemblyQualifiedName == persistentReferenceMemberInfo.ReferenceTypeAssemblyQualifiedName).Single();
                }
                else if (ownMember is IPersistentCollectionMemberInfo){
                    memberInfoType = typeof(XPCollection);
                    var collectionProperty = createCollectionProperty(_typeBuilder, ownMember, memberInfoType);
                    created.Invoke(collectionProperty,ownMember as INamePrefix);
                    continue;
                }
                var namePrefix = PersistentClassInfoTypeBuilder.GetNamePrefix(ownMember as INamePrefix);
                PropertyBuilder property = createGetSetProperty(_typeBuilder, namePrefix + ownMember.Name, memberInfoType,null);
                created.Invoke(property,ownMember as INamePrefix);
                propertyBuilders.Add(property);
               
            }
            createMissingPropertiesFromInterfaces(persistentClassInfo, _typeBuilder,created,propertyBuilders);
//            InvokePropertiesDefined(new EventArgs());
            return propertyBuilders; 
        }


        private void createMissingPropertiesFromInterfaces(IPersistentClassInfo info, TypeBuilder typeBuilder, Action<PropertyBuilder, INamePrefix> created, List<PropertyBuilder> propertyBuilders)
        {
            foreach (IInterfaceInfo interfaceType in info.Interfaces) {
                foreach (PropertyInfo property in interfaceType.Type.GetProperties()) {
                    PropertyInfo propertyInfo = property;
                    if (info.OwnMembers.Where(memberInfo => memberInfo.Name == propertyInfo.Name).FirstOrDefault() ==null) {
                        var persistentTypeNamePrefix = interfaceType as INamePrefix;
                        var namePrefix =PersistentClassInfoTypeBuilder.GetNamePrefix(persistentTypeNamePrefix);
                        PropertyBuilder propertyBuilder = createGetSetProperty(typeBuilder, namePrefix + property.Name,
                                                                               property.PropertyType,
                                                                               info.Name + "." + interfaceType.Type.Name +
                                                                               ".");
                        propertyBuilders.Add(propertyBuilder);
                        created.Invoke(propertyBuilder,persistentTypeNamePrefix);
                    }
                }
            }
        }


        public static IWithTypeBuilder GetBuildProperties()
        {
            return new PersistentMemberInfoBuilder();
        }


        private PropertyBuilder createGetSetProperty(TypeBuilder type, string memberName, Type memberInfoType, string explicitName)
        {

            FieldBuilder field = type.DefineField("_" + memberName, memberInfoType, FieldAttributes.Private);
            PropertyBuilder property = type.DefineProperty(explicitName + memberName, PropertyAttributes.HasDefault, memberInfoType, null);
            const MethodAttributes GetSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
            definePropertyGetMethod(type, memberInfoType, GetSetAttr, field, property, memberName);
            definePropertySetMethod(type, memberInfoType, GetSetAttr, field, property, memberName);
            return property;
        }
        private static void definePropertySetMethod(TypeBuilder type, Type memberInfoType, MethodAttributes GetSetAttr, FieldBuilder field, PropertyBuilder property, string memberName)
        {
            MethodBuilder currSetPropMthdBldr = type.DefineMethod("set_" + memberName, GetSetAttr, null, new[] { memberInfoType });
            ILGenerator generator = currSetPropMthdBldr.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, field);

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ldstr, memberName);
            var methodInfo = typeof(XPBaseObject).GetMethod("OnChanged",
                                                           BindingFlags.Instance | BindingFlags.NonPublic,
                                                           null, new[] { typeof(string) }, null);
            generator.Emit(OpCodes.Call, methodInfo);

            generator.Emit(OpCodes.Ret);
            property.SetSetMethod(currSetPropMthdBldr);
        }

        private static void definePropertyGetMethod(TypeBuilder type, Type memberInfoType, MethodAttributes GetSetAttr, FieldBuilder field, PropertyBuilder property, string memberName)
        {
            MethodBuilder defineMethod = type.DefineMethod("get_" + memberName, GetSetAttr, memberInfoType, Type.EmptyTypes);
            ILGenerator currGetIL = defineMethod.GetILGenerator();
            currGetIL.Emit(OpCodes.Ldarg_0);
            currGetIL.Emit(OpCodes.Ldfld, field);
            currGetIL.Emit(OpCodes.Ret);
            property.SetGetMethod(defineMethod);
        }


        private PropertyBuilder createCollectionProperty(TypeBuilder type, IPersistentMemberInfo memberInfo, Type memberInfoType)
        {
            PropertyBuilder property = type.DefineProperty(memberInfo.Name, PropertyAttributes.HasDefault, memberInfoType, null);
            const MethodAttributes GetSetAttr = MethodAttributes.SpecialName | MethodAttributes.Public | MethodAttributes.HideBySig;
            MethodBuilder getmethod = type.DefineMethod("get_" + memberInfo.Name, GetSetAttr, memberInfoType, Type.EmptyTypes);
            ILGenerator generator = getmethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Nop);
            generator.Emit(OpCodes.Ldstr, memberInfo.Name);
            generator.Emit(OpCodes.Call, typeof(XPBaseObject).GetMethod("GetCollection", BindingFlags.Instance | BindingFlags.NonPublic, new GetCollectionBinder(), new[] { typeof(string) }, null));
            generator.Emit(OpCodes.Ret);
            property.SetGetMethod(getmethod);
            return property;
        }

    }
}
