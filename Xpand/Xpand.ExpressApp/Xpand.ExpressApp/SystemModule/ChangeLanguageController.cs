using System.Globalization;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.SystemModule {
    public class ChangeLanguageController : WindowController {
        private readonly SingleChoiceAction _chooseLanguage;
        private readonly SingleChoiceAction _chooseFormattingCulture;

        public ChangeLanguageController() {
            TargetWindowType=WindowType.Main;
            _chooseLanguage = new SingleChoiceAction(this, "ChooseLanguage", PredefinedCategory.Tools);
            _chooseLanguage.Execute+=ChooseLanguage_Execute;
            _chooseFormattingCulture = new SingleChoiceAction(this, "ChooseFormattingCulture", PredefinedCategory.Tools);
            _chooseFormattingCulture.Execute+=ChooseFormattingCulture_Execute;
            var defaultCulture = CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
            var defaultFormattingCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            _chooseLanguage.Items.Add(new ChoiceActionItem(string.Format("Default ({0})",defaultCulture), defaultCulture));
            _chooseFormattingCulture.Items.Add(new ChoiceActionItem(string.Format("Default ({0})", defaultFormattingCulture), defaultFormattingCulture));
            var languages = new[]{"German (de)", "Spanish (es)", "Japanese (ja)", "Russian (ru)"};
            foreach (var language in languages) {
                _chooseFormattingCulture.Items.Add(new ChoiceActionItem(language, null));
                _chooseLanguage.Items.Add(new ChoiceActionItem(language, null));
            }
        }

        public SingleChoiceAction ChooseLanguage{
            get { return _chooseLanguage; }
        }

        public SingleChoiceAction ChooseFormattingCulture{
            get { return _chooseFormattingCulture; }
        }

        private void ChooseLanguage_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
            Application.SetLanguage(Regex.Match(e.SelectedChoiceActionItem.Caption, @"\(([^)]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value);
        }
        private void ChooseFormattingCulture_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
            Application.SetFormattingCulture(Regex.Match(e.SelectedChoiceActionItem.Caption, @"\(([^)]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value);
        }
    }
}
