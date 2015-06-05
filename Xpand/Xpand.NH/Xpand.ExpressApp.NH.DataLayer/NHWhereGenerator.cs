using DevExpress.Data.Db;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo.DB.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Xpand.ExpressApp.NH.DataLayer
{
    class NHWhereGenerator : BaseWhereGenerator, ICriteriaVisitor<string>
    {
        const string nullString = "null";

        string ICriteriaVisitor<string>.Visit(OperandValue theOperand)
        {
            object value = theOperand.Value;
            if (value == null)
                return nullString;
            TypeCode tc = Type.GetTypeCode(value.GetType());
            switch (tc)
            {
                case TypeCode.DBNull:
                case TypeCode.Empty:
                    return nullString;
                case TypeCode.Boolean:
                    return ((bool)value) ? "1" : "0";
                case TypeCode.Char:
                    return "'" + (char)value + "'";
                case TypeCode.DateTime:
                    DateTime datetimeValue = (DateTime)value;
                    string dateTimeFormatPattern;
                    if (datetimeValue.TimeOfDay == TimeSpan.Zero)
                    {
                        dateTimeFormatPattern = "yyyyMMdd";
                    }
                    else if (datetimeValue.Millisecond == 0)
                    {
                        dateTimeFormatPattern = "yyyyMMdd HH:mm:ss";
                    }
                    else
                    {
                        dateTimeFormatPattern = "yyyyMMdd HH:mm:ss.fff";
                    }
                    return "Cast('" + datetimeValue.ToString(dateTimeFormatPattern, CultureInfo.InvariantCulture) + "' as datetime)";
                case TypeCode.String:
                    return AsString(value);
                case TypeCode.Decimal:
                    return FixNonFixedText(((Decimal)value).ToString(CultureInfo.InvariantCulture));
                case TypeCode.Double:
                    return FixNonFixedText(((Double)value).ToString("r", CultureInfo.InvariantCulture));
                case TypeCode.Single:
                    return FixNonFixedText(((Single)value).ToString("r", CultureInfo.InvariantCulture));
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                    if (value is Enum)
                        return Convert.ToInt64(value).ToString();
                    return value.ToString();
                case TypeCode.UInt64:
                    if (value is Enum)
                        return Convert.ToUInt64(value).ToString();
                    return value.ToString();
                case TypeCode.Object:
                default:
                    if (value is Guid)
                    {
                        return "Cast('" + ((Guid)value).ToString() + "' as Guid)";
                    }
                    else if (value is TimeSpan)
                    {
                        return FixNonFixedText(((TimeSpan)value).TotalSeconds.ToString("r", CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        throw new ArgumentException(value.ToString());
                    }
            }
        }
        static string AsString(object value)
        {
            return "'" + value.ToString().Replace("'", "''") + "'";
        }
        static string FixNonFixedText(string toFix)
        {
            if (toFix.IndexOfAny(new char[] { '.', 'e', 'E' }) < 0)
                toFix += ".0";
            return toFix;
        }
        string ICriteriaVisitor<string>.Visit(BinaryOperator theOperator)
        {
            string left = Process(theOperator.LeftOperand);
            string right = Process(theOperator.RightOperand);
            return MsSqlFormatterHelper.FormatBinary(theOperator.OperatorType, left, right);
        }
        protected override string VisitInternal(FunctionOperator theOperator)
        {
            
            
            string[] operands = new string[theOperator.Operands.Count];
            for (int i = 0; i < theOperator.Operands.Count; ++i)
            {
                operands[i] = Process((CriteriaOperator)theOperator.Operands[i]);
            }
            string result = BaseFormatterHelper.DefaultFormatFunction(theOperator.OperatorType,  operands);
            if (result != null) return result;
            return base.VisitInternal(theOperator);
        }
        protected override string FormatOperandProperty(OperandProperty operand)
        {
            return operand.PropertyName;
        }
    }
}
