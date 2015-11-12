#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Linq;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.Reflection;
using Xpand.ExpressApp.NH.Core;
using DevExpress.Data.Db;
using DevExpress.Xpo.DB.Helpers;
using System.Globalization;
namespace Xpand.ExpressApp.NH.DataLayer
{
    public class CriteriaToHqlConverter : CriteriaToBasicStyleParameterlessProcessor
    {
        private String currentTableName;
        private Type objectType;
        private CriteriaToStringVisitResult ConvertCastFunction(FunctionOperator theOperator, Type type)
        {
            CriteriaToStringVisitResult result = null;
            if (theOperator.Operands.Count >= 1)
            {
                result = new CriteriaToStringVisitResult(String.Format("Cast({0} as {1})", Process(theOperator.Operands[0]).Result, type.FullName));
            }
            return result;
        }
        private Type GetListElementType(String memberName)
        {
            var info = objectType.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public);
            if (info != null)
            {
                return TypeSystem.GetElementType(info.PropertyType);
            }

            return null;
        }

        protected override String GetFunctionText(FunctionOperatorType operatorType)
        {
            if (operatorType == FunctionOperatorType.Upper)
            {
                return "ToUpper";
            }
            else if (operatorType == FunctionOperatorType.Lower)
            {
                return "ToLower";
            }
            else
            {
                return base.GetFunctionText(operatorType);
            }
        }

        protected override String GetInText()
        {
            return base.GetInText();
        }
        protected override String GetIsNotNullText()
        {
            return base.GetIsNotNullText();
        }
        protected override String GetIsNullText()
        {
            return base.GetIsNullText();
        }
        protected override String GetNotLikeText()
        {
            return base.GetNotLikeText();
        }
        protected override String GetOperatorString(Aggregate operatorType)
        {
            return base.GetOperatorString(operatorType);
        }
        public CriteriaToHqlConverter(String currentTableName, Type objectType)
            : base()
        {
            this.currentTableName = currentTableName;
            this.objectType = objectType;
        }

