using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Frames;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls
{
    public sealed class HintPanel:NotePanel8_1
    {
        public HintPanel()
        {
            BackColor = Color.LightGoldenrodYellow;
            Dock = DockStyle.Bottom;
            MaxRows = 25;
            TabIndex = 0;
            TabStop = false;
            MinimumSize = new Size(350, 33);
            Visible = false;
        }
    }
}