using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Editors;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using System;
using DevExpress.ExpressApp.Utils;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls {
    public sealed class WarningPanel : NotePanel8_1, ISupportLayoutManager, ISupportAppeareance, IAdditionalViewControl {
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

        public object LayoutItem { get; set; }

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

        string ISupportAppeareance.ImageName
        {
            get { return null; }
            set
            {
                Image image = null;
                if (!String.IsNullOrEmpty(value))
                    image = ImageLoader.Instance.GetImageInfo(value).Image;
                if (image != null)
                    ArrowImage = image;
            }
        }

        public IAdditionalViewControlsRule Rule { get; set; }
    }
}