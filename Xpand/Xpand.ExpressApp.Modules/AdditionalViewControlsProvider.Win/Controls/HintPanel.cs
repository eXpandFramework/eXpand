using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls {
    public class HintPanel : NotePanel8_1, ISupportAppeareance, IAdditionalViewControl {
        public HintPanel() {
            BackColor = Color.LightGoldenrodYellow;
            Dock = DockStyle.Bottom;
            MaxRows = 25;
            TabIndex = 0;
            TabStop = false;
            MinimumSize = new Size(350, 33);
            Visible = false;
        }
        #region ISupportAppeareance Members
        Color? ISupportAppeareance.BackColor {
            get { return BackColor; }
            set {
                if (value.HasValue)
                    BackColor = value.Value;
            }
        }

        Color? ISupportAppeareance.ForeColor {
            get { return ForeColor; }
            set {
                if (value.HasValue)
                    ForeColor = value.Value;
            }
        }

        FontStyle? ISupportAppeareance.FontStyle {
            get { return Font.Style; }
            set {
                if (value.HasValue)
                    Font = new Font(Font, value.Value);
            }
        }


        int? ISupportAppeareance.Height {
            get { return Height; }
            set {
                if (value.HasValue)
                    MinimumSize = new Size(Width, value.Value);
            }
        }

        int? ISupportAppeareance.FontSize {
            get { return (int?) Font.Size; }
            set {
                if (value.HasValue) {
                    Font = new Font(Font.FontFamily, value.Value, Font.Style);
                }
            }
        }
        #endregion

        public IAdditionalViewControlsRule Rule { get; set; }
    }
}