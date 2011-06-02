using System;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [PropertyEditor(typeof (TimeSpan))]
    public class DurationPropertyEditor : DXPropertyEditor {
        public DurationPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override object CreateControlCore() {
            return new StringEdit();
        }

        const string Days = @"(D(ays?)?)";
        const string Hours = @"(H((ours?)|(rs?))?)";
        const string Minutes = @"(M((inutes?)|(ins?))?)";
        const string Seconds = @"(S((econds?)|(ecs?))?)";

        readonly string Mask =
            string.Format(@"\s*((\d?\d?\d?\s*{0}))?\s*((\d?\d?\s*{1}))?\s*((\d?\d?\s*{2}))?\s*"
                        , Days
                        , Hours
                        , Minutes);

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);

            ((RepositoryItemStringEdit) item).Mask.MaskType = MaskType.RegEx;
            ((RepositoryItemStringEdit)item).Mask.EditMask = Mask;

            if (Control == null) return;

            Control.ShowToolTips = true;
            Control.ToolTip =
                " Examples:  " + Environment.NewLine +
                " 1d                     = 1 Day" + Environment.NewLine +
                " 1 day                  = 1 Day" + Environment.NewLine +
                " 2d 5h 45 m             = 2 Days 5 Hours 45 minutes" + Environment.NewLine +
                " 2 days 4 hours 25 min  = 2 Days 4 Hours 25 minutes" + Environment.NewLine
                ;
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
            const string Quantity = "quantity";
            const string Unit = "unit";

            var timeSpanRegex = new Regex(
                string.Format(@"\s*(?<{0}>\d+)\s*(?<{1}>({2}|{3}|{4}|{5}|\Z))",
                              Quantity, Unit, Days, Hours, Minutes, Seconds),
                RegexOptions.IgnoreCase);
            MatchCollection matches = timeSpanRegex.Matches(s);

            var ts = new TimeSpan();
            foreach (Match match in matches) {
                if (Regex.IsMatch(match.Groups[Unit].Value, @"\A" + Days)) {
                    ts = ts.Add(TimeSpan.FromDays(double.Parse(match.Groups[Quantity].Value)));
                }
                else if (Regex.IsMatch(match.Groups[Unit].Value, Hours)) {
                    ts = ts.Add(TimeSpan.FromHours(double.Parse(match.Groups[Quantity].Value)));
                }
                else if (Regex.IsMatch(match.Groups[Unit].Value, Minutes)) {
                    ts = ts.Add(TimeSpan.FromMinutes(double.Parse(match.Groups[Quantity].Value)));
                }
                else if (Regex.IsMatch(match.Groups[Unit].Value, Seconds)) {
                    ts = ts.Add(TimeSpan.FromSeconds(double.Parse(match.Groups[Quantity].Value)));
                }
                else {
                    // Quantity given but no unit, default to Hours
                    ts = ts.Add(TimeSpan.FromHours(double.Parse(match.Groups[Quantity].Value)));
                }
            }
            return ts;
        }

        /// <summary>
        /// Convers timeSpan to a readable String like : 2 days 4 hours 25 minutes  
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns>Text</returns>
        public static string DecodeTimeSpan(TimeSpan timeSpan)
        {
            var time = string.Empty;

            if (timeSpan.Days > 0)
                time = timeSpan.Days + " Day" + (timeSpan.Days > 1 ? "s" : "");

            if (timeSpan.Hours > 0)
                time += (time != string.Empty ? " " : "") + timeSpan.Hours + " Hour"
                        + (timeSpan.Hours.ToString().ToCharArray().Last() != '1' ? "s" : "");

            if (timeSpan.Minutes > 0)
                time += (time != string.Empty ? " " : "") + timeSpan.Minutes + " Minute"
                    + (timeSpan.Minutes.ToString().ToCharArray().Last() != '1' ? "s" : "");

            if (timeSpan.Seconds > 0)
                time += (time != string.Empty ? " " : "") + timeSpan.Seconds + " Second"
                    + (timeSpan.Seconds.ToString().ToCharArray().Last() != '1' ? "s" : ""); ;

            return time;
        }
    }
}