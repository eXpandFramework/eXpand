using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Web;

namespace Xpand.ExpressApp.Web.ListEditors {
    internal class WebControlAppearanceAdapter : IAppearanceFormat, IAppearanceEnabled,
        IAppearanceVisibility {
        private readonly object _data;
        protected WebControl Control;

        public WebControlAppearanceAdapter(WebControl control, object data) {
            Control = control;
            _data = data;
        }

        #region IAppearanceFormat Members

        public FontStyle FontStyle {
            get{
                return Control != null ? RenderHelper.GetFontStyle(Control) : FontStyle.Regular;
            }
            set {
                if (Control != null) {
                    RenderHelper.SetFontStyle(Control, value);
                }
            }
        }

        public Color FontColor {
            get { return Control.ForeColor; }
            set {
                if (Control != null) {
                    Control.ForeColor = value;
                }
            }
        }

        public Color BackColor {
            get { return Control.BackColor; }
            set {
                if (Control != null) {
                    Control.BackColor = value;
                }
            }
        }

        public void ResetFontStyle() {
        }

        public void ResetFontColor() {
        }

        public void ResetBackColor() {
        }

        #endregion

        #region IAppearanceItem Members

        public object Data {
            get { return _data; }
        }

        #endregion

        public bool Enabled {
            get { return Control.Enabled; }
            set {
                if (Control != null) {
                    Control.Enabled = value;
                }
            }
        }

        public void ResetEnabled() {
            Control.Enabled = true;
        }

        public void ResetVisibility() {
            Control.Visible = true;
        }

        public ViewItemVisibility Visibility {
            get {
                if (!Control.Visible)
                    return ViewItemVisibility.Hide;
                bool controlsvisible = true;
                foreach (Control c in Control.Controls) {
                    if (!c.Visible)
                        controlsvisible = false;
                }
                if (!controlsvisible)
                    return ViewItemVisibility.ShowEmptySpace;

                return ViewItemVisibility.Show;
            }
            set {
                if (Control != null) {
                    Control.Visible = value != ViewItemVisibility.Hide;
                    foreach (Control c in Control.Controls) {
                        c.Visible = value != ViewItemVisibility.ShowEmptySpace;
                    }
                }
            }
        }
    }
}