using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.ExpressionBuilder;

namespace eXpand.ExpressApp.WorldCreator.ClassTypeBuilder {
    public class PersistentClassTypeBuilder : Builder<Type>, IClassAssemblyNameBuilder, IClassDefineBuilder
    {
        
        private ModuleBuilder _moduleBuilder;
        readonly Dictionary<string,ModuleBuilder> assemblies=new Dictionary<string, ModuleBuilder>();
        private string _assemblyName;

        public static IClassAssemblyNameBuilder BuildClass()
        {
            return new PersistentClassTypeBuilder();
        }


        IClassDefineBuilder IClassAssemblyNameBuilder.WithAssemblyName(string assemblyName)
        {
            _assemblyName = assemblyName;
            if (!assemblies.ContainsKey(assemblyName)) {
                AssemblyBuilder assemblyBuilder =
                    AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName),
                                                                  AssemblyBuilderAccess.RunAndSave);
                _moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");
                assemblies[assemblyName] = _moduleBuilder;
            }
            _moduleBuilder = assemblies[assemblyName];
            return this;
        }

        private void CreateConstructors(TypeBuilder type, Type parent)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            const MethodAttributes flags2 = MethodAttributes.SpecialName | MethodAttributes.HideBySig |
                                            MethodAttributes.Public;

            foreach (ConstructorInfo ci in parent.GetConstructors(flags))
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
                    for (int i = 0; i < parameters.Length; i++)
                    {
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

        public Type Define(IPersistentClassInfo classInfo) {
            var parent = classInfo.GetDefaultBaseClass();
            var typeBuilder = _moduleBuilder.DefineType(_assemblyName + "." + classInfo.Name, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit, parent);
            CreateConstructors(typeBuilder, parent);
            var define = typeBuilder.CreateType();
            return define;
        }


    }
}