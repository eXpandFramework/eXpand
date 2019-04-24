using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandCheckFieldValuesCommand : Command{
        public const string Name = "XpandCheckFieldValues";

        public static string GetFieldValue(ICommandAdapter adapter, string fieldName){
            return adapter.CreateTestControl("Field", fieldName).GetInterface<IControlReadOnlyText>().Text;
        }

        protected override void InternalExecute(ICommandAdapter adapter){
            var comparisionHelper = new MultiLineComparisionHelper();
            foreach (Parameter parameter in Parameters){
                string fieldValue = CheckFieldValuesCommand.GetFieldValue(adapter, parameter.Name);
                string errorMessage = comparisionHelper.Compare("CheckFieldValues", parameter, fieldValue, "field value");
                if (!string.IsNullOrEmpty(errorMessage))
                    throw new TestException(errorMessage, parameter.PositionInScript);
            }
        }
    }
}