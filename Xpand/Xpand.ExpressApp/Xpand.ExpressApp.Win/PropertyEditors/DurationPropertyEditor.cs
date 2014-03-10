using System;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [PropertyEditor(typeof (TimeSpan), false)]
    public class DurationPropertyEditor : DXPropertyEditor {
        public DurationPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override object CreateControlCore() {
            return new StringEdit();
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);

            ((RepositoryItemStringEdit) item).Mask.MaskType = MaskType.RegEx;
            ((RepositoryItemStringEdit) item).Mask.EditMask
                = @"\s*((\d?\d?\d?\s*(d(ays?)?)))?\s*((\d?\d?\s*(h(ours)?)?))?\s*(\d?\d?\s*(m(in(utes)?)?)?)?";

            if (Control == null) return;

            Control.ShowToolTips = true;
            Control.ToolTip =
                @" Examples:  " + Environment.NewLine +
                @" 1d                     = 1 Day" + Environment.NewLine +
                @" 1 day                  = 1 Day" + Environment.NewLine +
                @" 2d 5h 45 m             = 2 Days 5 Hours 45 minutes" + Environment.NewLine +
                @" 2 days 4 hours 25 min  = 2 Days 4 Hours 25 minutes" + Environment.NewLine;
            Control.EditValueChanged += Control_EditValueChanged;
        }

        void Control_EditValueChanged(object sender, EventArgs e) {
            WriteValue();
            OnControlValueChanged();
        }

        protected override object GetControlValueCore() {
            return ParseTimeSpan(Control.Text);
        }

        protected override void ReadValueCore() {
            Control.EditValue = DecodeTimeSpan((TimeSpan) PropertyValue);
        }


        public static TimeSpan ParseTimeSpan(string s) {
            const string quantity = "quantity";
            const string unit = "unit";

            const string days = @"(d(ays?)?)";
            const string hours = @"(h((ours?)|(rs?))?)";
            const string minutes = @"(m((inutes?)|(ins?))?)";
            const string seconds = @"(s((econds?)|(ecs?))?)";

            var timeSpanRegex = new Regex(
                string.Format(@"\s*(?<{0}>\d+)\s*(?<{1}>({2}|{3}|{4}|{5}|\Z))",
                              quantity, unit, days, hours, minutes, seconds),
                RegexOptions.IgnoreCase);
            MatchCollection matches = timeSpanRegex.Matches(s);

            var ts = new TimeSpan();
            foreach (Match match in matches) {
                if (Regex.IsMatch(match.Groups[unit].Value, @"\A" + days)) {
                    ts = ts.Add(TimeSpan.FromDays(double.Parse(match.Groups[quantity].Value)));
                }
                else if (Regex.IsMatch(match.Groups[unit].Value, hours)) {
                    ts = ts.Add(TimeSpan.FromHours(double.Parse(match.Groups[quantity].Value)));
                }
                else if (Regex.IsMatch(match.Groups[unit].Value, minutes)) {
                    ts = ts.Add(TimeSpan.FromMinutes(double.Parse(match.Groups[quantity].Value)));
                }
                else if (Regex.IsMatch(match.Groups[unit].Value, seconds)) {
                    ts = ts.Add(TimeSpan.FromSeconds(double.Parse(match.Groups[quantity].Value)));
                }
                else {
                    // Quantity given but no unit, default to Hours
                    ts = ts.Add(TimeSpan.FromHours(double.Parse(match.Groups[quantity].Value)));
                }
            }
            return ts;
        }

        public static string DecodeTimeSpan(TimeSpan timeSpan) {
            string time = string.Empty;

            if (timeSpan.Days > 0)
                time = timeSpan.Days + " Days";


            if (timeSpan.Hours > 0)
                time += (time != string.Empty ? " " : "") + timeSpan.Hours + " Hours";


            if (timeSpan.Minutes > 0)
                time += (time != string.Empty ? " " : "") + timeSpan.Minutes + " Minutes";

            return time;
        }
    }
}