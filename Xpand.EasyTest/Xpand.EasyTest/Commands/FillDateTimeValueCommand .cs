using System;
using System.Globalization;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands {

    public class FillDateTimeValueCommand : Command {
        private int GetIntegerParameterValue(string parameterName) {
            int result = 0;
            Parameter parameter = Parameters[parameterName];
            if (parameter != null) {
                if (!Int32.TryParse(parameter.Value, out result)) {
                    throw new CommandException(string.Format(
                        "'{0}' value is incorrect", parameterName), StartPosition);
                }
            }
            return result;
        }
        protected override void InternalExecute(ICommandAdapter adapter) {
            int deltaDays = GetIntegerParameterValue("Days");
            int deltaHours = GetIntegerParameterValue("Hours");
            int deltaMinutes = GetIntegerParameterValue("Minutes");
            string cultureName = Parameters["Culture"] != null ? Parameters["Culture"].Value : null;
            CultureInfo currentCulture =
                cultureName != null ? CultureInfo.GetCultureInfo(cultureName) : null;
            string fieldName = Parameters.MainParameter.Value;

            ITestControl testControl = adapter.CreateTestControl(TestControlType.Field, fieldName);
            DateTime dateTime = DateTime.Now.Add(new TimeSpan(deltaDays, deltaHours, deltaMinutes, 0));
            string dateTimeValue = currentCulture != null ?
                                       dateTime.ToString(currentCulture) : dateTime.ToString(CultureInfo.InvariantCulture);
            testControl.GetInterface<IControlText>().Text = dateTimeValue;
        }

    }
}