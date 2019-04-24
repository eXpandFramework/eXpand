using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class FieldIsVisibleCommand:Command{
        public const string Name = "FieldIsVisible";
        protected override void InternalExecute(ICommandAdapter adapter){
            foreach (var fieldName in Parameters["Fields"].Value.Split(';')){
                adapter.CreateTestControl("Field", fieldName);
                if (ExpectException)
                    throw new ExpectedExceptionCommandException(StartPosition);
            }
        }
    }
}