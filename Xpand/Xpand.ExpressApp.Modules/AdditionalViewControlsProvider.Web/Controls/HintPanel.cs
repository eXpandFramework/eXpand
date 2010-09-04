using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxPanel;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls {
    [AdditionalViewControl]
    public class HintPanel : ASPxPanel
    {
        private readonly ASPxLabel label;
        public HintPanel()
        {
            Paddings.PaddingBottom = 8;
            var innerHintPanel = new ASPxPanel();
            Controls.Add(innerHintPanel);

            label = new ASPxLabel { EncodeHtml = false };
            innerHintPanel.Controls.Add(label);
        }
        public ASPxLabel Label
        {
            get { return label; }
        }
    }
}