using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Win.Controls
{
    [AdditionalViewControl]
    public  class HintPanel:NotePanel8_1,ISupportAppeareance
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

        Color? ISupportAppeareance.BackColor {
            get { return BackColor; }
            set {
                if (value.HasValue)
                    BackColor = value.Value;
            }
        }

        Color? ISupportAppeareance.ForeColor
        {
            get { return ForeColor; }
            set
            {
                if (value.HasValue)
                    ForeColor = value.Value;
            }
        }

        FontStyle? ISupportAppeareance.FontStyle
        {
            get { return this.Font.Style; }
            set
            {
                if (value.HasValue)
                    Font = new Font(Font, value.Value);
            }
        }

        int? ISupportAppeareance.Height
        {
            get { return Height; }
            set
            {
                if (value.HasValue)
                    MinimumSize = new Size(Width, value.Value);
            }
        }
    }
}