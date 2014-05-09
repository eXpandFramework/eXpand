using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo.CustomFunctions;
using Xpand.Xpo.Filtering;

namespace Xpand.Persistent.Base.Xpo{
    public class FullTextOperatorProcessor : IClientCriteriaVisitor, IQueryCriteriaVisitor {
        readonly List<XPMemberInfo> _memberInfos=new List<XPMemberInfo>();

        public FullTextOperatorProcessor(List<XPMemberInfo> memberInfos){
            _memberInfos = memberInfos;
        }

        public static object Process(CriteriaOperator op, List<XPMemberInfo> memberInfos) {
            return op.Accept(new FullTextOperatorProcessor(memberInfos)) ;
        }

        public object Visit(BetweenOperator theOperator){
            return IsFullIndexed(theOperator.TestExpression)
                ? (object) new FunctionOperator(FullTextContainsFunction.FunctionName, theOperator.GetOperators())
                : theOperator;
        }

        object ICriteriaVisitor.Visit(BinaryOperator theOperator){
            return IsFullIndexed(theOperator.LeftOperand)
                ? (object) new FunctionOperator(FullTextContainsFunction.FunctionName, theOperator.GetOperators())
                : theOperator;
        }

        private bool IsFullIndexed(CriteriaOperator theOperator){
            var queryOperand = theOperator as QueryOperand;
            if (!ReferenceEquals(queryOperand, null)){
                return IsFullIndexedCore(queryOperand.ColumnName);
            }
            var operandProperty = theOperator as OperandProperty;
            return !ReferenceEquals(operandProperty, null) && IsFullIndexedCore(operandProperty.PropertyName);
        }

        private bool IsFullIndexedCore(string name){
            return _memberInfos.Any(info => info.MappingField == name);
        }

        public object Visit(UnaryOperator theOperator){
            return theOperator;
        }

        public object Visit(InOperator theOperator){
            return theOperator;
        }

        object ICriteriaVisitor.Visit(GroupOperator theOperator) {
            var criteriaOperators = theOperator.Operands.Select(@operator => @operator.Accept(this)).Cast<CriteriaOperator>().ToList();
            theOperator.Operands.Clear();
            theOperator.Operands.AddRange(criteriaOperators);
            return theOperator;
        }

        public object Visit(OperandValue theOperand){
            return theOperand;
        }

        object ICriteriaVisitor.Visit(FunctionOperator theOperator){
            if (theOperator.OperatorType != FunctionOperatorType.Custom){
                if (IsFullIndexed(theOperator.Operands[0])){
                    theOperator.OperatorType = FunctionOperatorType.Custom;
                    theOperator.Operands.Insert(0, new OperandValue(FullTextContainsFunction.FunctionName));
                }
            }
            return theOperator;
        }

        object IQueryCriteriaVisitor.Visit(QueryOperand theOperand){
            return theOperand;
        }

        public object Visit(QuerySubQueryContainer theOperand){
            return theOperand;
        }

        public object Visit(AggregateOperand theOperand){
            return theOperand;
        }

        public object Visit(OperandProperty theOperand){
            return theOperand;
        }

        public object Visit(JoinOperand theOperand){
            return theOperand;
        }
    }
}