using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using eXpand.Xpo.DB;
using MbUnit.Framework;

namespace eXpand.Tests.eXpand.ExpressApp
{
    [TestFixture]
    public class MergingObjects_Tests
    {
        [Test]
        public void When_Statement_Has_ObjectType_Condtion() {
            var selectStatement = new SelectStatement
            {
                Condition = new BinaryOperator
                {
                    LeftOperand =
                        new QueryOperand("ObjectType", "N0", DBColumnType.Int32),
                    RightOperand = new ParameterValue { Value = 14 }
                }
            };
            var baseStatements = new BaseStatement[]{selectStatement};
            var objectMerger = new ObjectMerger(baseStatements);
            
            objectMerger.Merge(14, 2);


            Assert.AreEqual("N0.{ObjectType,Int32} = 2 Or N0.{ObjectType,Int32} = 14", baseStatements[0].Condition.ToString());
        }
    }

}
