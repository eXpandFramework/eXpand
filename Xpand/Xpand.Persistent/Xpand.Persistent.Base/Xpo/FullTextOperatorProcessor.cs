using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using Xpand.Xpo.CustomFunctions;

namespace Xpand.Persistent.Base.Xpo{
    public class FullTextOperatorProcessor : CriteriaProcessorBase, ICriteriaVisitor {
        readonly HashSet<string>  _names=new HashSet<string>();

        public FullTextOperatorProcessor(HashSet<string> names){
            _names = names;
        }
        object ICriteriaVisitor.Visit(BinaryOperator theOperator) {
            Process(theOperator);
            
            return theOperator;
        }

        object ICriteriaVisitor.Visit(FunctionOperator theOperator){
            Process(theOperator);
            if (theOperator.OperatorType == FunctionOperatorType.Contains) {
                var propertyName = ((OperandProperty)theOperator.Operands[0]).PropertyName;
                if (_names.Contains(propertyName)) {
                    theOperator.OperatorType=FunctionOperatorType.Custom;
                    theOperator.Operands.Insert(0, new OperandValue(FullTextContainsFunction.FunctionName));
                }
            }
            return theOperator;
        }
    }
}