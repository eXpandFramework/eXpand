using System;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using Fasterflect;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Model.VisibilityCalculators;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    public interface IModelMemberViewItemDuration:IModelMemberViewItem{
        [ModelBrowsable(typeof(DurationPropertyEditorVisibilityCalculator))]
        IModelDurationPropertySettings DurationSettings { get; }
    }

    public class DurationPropertyEditorVisibilityCalculator:EditorTypeVisibilityCalculator<DurationPropertyEditor,IModelMemberViewItem>{
    }

    public interface IModelDurationPropertySettings:IModelNode{
        DurationTotal DisplayTotal { get; set; }
    }

    public enum DurationTotal{
        None,
        Days,
        Hours,
        Minutes,
        Seconds,
        MilliSeconds
    }

    [PropertyEditor(typeof (TimeSpan), false)]
    public class DurationPropertyEditor : DXPropertyEditor,ISupportNotifiedMembers {
        

        public DurationPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model){
        }

        protected override object CreateControlCore(){
            return TotalsEnabled() ? (object) new DoubleEdit(EditMask, DisplayFormat) : new StringEdit();
        }

        protected override RepositoryItem CreateRepositoryItem(){
            if (TotalsEnabled())
                return new RepositoryItemDoubleEdit(EditMask, DisplayFormat);
            return base.CreateRepositoryItem();
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);

            if (!TotalsEnabled()){
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
                
            }
            else{
                ((RepositoryItemDoubleEdit) item).Init(EditMask, DisplayFormat);
            }
            if (Control != null) Control.EditValueChanged += Control_EditValueChanged;
        }

        private bool TotalsEnabled(){
            return ((IModelMemberViewItemDuration) Model).DurationSettings.DisplayTotal!=DurationTotal.None;
        }

        void Control_EditValueChanged(object sender, EventArgs e) {
            if (AllowEdit){
                WriteValue();
                OnControlValueChanged();
            }
        }

        protected override object GetControlValueCore() {
            return !TotalsEnabled()?ParseTimeSpan(Control.Text):ParseTimeSpanFromTotals(Control.Text);
        }

        private TimeSpan ParseTimeSpanFromTotals(string text){
            var displayTotal = ((IModelMemberViewItemDuration) Model).DurationSettings.DisplayTotal;
            var value = (double) text.Change(typeof (double));
            if (displayTotal == DurationTotal.Days)
                return TimeSpan.FromDays(value);
            if (displayTotal == DurationTotal.Hours)
                return TimeSpan.FromHours(value);
            if (displayTotal == DurationTotal.Minutes)
                return TimeSpan.FromMinutes(value);
            if (displayTotal == DurationTotal.Seconds)
                return TimeSpan.FromSeconds(value);
            if (displayTotal == DurationTotal.MilliSeconds)
                return TimeSpan.FromMilliseconds(value);
            throw new NotImplementedException();
        }

        protected override void ReadValueCore() {
            Control.EditValue = DecodeTimeSpan((TimeSpan?) PropertyValue);
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
                    ts = ts.Add(TimeSpan.FromHours(double.Parse(match.Groups[quantity].Value)));
                }
            }
            return ts;
        }

        public object DecodeTimeSpan(TimeSpan? value){
            if (!TotalsEnabled()){
                string time = string.Empty;
                if (value.HasValue){
                    TimeSpan timeSpan = value.Value;
                    if (timeSpan.Days > 0)
                        time = timeSpan.Days + " Days";
                    if (timeSpan.Hours > 0)
                        time += (time != string.Empty ? " " : "") + timeSpan.Hours + " Hours";
                    if (timeSpan.Minutes > 0)
                        time += (time != string.Empty ? " " : "") + timeSpan.Minutes + " Minutes";
                }
                return time;
            }
            return
                value.GetPropertyValue(
                    ((IModelMemberViewItemDuration) Model).DurationSettings.DisplayTotal.ToString());
        }

    }
}