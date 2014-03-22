using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.Xpo{
    public class CustomFunctionValueProcessor:CriteriaProcessorBase, ICriteriaVisitor{
        object ICriteriaVisitor.Visit(BetweenOperator theOperator){
            Process(theOperator);
            var leftOperandValue = GetCustomFunctionOperandValue(theOperator.BeginExpression);
            if (leftOperandValue != null)
                theOperator.BeginExpression = leftOperandValue;
            var rightOperandValue = GetCustomFunctionOperandValue(theOperator.EndExpression);
            if (rightOperandValue != null)
                theOperator.EndExpression = rightOperandValue;
            return theOperator;
        }

        object ICriteriaVisitor.Visit(BinaryOperator theOperator) {
            Process(theOperator);
            var leftOperandValue = GetCustomFunctionOperandValue(theOperator.LeftOperand);
            if (leftOperandValue!=null)
                theOperator.LeftOperand=leftOperandValue;
            var rightOperandValue = GetCustomFunctionOperandValue(theOperator.RightOperand);
            if (rightOperandValue!=null)
                theOperator.RightOperand=rightOperandValue;
            return theOperator;
        }

        private OperandValue GetCustomFunctionOperandValue(CriteriaOperator theOperator){
            var functionOperator = theOperator as FunctionOperator;
            return functionOperator != null? (functionOperator.OperatorType == FunctionOperatorType.Custom
                    ? new OperandValue(theOperator.Accept(this)): null): null;
        }

        object ICriteriaVisitor.Visit(FunctionOperator theOperator){
            Process(theOperator);
            if (theOperator.OperatorType == FunctionOperatorType.Custom){
                var customFunctionOperator =CriteriaOperator.GetCustomFunction(((OperandValue)theOperator.Operands.First()).Value.ToString());
                if (customFunctionOperator != null) {
                    var parameters =theOperator.Operands.OfType<OperandValue>().Skip(1).Select(operand => operand.Value);
                    return customFunctionOperator.Evaluate(parameters);
                }

            }
            return theOperator;
        }
    }
}