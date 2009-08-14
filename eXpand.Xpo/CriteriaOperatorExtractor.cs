using System.Collections.Generic;
using DevExpress.Data.Filtering;

namespace eXpand.Xpo
{
    /// <summary>
    /// Extracts the binaryOperators out 
    /// </summary>
    public class CriteriaOperatorExtractor
    {
        private readonly List<BinaryOperator> binaryOperators = new List<BinaryOperator>();
        private readonly List<NotOperator> notOperators = new List<NotOperator>();
        private readonly List<NullOperator> nullOperators = new List<NullOperator>();
        private readonly List<UnaryOperator> unaryOperators = new List<UnaryOperator>();

        public List<UnaryOperator> UnaryOperators
        {
            get { return unaryOperators; }
        }

        public List<NotOperator> NotOperators
        {
            get { return notOperators; }
        }

        public List<NullOperator> NullOperators
        {
            get { return nullOperators; }
        }

        /// <summary>
        /// A list of the BinaryOperators of type <see cref="BinaryOperator"/> 
        /// </summary>
        public List<BinaryOperator> BinaryOperators
        {
            get { return binaryOperators; }
        }

        private CriteriaOperator Extract(CriteriaOperator criteriaOperator, string s)
        {
            return Extract(criteriaOperator, s, null);
        }

        public CriteriaOperator Extract(CriteriaOperator criteriaOperator)
        {
            return Extract(criteriaOperator, null);
        }




        public void Remove(ref CriteriaOperator criteriaOperator, string removeString)
        {
            if (criteriaOperator.ToString() == removeString)
            {
                criteriaOperator = null;
                return;
            }
            Extract(criteriaOperator, removeString);
        }

        public void Replace(ref CriteriaOperator criteriaOperator, string matchString, CriteriaOperator replaceOperator)
        {
            if (criteriaOperator.ToString() == matchString)
            {
                criteriaOperator = replaceOperator;
                return;
            }
            Extract(criteriaOperator, matchString, replaceOperator);
        }

        private CriteriaOperator Extract(CriteriaOperator criteriaOperator, string matchString,
                                         CriteriaOperator replaceOperator)
        {
            if (criteriaOperator is BinaryOperator)
            {
                var binaryOperator = (BinaryOperator) criteriaOperator;
                binaryOperators.Add(binaryOperator);

                return binaryOperator;
            }
            if (criteriaOperator is NullOperator)
            {
                nullOperators.Add((NullOperator) criteriaOperator);
                return criteriaOperator;
            }
            if (criteriaOperator is NotOperator)
            {
                var notOperator = (NotOperator) criteriaOperator;
                notOperators.Add(notOperator);
                Extract(notOperator.Operand);
            }
            else if (criteriaOperator is UnaryOperator)
            {
                unaryOperators.Add((UnaryOperator) criteriaOperator);
                return criteriaOperator;
            }

            else if (criteriaOperator is GroupOperator)
            {
                var groupOperator = (GroupOperator) criteriaOperator;
                CriteriaOperatorCollection operands = groupOperator.Operands;
                var indexesToremove = new List<int>();
                for (int i = 0; i < operands.Count; i++)
                {
                    CriteriaOperator operand = operands[i];
                    if (operand.ToString() == matchString)
                    {
                        if (replaceOperator == null)
                            indexesToremove.Add(i);
                        else
                            operands[i] = replaceOperator;
                    }
                    else
                    {
                        CriteriaOperator extract = Extract(operand);
                        operands.RemoveAt(i);
                        operands.Insert(i, extract);
                    }
                }
                foreach (int i in indexesToremove)
                    operands.RemoveAt(i);
            }
            else if (criteriaOperator is ContainsOperator)
            {
                var containsOperator = (ContainsOperator) criteriaOperator;
                //                containsOperator.Operand=Extract(containsOperator.Operand);
                CriteriaOperator condition = containsOperator.Condition;
                Extract(condition);
            }
            return criteriaOperator;
        }
    }
}