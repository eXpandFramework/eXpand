using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

namespace eXpand.Xpo.DB {
    public class ObjectMerger
    {
        private readonly BaseStatement[] _selectStatements;

        public ObjectMerger(BaseStatement[] selectStatements)
        {
            _selectStatements = selectStatements;
        }

        public void Merge(int objectTypeToMerge, int objectType)
        {
            foreach (var selectStatement in _selectStatements)
            {
                var criteriaOperatorExtractor = new CriteriaOperatorExtractor();
                var binaryOperator = getOperator(objectTypeToMerge);
                var binaryOperator1 = getOperator(objectType);

                criteriaOperatorExtractor.Replace(ref selectStatement.Condition, binaryOperator.ToString(), new GroupOperator(GroupOperatorType.Or, binaryOperator1, binaryOperator));
            }

        }

        private CriteriaOperator getOperator(int toMerge) {
            return new BinaryOperator
            {
                                   LeftOperand =
                                       new QueryOperand(XPObject.Fields.ObjectType.PropertyName, "N0", DBColumnType.Int32),
                                   RightOperand = new ParameterValue {Value = toMerge}
                               };
        }
    }
}