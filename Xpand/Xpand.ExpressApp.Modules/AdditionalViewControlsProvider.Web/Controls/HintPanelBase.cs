using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxPanel;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Web.Controls {
    public class HintPanelBase : ASPxPanel, ISupportAppeareance {
        readonly ASPxLabel label;

        public HintPanelBase() {
            Paddings.PaddingBottom = 8;
            var innerHintPanel = new ASPxPanel();
            Controls.Add(innerHintPanel);

            label = new ASPxLabel { EncodeHtml = false };
            innerHintPanel.Controls.Add(label);
        }

        public ASPxLabel Label {
            get { return label; }
        }

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
            get {
                if (Font.Italic)
                    return FontStyle.Italic;
                if (Font.Bold)
                    return FontStyle.Bold;
                if (Font.Strikeout)
                    return FontStyle.Strikeout;
                return FontStyle.Regular;
            }
            set {
                if (value.HasValue) {
                    Font.Italic = value == FontStyle.Italic;
                    Font.Strikeout = value == FontStyle.Strikeout;
                    Font.Bold = value == FontStyle.Bold;
                }
            }
        }

        int? ISupportAppeareance.Height {
            get {
                if (Height != Unit.Empty)
                    return (int?)Height.Value;
                return null;
            }
            set {
                if (value.HasValue)
                    Height = Unit.Pixel(value.Value);
            }
        }

        int? ISupportAppeareance.FontSize {
            get {
                FontUnit fontUnit = Font.Size;
                return (int?)fontUnit.Unit.Value;
            }
            set {
                if (value.HasValue) {
                    Label.Font.Size = FontUnit.Point(value.Value);
                }
            }
        }

        string ISupportAppeareance.ImageName
        {
            get; set; 
        }

    }
}