using DevExpress.Data.Filtering;

namespace eXpand.Xpo.FunctionOperators
{
    public class YearFunction:FunctionOperator
    {
        public const string YearFunctionName = "Year";


        public YearFunction()
        {
            OperatorType = FunctionOperatorType.Custom;
            Operands.Clear();
            Operands.Add(new OperandValue(YearFunctionName));
        }

        public YearFunction(string propertyName):this()
        {
            Operands.Add(new OperandProperty(propertyName));
        }
        public YearFunction(string propertyName, int year):this(propertyName)
        {
            Operands.Add(new OperandValue(year));
        }
    }

    public class ContainsFunction : FunctionOperator
    {
        public const string ContainsFunctionName = "contains";

        public ContainsFunction(string propertyName, OperandValue operandValue)
        {
            OperatorType = FunctionOperatorType.Custom;
            Operands.Clear();
            Operands.Add(new OperandValue(ContainsFunctionName));
            Operands.Add(new OperandProperty(propertyName));
//            string[] strings = operandValue.Value.ToString().Split(' ');
//            string value = "";
//            for (int i = 0; i < strings.Length; i++)
//            {
//                strings[i] = strings[i].Trim() + " NEAR ";
//                value += strings[i];
//            }
//            value = value.Substring(0, value.LastIndexOf(" NEAR "));
//            operandValue.Value=value;
            operandValue.Value = "\"" + operandValue.Value + "\"";
            Operands.Add(operandValue);
        }
    }
}