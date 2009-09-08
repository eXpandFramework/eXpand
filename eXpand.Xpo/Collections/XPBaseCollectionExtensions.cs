using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Collections
{
    public static class XPBaseCollectionExtensions
    {
        public static CriteriaOperator EmptyCollectionCriteria =
            new BinaryOperator(new OperandValue(true), new OperandValue(false), BinaryOperatorType.Equal);
        public static int GetCount(this XPBaseCollection xpBaseCollection)
        {
            if (!xpBaseCollection.IsLoaded)
            {
                CriteriaOperator totalCriteria = CombineCriteria(xpBaseCollection.Criteria, xpBaseCollection.Filter);
                return GetObjectsCount(xpBaseCollection.GetObjectClassInfo(), totalCriteria,xpBaseCollection.Session);
            }
            return xpBaseCollection.Count;
        }
        public static int GetObjectsCount(XPClassInfo xpClassInfo, CriteriaOperator criteria,Session session)
        {
            if ((xpClassInfo != null) && xpClassInfo.IsPersistent)
                return (int) session.Evaluate(xpClassInfo, new AggregateOperand("", Aggregate.Count), criteria);
            return 0;
        }

        public static CriteriaOperator CombineCriteria(params CriteriaOperator[] criteriaOperators)
        {
            CriteriaOperator totalCriteria = null;
            var operators = new List<CriteriaOperator>();
            foreach (CriteriaOperator op in criteriaOperators)
                if (!ReferenceEquals(null, op))
                {
                    if (op.Equals(EmptyCollectionCriteria))
                        return EmptyCollectionCriteria;
                    operators.Add(op);
                }
            if (operators.Count != 0)
                totalCriteria = new GroupOperator(GroupOperatorType.And, operators);
            return totalCriteria;
        }
    }
}