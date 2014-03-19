using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Xpand.ExpressApp.ImportWizard.Core {
    public abstract class DynamicClass {
        public override string ToString() {
            var props = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var sb = new StringBuilder();
            sb.Append(@"{");
            for (var i = 0; i < props.Length; i++) {
                if (i > 0) sb.Append(@", ");
                sb.Append(props[i].Name);
                sb.Append(@"=");
                sb.Append(props[i].GetValue(this, null));
            }
            sb.Append(@"}");
            return sb.ToString();
        }
    }

    public class DynamicProperty {
        private readonly string _name;
        private readonly Type _type;

        /// <exclude/>
        /// <excludeToc/>
        public DynamicProperty(string name, Type type) {
            if (name == null) throw new ArgumentNullException("name");
            if (type == null) throw new ArgumentNullException("type");
            _name = name;
            _type = type;
        }

        public string Name {
            get {
                return _name;
            }
        }

        public Type Type {
            get {
                return _type;
            }
        }
    }

    internal class ClassFactory {
        public static readonly ClassFactory Instance = new ClassFactory();

        private readonly Dictionary<Signature, Type> _classes;
        private readonly ModuleBuilder _module;
        private int _classCount;

        private ClassFactory() {
            var name = new AssemblyName("DynamicClasses");
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            _module = assembly.DefineDynamicModule("Module");
            _classes = new Dictionary<Signature, Type>();
        }

        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties) {
            var signature = new Signature(properties);
            Type type;
            if (!_classes.TryGetValue(signature, out type)) {
                type = CreateDynamicClass(signature.Properties);
                _classes.Add(signature, type);
            }
            return type;
        }

        private Type CreateDynamicClass(DynamicProperty[] properties) {
            var typeName = @"DynamicClass" + (_classCount + 1);
            var tb = _module.DefineType(
                typeName,
                TypeAttributes.Class |
                TypeAttributes.Public,
                typeof(DynamicClass));
            var fields = GenerateProperties(tb, properties);
            GenerateEquals(tb, fields);
            GenerateGetHashCode(tb, fields);
            var result = tb.CreateType();
            _classCount++;
            return result;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private static void GenerateEquals(TypeBuilder tb, IEnumerable<FieldInfo> fields) {
            var mb = tb.DefineMethod(
                "Equals",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(bool),
                new[] { typeof(object) });
            var gen = mb.GetILGenerator();
            var other = gen.DeclareLocal(tb);
            var next = gen.DefineLabel();
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, tb);
            gen.Emit(OpCodes.Stloc, other);
            gen.Emit(OpCodes.Ldloc, other);
            gen.Emit(OpCodes.Brtrue_S, next);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);
            gen.MarkLabel(next);
            foreach (var field in fields) {
                var ft = field.FieldType;
                var ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new[] { ft, ft }), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private static void GenerateGetHashCode(TypeBuilder tb, IEnumerable<FieldInfo> fields) {
            var mb = tb.DefineMethod(
                "GetHashCode",
                MethodAttributes.Public | MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual | MethodAttributes.HideBySig,
                typeof(int),
                Type.EmptyTypes);
            var gen = mb.GetILGenerator();
            gen.Emit(OpCodes.Ldc_I4_0);
            foreach (var field in fields) {
                var ft = field.FieldType;
                var ct = typeof(EqualityComparer<>).MakeGenericType(ft);
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new[] { ft }), null);
                gen.Emit(OpCodes.Xor);
            }
            gen.Emit(OpCodes.Ret);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        private static IEnumerable<FieldInfo> GenerateProperties(TypeBuilder tb, IList<DynamicProperty> properties) {
            var fields = new FieldBuilder[properties.Count];
            for (var i = 0; i < properties.Count; i++) {
                var dp = properties[i];
                var fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
                var pb = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, null);
                var mbGet = tb.DefineMethod(
                    "get_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    dp.Type,
                    Type.EmptyTypes);
                var genGet = mbGet.GetILGenerator();
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, fb);
                genGet.Emit(OpCodes.Ret);
                var mbSet = tb.DefineMethod(
                    "set_" + dp.Name,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    null,
                    new[] { dp.Type });
                var genSet = mbSet.GetILGenerator();
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, fb);
                genSet.Emit(OpCodes.Ret);
                pb.SetGetMethod(mbGet);
                pb.SetSetMethod(mbSet);
                fields[i] = fb;
            }
            return fields;
        }
    }

    internal class Signature : IEquatable<Signature> {
        public int HashCode;
        public DynamicProperty[] Properties;

        [SuppressMessage("Microsoft.Performance", "CA1805:DoNotInitializeUnnecessarily")]
        public Signature(IEnumerable<DynamicProperty> properties) {
            HashCode = 0;
            if (properties == null) return;

            Properties = properties.ToArray();
            foreach (var p in properties) {
                HashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
            }
        }

        public override bool Equals(object obj) {
            return obj is Signature && Equals((Signature)obj);
        }

        public override int GetHashCode() {
            return HashCode;
        }

        public bool Equals(Signature other) {
            if (Properties.Length != other.Properties.Length) return false;
            return !Properties.Where((t, i) => t.Name != other.Properties[i].Name || t.Type != other.Properties[i].Type).Any();
        }
    }
}