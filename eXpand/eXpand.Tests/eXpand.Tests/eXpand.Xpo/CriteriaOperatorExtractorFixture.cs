using System;
using DevExpress.Data.Filtering;
using eXpand.Xpo;
using eXpand.Xpo.Filtering;
using MbUnit.Framework;

namespace eXpand.Tests.eXpand.Xpo {
    [TestFixture]
    public class CriteriaOperatorExtractorFixture
    {
        [Test]
        public void ExtractGroupOperator()
        {
            int dayOfYear = DateTime.Now.DayOfYear;
            BinaryOperator binaryOperator2;
            CriteriaOperator groupOperator;
            BinaryOperator binaryOperator = getGroupOperator(out binaryOperator2, out groupOperator);
            var extractor = new CriteriaOperatorExtractor();
            extractor.Extract(groupOperator);
            Assert.AreEqual(binaryOperator, extractor.BinaryOperators[0]);
            Assert.AreEqual(binaryOperator2, extractor.BinaryOperators[1]);
        }

        private static BinaryOperator getGroupOperator(out BinaryOperator binaryOperator2,
                                                       out CriteriaOperator groupOperator)
        {
            var binaryOperator = new BinaryOperator("dfs", 324);
            binaryOperator2 = new BinaryOperator("sdfs", 3455);
            groupOperator = new GroupOperator(binaryOperator, binaryOperator2);
            return binaryOperator;
        }

        [Test]
        public void ExtractNestedGroupOperator()
        {
            BinaryOperator binaryOperator2;
            CriteriaOperator nestedGroupOperator;
            BinaryOperator binaryOperator1 = getGroupOperator(out binaryOperator2, out nestedGroupOperator);
            var groupOperator = new GroupOperator(nestedGroupOperator, binaryOperator1, binaryOperator2);
            CriteriaOperator containsOperator = new ContainsOperator("", groupOperator);

            var extractor = new CriteriaOperatorExtractor();
            extractor.Extract(containsOperator);

            Assert.AreEqual(4, extractor.BinaryOperators.Count);
            Assert.AreEqual(binaryOperator1, extractor.BinaryOperators[0]);
            Assert.AreEqual(binaryOperator2, extractor.BinaryOperators[1]);
            Assert.AreEqual(binaryOperator1, extractor.BinaryOperators[2]);
            Assert.AreEqual(binaryOperator2, extractor.BinaryOperators[3]);
        }



        [Test]
        public void ExtractNullOperator()
        {
            var nullOperator = new NullOperator("");

            CriteriaOperatorExtractor binaryOperatorExtractor = GetBinaryOperatorExtractor(nullOperator);

            Assert.AreEqual(nullOperator, binaryOperatorExtractor.NullOperators[0]);
        }

        private CriteriaOperatorExtractor GetBinaryOperatorExtractor(CriteriaOperator criteriaOperator)
        {
            var binaryOperatorExtractor = new CriteriaOperatorExtractor();
            binaryOperatorExtractor.Extract(criteriaOperator);
            return binaryOperatorExtractor;
        }

        [Test]
        public void ExtractGroupedNullOperator()
        {
            var nullOperator1 = new NullOperator();
            var nullOperator2 = new NullOperator();
            var groupOperator = new GroupOperator(nullOperator1, nullOperator2);

            CriteriaOperatorExtractor binaryOperatorExtractor = GetBinaryOperatorExtractor(groupOperator);

            Assert.AreEqual(nullOperator1, binaryOperatorExtractor.NullOperators[0]);
            Assert.AreEqual(nullOperator2, binaryOperatorExtractor.NullOperators[1]);
        }

        [Test]
        public void ExtractContainsNullOperator()
        {
            var nullOperator = new NullOperator();
            var containsOperator = new ContainsOperator("", nullOperator);

            CriteriaOperatorExtractor binaryOperatorExtractor = GetBinaryOperatorExtractor(containsOperator);

            Assert.AreEqual(nullOperator, binaryOperatorExtractor.NullOperators[0]);
        }

        [Test]
        public void ExtractNotOperator()
        {
            var notOperator = new NotOperator();

            CriteriaOperatorExtractor binaryOperatorExtractor = GetBinaryOperatorExtractor(notOperator);

            Assert.AreEqual(notOperator, binaryOperatorExtractor.NotOperators[0]);
        }

