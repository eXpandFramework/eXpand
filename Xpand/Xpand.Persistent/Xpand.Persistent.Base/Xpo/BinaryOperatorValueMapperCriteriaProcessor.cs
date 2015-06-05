using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.Xpo{
    public class BinaryOperatorValueMapperCriteriaProcessor : CriteriaProcessorBase ,IClientCriteriaVisitor{
        readonly Dictionary<string,object> _dictionary;

        public BinaryOperatorValueMapperCriteriaProcessor(Dictionary<string, object> dictionary){
            _dictionary = dictionary;
        }

        void ICriteriaVisitor.Visit(BinaryOperator theOperator){
            var propertyName = ((OperandProperty) theOperator.LeftOperand).PropertyName;
            if (_dictionary.ContainsKey(propertyName)){
                ((OperandValue) theOperator.RightOperand).Value = _dictionary[propertyName];
            }
        }
    }
}