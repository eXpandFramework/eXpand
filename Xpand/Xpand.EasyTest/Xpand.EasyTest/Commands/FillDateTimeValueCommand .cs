using System;
using System.Globalization;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands {
    public class FillDateTimeValueCommand : Command{
        public const string Name = "FillDateTimeValue";
        protected override void InternalExecute(ICommandAdapter adapter) {
            var deltaDays = this.ParameterValue<int>("Days");
            var deltaHours = this.ParameterValue<int>("Hours");
            var deltaMinutes = this.ParameterValue<int>("Minutes");
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