        [Test]
        public void ExtractContainsNotOperator()
        {
            var notOperator = new NotOperator();
            var containsOperator = new ContainsOperator("", notOperator);

            CriteriaOperatorExtractor binaryOperatorExtractor = GetBinaryOperatorExtractor(containsOperator);

            Assert.AreEqual(notOperator, binaryOperatorExtractor.NotOperators[0]);
        }

        [Test]
        public void ExtractGroupedNotOperator()
        {
            var notOperator1 = new NotOperator();
            var notOperator2 = new NotOperator();
            var groupOperator = new GroupOperator(notOperator2, notOperator1);

            CriteriaOperatorExtractor binaryOperatorExtractor = GetBinaryOperatorExtractor(groupOperator);

            Assert.AreEqual(notOperator1, binaryOperatorExtractor.NotOperators[0]);
            Assert.AreEqual(notOperator1, binaryOperatorExtractor.NotOperators[1]);
        }

        [Test]
        public void ExtractNotNullOperator()
        {
            var nullOperator = new NullOperator();
            var notOperator = new NotOperator(nullOperator);

            CriteriaOperatorExtractor binaryOperatorExtractor = GetBinaryOperatorExtractor(notOperator);

            Assert.AreEqual(nullOperator, binaryOperatorExtractor.NullOperators[0]);
            Assert.AreEqual(notOperator, binaryOperatorExtractor.NotOperators[0]);
        }

        [Test]
        public void ExtractNotBinaryOperator()
        {
            var binaryOperator = new BinaryOperator();
            var notOperator = new NotOperator(binaryOperator);

            CriteriaOperatorExtractor binaryOperatorExtractor = GetBinaryOperatorExtractor(notOperator);

            Assert.AreEqual(binaryOperator, binaryOperatorExtractor.BinaryOperators[0]);
            Assert.AreEqual(notOperator, binaryOperatorExtractor.NotOperators[0]);
        }

        [Test]
        public void ExtractMethodName()
        {
            var unaryOperator = new UnaryOperator(UnaryOperatorType.IsNull, "");

            CriteriaOperatorExtractor binaryOperatorExtractor = GetBinaryOperatorExtractor(unaryOperator);

            Assert.AreEqual(unaryOperator, binaryOperatorExtractor.UnaryOperators[0]);
        }

        [Test]
        public void RemoveRootOperator()
        {
            CriteriaOperator unaryOperator = new UnaryOperator(UnaryOperatorType.IsNull, "prop2");

            var binaryOperatorExtractor = new CriteriaOperatorExtractor();
            binaryOperatorExtractor.Remove(ref unaryOperator, unaryOperator.ToString());

            Assert.IsNull(unaryOperator);
        }

        [Test]
        public void RemoveNestedOperator()
        {
            var unaryOperator = new UnaryOperator(UnaryOperatorType.IsNull, "prop2");
            CriteriaOperator groupOperator = new GroupOperator(new BinaryOperator("pro1", 1), unaryOperator);

            var binaryOperatorExtractor = new CriteriaOperatorExtractor();
            binaryOperatorExtractor.Remove(ref groupOperator, unaryOperator.ToString());

            Assert.AreEqual(new BinaryOperator("pro1", 1).ToString(), groupOperator.ToString());
        }

        [Test]
        public void ReplaceRootOperator()
        {
            CriteriaOperator unaryOperator = new UnaryOperator(UnaryOperatorType.IsNull, "prop2");

            var binaryOperatorExtractor = new CriteriaOperatorExtractor();
            var replaceOperator = new BinaryOperator("pr", 1);
            binaryOperatorExtractor.Replace(ref unaryOperator, unaryOperator.ToString(), replaceOperator);

            Assert.AreEqual(unaryOperator, replaceOperator);
        }

        [Test]
        public void ReplaceNestedOperator()
        {
            var unaryOperator = new UnaryOperator(UnaryOperatorType.BitwiseNot, "pro");
            CriteriaOperator criteriaOperator = new GroupOperator(new BinaryOperator(), unaryOperator);

            var binaryOperatorExtractor = new CriteriaOperatorExtractor();
            var notOperator = new NotOperator();
            binaryOperatorExtractor.Replace(ref criteriaOperator, unaryOperator.ToString(), notOperator);

            Assert.AreEqual(((GroupOperator) criteriaOperator).Operands[1], notOperator);
        }
    }
}