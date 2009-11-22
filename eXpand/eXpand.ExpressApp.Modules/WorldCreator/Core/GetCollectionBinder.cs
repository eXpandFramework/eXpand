using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace eXpand.ExpressApp.WorldCreator.Core {
    internal class GetCollectionBinder : Binder
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
            return match.FirstOrDefault(t => !t.IsGenericMethod);
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