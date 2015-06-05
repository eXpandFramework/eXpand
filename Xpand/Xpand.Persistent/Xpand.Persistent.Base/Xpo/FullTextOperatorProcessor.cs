using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.Xpo.CustomFunctions;
using Xpand.Xpo.Filtering;

namespace Xpand.Persistent.Base.Xpo{
    public class FullTextOperatorProcessor : IClientCriteriaVisitor<CriteriaOperator>, IQueryCriteriaVisitor<CriteriaOperator> {
        readonly IEnumerable<XPMemberInfo> _memberInfos;

        public FullTextOperatorProcessor(IEnumerable<XPMemberInfo> memberInfos){
            _memberInfos = memberInfos;
        }

        public static object Process(CriteriaOperator op, IEnumerable<XPMemberInfo> memberInfos) {
            return op.Accept(new FullTextOperatorProcessor(memberInfos)) ;
        }

        public CriteriaOperator Visit(BetweenOperator theOperator){
            return (CriteriaOperator) (IsFullIndexed(theOperator.TestExpression)
                ? (object) new FunctionOperator(FullTextContainsFunction.FunctionName, theOperator.GetOperators())
                : theOperator);
        }

        CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BinaryOperator theOperator){
            return (CriteriaOperator) (IsFullIndexed(theOperator.LeftOperand)
                ? (object) new FunctionOperator(FullTextContainsFunction.FunctionName, theOperator.GetOperators())
                : theOperator);
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
            return _memberInfos.Any(info => (info.MappingField != null && info.MappingField == name)||(info.MappingField==null&&info.Name==name));
        }

        public CriteriaOperator Visit(UnaryOperator theOperator){
            return theOperator;
        }

        public CriteriaOperator Visit(InOperator theOperator){
            return theOperator;
        }

        CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(GroupOperator theOperator) {
            var criteriaOperators = theOperator.Operands.Select(@operator => @operator.Accept(this)).ToList();
            theOperator.Operands.Clear();
            theOperator.Operands.AddRange(criteriaOperators);
            return theOperator;
        }

        public CriteriaOperator Visit(OperandValue theOperand){
            return theOperand;
        }

        CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(FunctionOperator theOperator){
            if (theOperator.OperatorType != FunctionOperatorType.Custom){
                if (IsFullIndexed(theOperator.Operands[0])){
                    theOperator.OperatorType = FunctionOperatorType.Custom;
                    theOperator.Operands.Insert(0, new OperandValue(FullTextContainsFunction.FunctionName));
                }
            }
            return theOperator;
        }

        CriteriaOperator IQueryCriteriaVisitor<CriteriaOperator>.Visit(QueryOperand theOperand){
            return theOperand;
        }

        public CriteriaOperator Visit(QuerySubQueryContainer theOperand){
            return theOperand;
        }

        public CriteriaOperator Visit(AggregateOperand theOperand){
            return theOperand;
        }

        public CriteriaOperator Visit(OperandProperty theOperand){
            return theOperand;
        }

        public CriteriaOperator Visit(JoinOperand theOperand){
            return theOperand;
        }
    }
}