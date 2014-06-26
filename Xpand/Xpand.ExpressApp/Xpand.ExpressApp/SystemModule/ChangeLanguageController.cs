using System.Globalization;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.SystemModule {
    public class ChangeLanguageController : WindowController {
        public ChangeLanguageController() {
            var chooseLanguage = new SingleChoiceAction(this, "ChooseLanguage", PredefinedCategory.Tools);
            chooseLanguage.Execute+=ChooseLanguage_Execute;
            var chooseFormattingCulture = new SingleChoiceAction(this, "ChooseFormattingCulture", PredefinedCategory.Tools);
            chooseFormattingCulture.Execute+=ChooseFormattingCulture_Execute;
            var defaultCulture = CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
            var defaultFormattingCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

            chooseLanguage.Items.Add(new ChoiceActionItem(string.Format("Default ({0})",defaultCulture), defaultCulture));
            chooseFormattingCulture.Items.Add(new ChoiceActionItem(string.Format("Default ({0})", defaultFormattingCulture), defaultFormattingCulture));
            var languages = new[]{"German (de)", "Spanish (es)", "Japanese (ja)", "Russian (ru)"};
            foreach (var language in languages) {
                chooseFormattingCulture.Items.Add(new ChoiceActionItem(language, null));
                chooseLanguage.Items.Add(new ChoiceActionItem(language, null));
            }
        }

        private void ChooseLanguage_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
            Application.SetLanguage(Regex.Match(e.SelectedChoiceActionItem.Caption, @"\(([^)]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value);
        }
        private void ChooseFormattingCulture_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
            Application.SetFormattingCulture(Regex.Match(e.SelectedChoiceActionItem.Caption, @"\(([^)]*)", RegexOptions.Singleline | RegexOptions.IgnoreCase).Groups[1].Value);
        }
    }
}
