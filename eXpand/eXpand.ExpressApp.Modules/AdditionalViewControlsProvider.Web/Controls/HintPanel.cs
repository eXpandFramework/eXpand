using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxPanel;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls
{
    public class HintPanel : ASPxPanel
    {
        private readonly ASPxLabel label;
        public HintPanel()
        {
            Paddings.PaddingBottom = 8;

            var innerHintPanel = new ASPxPanel();
            innerHintPanel.Paddings.Assign(new Paddings(8, 8, 8, 8));
            innerHintPanel.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            Controls.Add(innerHintPanel);

            label = new ASPxLabel();
            innerHintPanel.Controls.Add(label);
        }
        public ASPxLabel Label
        {
            get { return label; }
        }
    }
}