        public CriteriaToHqlConverter(Type objectType)
            : this(null, objectType)
        {

        }
        public override String GetOperatorString(BinaryOperatorType operatorType)
        {
            return base.GetOperatorString(operatorType);
        }
        public override String GetOperatorString(GroupOperatorType operatorType)
        {
            return base.GetOperatorString(operatorType);
        }
        public override String GetOperatorString(UnaryOperatorType operatorType)
        {
            return base.GetOperatorString(operatorType);
        }
        public override CriteriaToStringVisitResult Visit(AggregateOperand operand)
        {
            CriteriaToStringVisitResult result = null;
            if (operand.AggregateType == Aggregate.Count)
            {
                if (Object.ReferenceEquals(operand.CollectionProperty, null))
                {
                    result = new CriteriaToStringVisitResult("Count(0)");
                }
                else
                {
                    result = new CriteriaToStringVisitResult(String.Format("Count(select value 1 from {0})", Process(operand.CollectionProperty).Result));
                }
            }
            else if (operand.AggregateType == Aggregate.Sum)
            {
                if (Object.ReferenceEquals(operand.CollectionProperty, null))
                {
                    result = new CriteriaToStringVisitResult(String.Format("Sum({0})", Process(operand.AggregatedExpression).Result));
                }
                else
                {
                    String collectionTableName = operand.CollectionProperty.PropertyName;
                    Xpand.ExpressApp.NH.DataLayer.CriteriaToHqlConverter criteriaToEFSqlConverter = new Xpand.ExpressApp.NH.DataLayer.CriteriaToHqlConverter(collectionTableName, GetListElementType(operand.CollectionProperty.PropertyName));
                    result = new CriteriaToStringVisitResult(String.Format("Sum(select value {0} from {1} as {2})",
                        criteriaToEFSqlConverter.Convert(operand.AggregatedExpression),
                        Process(operand.CollectionProperty).Result,
                        collectionTableName));
                }
            }
            else if (operand.AggregateType == Aggregate.Exists)
            {
                String collectionTableName = operand.CollectionProperty.PropertyName;
                Xpand.ExpressApp.NH.DataLayer.CriteriaToHqlConverter criteriaToEFSqlConverter = new Xpand.ExpressApp.NH.DataLayer.CriteriaToHqlConverter(collectionTableName, GetListElementType(operand.CollectionProperty.PropertyName));
                result = new CriteriaToStringVisitResult(String.Format("Exists(from {0} as {1} where {2})",
                    Process(operand.CollectionProperty).Result,
                    collectionTableName,
                    criteriaToEFSqlConverter.Convert(operand.Condition)));
            }
            else
            {
                String resultString = ((CriteriaToStringVisitResult)base.Visit(operand)).Result;
                result = new CriteriaToStringVisitResult(resultString.TrimStart('[', ']', '.'));
            }
            return result;
        }
        public override CriteriaToStringVisitResult Visit(BetweenOperator theOperator)
        {
            return base.Visit(theOperator);
        }
        public override CriteriaToStringVisitResult Visit(BinaryOperator theOperator)
        {
            return base.Visit(theOperator);
        }
        public override CriteriaToStringVisitResult Visit(FunctionOperator theOperator)
        {
            switch (theOperator.OperatorType)
            {
                case FunctionOperatorType.ToStr:
                    return ConvertCastFunction(theOperator, typeof(String));
                case FunctionOperatorType.ToDecimal:
                    return ConvertCastFunction(theOperator, typeof(Decimal));
                case FunctionOperatorType.ToDouble:
                    return ConvertCastFunction(theOperator, typeof(Double));
                case FunctionOperatorType.ToFloat:
                    return ConvertCastFunction(theOperator, typeof(Single));
                case FunctionOperatorType.ToInt:
                    return ConvertCastFunction(theOperator, typeof(Int32));
                case FunctionOperatorType.ToLong:
                    return ConvertCastFunction(theOperator, typeof(Int64));
                case FunctionOperatorType.Iif:
                    if (theOperator.Operands.Count >= 3)
                    {
                        return new CriteriaToStringVisitResult(
                            String.Format("case when ({0}) then {1} else {2} end",
                                Process(theOperator.Operands[0]).Result,
                                Process(theOperator.Operands[1]).Result,
                                Process(theOperator.Operands[2]).Result)
                        );
                    }
                    else
                    {
                        return base.Visit(theOperator);
                    }
                case FunctionOperatorType.StartsWith:
                    return new CriteriaToStringVisitResult(MsSqlFormatterHelper.FormatFunction(
                        new ProcessParameter((o) => Process((CriteriaOperator)o).Result),
                        theOperator.OperatorType,
                        new MsSqlFormatterHelper.MSSqlServerVersion(false, false, false),
                        theOperator.Operands.ToArray()));
                case FunctionOperatorType.Contains:
                    if (theOperator.Operands.Count >= 2)
                    {
                        return new CriteriaToStringVisitResult(
                            String.Format(CultureInfo.InvariantCulture, "{0} like '%' + {1} + '%'",
                                Process(theOperator.Operands[0]).Result,
                                Process(theOperator.Operands[1]).Result)
                        );
                    }
                    else
                    {
                        return base.Visit(theOperator);
                    }
                default:
                    object result;
                    if (CriteriaToHqlConverterHelper.ConvertCustomFunctionToValue(theOperator, out result))
                    {
                        return new CriteriaToStringVisitResult(ValueToString(result));
                    }
                    else
                    {
                        return base.Visit(theOperator);
                    }
            }
        }
        public override CriteriaToStringVisitResult Visit(GroupOperator theOperator)
        {
            return base.Visit(theOperator);
        }
        public override CriteriaToStringVisitResult Visit(InOperator theOperator)
        {
            return new CriteriaToStringVisitResult(Process(theOperator.LeftOperand).Result + " " + GetInText() + " {" + ProcessToCommaDelimitedList(theOperator.Operands) + "}");
        }
        public override CriteriaToStringVisitResult Visit(JoinOperand operand)
        {
            return base.Visit(operand);
        }
        public override CriteriaToStringVisitResult Visit(OperandProperty operand)
        {
            string result = operand.PropertyName;
            if (!String.IsNullOrWhiteSpace(operand.PropertyName))
            {
                result = currentTableName + "." + operand.PropertyName;
            }
            return new CriteriaToStringVisitResult(result);
        }
        public override CriteriaToStringVisitResult Visit(OperandValue operand)
        {
            if (operand is OperandParameter)
            {
                if (operand.Value is DateTime)
                {
                    return new CriteriaToStringVisitResult("DateTime'" + ((DateTime)operand.Value).ToString("yyyy-MM-dd HH:mm:ss.fffff") + "'");
                }
                else
                {
                    return new CriteriaToStringVisitResult(ValueToString(operand.Value));
                }
            }
            else if ((operand.Value != null))
            {
                if (operand.Value.GetType().IsEnum)
                {
                    return new CriteriaToStringVisitResult(
                        String.Format("Cast({0} as {1})", ValueToString(System.Convert.ToInt64(operand.Value)), operand.Value.GetType().FullName));
                }
                else if (operand.Value is Guid)
                {
                    return new CriteriaToStringVisitResult(string.Format(CultureInfo.InvariantCulture, "'{0}'", operand.Value));
                }
            }
            return base.Visit(operand);
        }
        public override CriteriaToStringVisitResult Visit(QueryOperand operand)
        {
            return base.Visit(operand);
        }
        public override CriteriaToStringVisitResult Visit(QuerySubQueryContainer operand)
        {
            return base.Visit(operand);
        }
        public override CriteriaToStringVisitResult Visit(UnaryOperator theOperator)
        {
            return base.Visit(theOperator);
        }
        public String Convert(CriteriaOperator expression)
        {
            String result = null;
            if (!Object.ReferenceEquals(expression, null))
            {
                result = Process(expression).Result;
            }
            return result;
        }
        public String Convert(String expression)
        {
            return Convert(CriteriaOperator.Parse(expression));
        }
    }
}
