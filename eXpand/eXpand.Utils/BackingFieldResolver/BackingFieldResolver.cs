using System;
using System.Reflection;
using System.Reflection.Emit;

namespace eXpand.Utils.BackingFieldResolver {
    internal static class BackingFieldResolver {
        private static readonly ILPattern GetterPattern =
            ILPattern.Sequence(
                ILPattern.Optional(OpCodes.Nop),
                ILPattern.Either(
                    ILPattern.Field(OpCodes.Ldsfld),
                    ILPattern.Sequence(
                        ILPattern.OpCode(OpCodes.Ldarg_0),
                        ILPattern.Field(OpCodes.Ldfld))),
                ILPattern.Optional(
                    ILPattern.Sequence(
                        ILPattern.OpCode(OpCodes.Stloc_0),
                        ILPattern.OpCode(OpCodes.Br_S),
                        ILPattern.OpCode(OpCodes.Ldloc_0))),
                ILPattern.OpCode(OpCodes.Ret));

        private static readonly ILPattern SetterPattern =
            ILPattern.Sequence(
                ILPattern.Optional(OpCodes.Nop),
                ILPattern.OpCode(OpCodes.Ldarg_0),
                ILPattern.Either(
                    ILPattern.Field(OpCodes.Stsfld),
                    ILPattern.Sequence(
                        ILPattern.OpCode(OpCodes.Ldarg_1),
                        ILPattern.Field(OpCodes.Stfld))),
                ILPattern.OpCode(OpCodes.Ret));

        private static FieldInfo GetBackingField(MethodInfo method, ILPattern pattern) {
            MatchContext result = ILPattern.Match(method, pattern);
            if (!result.success)
                throw new NotSupportedException();

            return result.field;
        }

        public static FieldInfo GetBackingField(this PropertyInfo self) {
            MethodInfo getter = self.GetGetMethod(true);
            if (getter != null)
                return GetBackingField(getter, GetterPattern);

            MethodInfo setter = self.GetSetMethod(true);
            if (setter != null)
                return GetBackingField(setter, SetterPattern);

            throw new ArgumentException();
        }
    }
}