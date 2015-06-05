using DevExpress.Data.Filtering;

namespace Xpand.Xpo.Filtering{
    public class XpandCriteriaProcessorBase : IClientCriteriaVisitor<object>{
        protected int AggregateLevel { get; private set; }

        object IClientCriteriaVisitor<object>.Visit(AggregateOperand operand){
            Process(operand);
            AggregateLevel++;
            if (!ReferenceEquals(operand.AggregatedExpression, null)){
                operand.AggregatedExpression.Accept(this);
            }
            if (!ReferenceEquals(operand.CollectionProperty, null)){
                operand.CollectionProperty.Accept(this);
            }
            AggregateLevel--;
            return null;
        }

        object ICriteriaVisitor<object>.Visit(BetweenOperator theOperator){
            Process(theOperator);
            if (!ReferenceEquals(theOperator.BeginExpression, null)){
                theOperator.BeginExpression.Accept(this);
            }
            if (!ReferenceEquals(theOperator.EndExpression, null)){
                theOperator.EndExpression.Accept(this);
            }
            if (!ReferenceEquals(theOperator.TestExpression, null)){
                theOperator.TestExpression.Accept(this);
            }
            return null;
        }

        object ICriteriaVisitor<object>.Visit(BinaryOperator theOperator){
            Process(theOperator);
            if (!ReferenceEquals(theOperator.LeftOperand, null)){
                theOperator.LeftOperand.Accept(this);
            }
            if (!ReferenceEquals(theOperator.RightOperand, null)){
                theOperator.RightOperand.Accept(this);
            }
            return null;
        }

        object ICriteriaVisitor<object>.Visit(FunctionOperator theOperator){
            Process(theOperator);
            foreach (CriteriaOperator @operator in theOperator.Operands){
                @operator.Accept(this);
            }
            return null;
        }

        object ICriteriaVisitor<object>.Visit(GroupOperator theOperator){
            Process(theOperator);
            foreach (CriteriaOperator @operator in theOperator.Operands){
                @operator.Accept(this);
            }
            return null;
        }

        object ICriteriaVisitor<object>.Visit(InOperator theOperator){
            Process(theOperator);
            if (!ReferenceEquals(theOperator.LeftOperand, null)){
                theOperator.LeftOperand.Accept(this);
            }
            foreach (CriteriaOperator @operator in theOperator.Operands){
                @operator.Accept(this);
            }
            return null;
        }

        object ICriteriaVisitor<object>.Visit(OperandValue operand){
            Process(operand);
            return null;
        }

        object ICriteriaVisitor<object>.Visit(UnaryOperator theOperator){
            Process(theOperator);
            if (!ReferenceEquals(theOperator.Operand, null)){
                theOperator.Operand.Accept(this);
            }
            return null;
        }

        object IClientCriteriaVisitor<object>.Visit(JoinOperand operand){
            Process(operand);
            if (!ReferenceEquals(operand.AggregatedExpression, null)){
                operand.AggregatedExpression.Accept(this);
            }
            return null;
        }

        object IClientCriteriaVisitor<object>.Visit(OperandProperty operand){
            Process(operand);
            return null;
        }

        protected virtual void Process(AggregateOperand operand){
        }

        protected virtual void Process(BetweenOperator theOperator){
        }

        protected virtual void Process(BinaryOperator theOperator){
        }

        public void Process(CriteriaOperator criteria){
            if (!ReferenceEquals(criteria, null)){
                criteria.Accept(this);
            }
        }

        protected virtual void Process(FunctionOperator theOperator){
        }

        protected virtual void Process(GroupOperator theOperator){
        }

        protected virtual void Process(InOperator theOperator){
        }

        protected virtual void Process(JoinOperand operand){
        }

        protected virtual void Process(OperandProperty operand){
        }

        protected virtual void Process(OperandValue operand){
        }

        protected virtual void Process(UnaryOperator theOperator){
        }

    }
}