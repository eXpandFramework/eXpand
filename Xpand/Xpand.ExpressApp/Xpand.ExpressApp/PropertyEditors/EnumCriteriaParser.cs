using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;

namespace Xpand.ExpressApp.PropertyEditors {
    public class EnumCriteriaParser : ICriteriaVisitor {
        /* Private */
        readonly Dictionary<string, OperandValue> _values = new Dictionary<string, OperandValue>();

        /* Internal */

        /* Constructor */

        public EnumCriteriaParser(string propertyName, Type enumType) {
            PropertyName = propertyName;
            if (enumType.IsGenericType) {
                Type[] types = enumType.GetGenericArguments();
                if (types.Length == 1 && typeof(Nullable<>).MakeGenericType(types[0]) == enumType)
                    enumType = types[0];
            }
            EnumType = enumType;
            foreach (object value in Enum.GetValues(enumType))
                _values.Add(value.ToString(), new OperandValue(value));
        }

        public string PropertyName { get; private set; }
        public Type EnumType { get; private set; }

        /* Operand visitors */
        #region ICriteriaVisitor Members
        public object Visit(FunctionOperator theOperator) {
            /* Nothing special to do here.. */
            return theOperator;
        }

        public object Visit(OperandValue theOperand) {
            /* Nothing special to do here.. */
            return theOperand;
        }

        public object Visit(GroupOperator theOperator) {
            foreach (CriteriaOperator operand in theOperator.Operands)
                operand.Accept(this);

            return theOperator;
        }

        public object Visit(InOperator theOperator) {
            UpdatePropertyName(theOperator.LeftOperand);
            ToValue(theOperator.Operands);

            return theOperator;
        }

        public object Visit(UnaryOperator theOperator) {
            switch (theOperator.OperatorType) {
                case UnaryOperatorType.IsNull:
                    UpdatePropertyName(theOperator.Operand);
                    break;
                case UnaryOperatorType.Not:
                    theOperator.Operand.Accept(this);
                    break;
            }

            return theOperator;
        }

        public object Visit(BinaryOperator theOperator) {
            UpdatePropertyName(theOperator.LeftOperand);

            CriteriaOperator operandValue;

            if (ToValue(theOperator.RightOperand, out operandValue))
                theOperator.RightOperand = operandValue;
            else
                theOperator.RightOperand.Accept(this);

            return theOperator;
        }

        public object Visit(BetweenOperator theOperator) {
            UpdatePropertyName(theOperator.TestExpression);

            CriteriaOperator operandValue;

            if (ToValue(theOperator.BeginExpression, out operandValue))
                theOperator.BeginExpression = operandValue;
            else
                theOperator.BeginExpression.Accept(this);

            if (ToValue(theOperator.EndExpression, out operandValue))
                theOperator.EndExpression = operandValue;
            else
                theOperator.EndExpression.Accept(this);

            return theOperator;
        }
        #endregion
        static void UnsupportedCriteria() {
            throw new InvalidEnumArgumentException("Unsupported criteria.");
        }

        void UpdatePropertyName(CriteriaOperator operand) {
            var operandProperty = operand as OperandProperty;
            if ((ReferenceEquals(operandProperty, null)) ||
                (!operandProperty.PropertyName.Equals(PropertyName)))
                UnsupportedCriteria();
            if (!ReferenceEquals(operandProperty, null)) operandProperty.PropertyName = "Value";
        }

        void ToValue(IList<CriteriaOperator> operands) {
            CriteriaOperator operandValue;
            for (int i = 0; i < operands.Count; i++)
                if (ToValue(operands[i], out operandValue))
                    operands[i] = operandValue;
        }

        bool ToValue(CriteriaOperator operand, out CriteriaOperator operandValue) {
            operandValue = null;

            string valueName;

            var property = operand as OperandProperty;
            if (property != null)
                valueName = property.PropertyName;
            else if (operand is OperandValue && ((OperandValue)operand).Value is string)
                valueName = (string)((OperandValue)operand).Value;
            else
                return false;

            operandValue = _values[valueName];

            return true;
        }

        /* Meta */
    }
}