using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.XtraLayout;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls {
    public interface ISupportLayoutManager {
        BaseLayoutItem LayoutItem { get; set; }
    }
    [AdditionalViewControl]
    public class WarningPanel:NotePanel8_1, ISupportLayoutManager {
        public WarningPanel() {
            BackColor = Color.LightGoldenrodYellow;
            Dock = DockStyle.Bottom;
            MaxRows = 5;
            TabIndex = 0;
            TabStop = false;
            MinimumSize = new Size(350, 13);
            Visible = false;
            ArrowImage = null;
        }

        public BaseLayoutItem LayoutItem { get; set; }
    }
}