using System;
using DevExpress.Web.ASPxPanel;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxEditors;
using System.Collections.Generic;


namespace DevExpress.ExpressApp.Web.Demos {
    public class HintPanel : ASPxPanel {
        private ASPxLabel label;
        public HintPanel() {
            Paddings.PaddingBottom = 8;

            ASPxPanel innerHintPanel = new ASPxPanel();
            innerHintPanel.Paddings.Assign(new Paddings(8, 8, 8, 8));
            innerHintPanel.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            Controls.Add(innerHintPanel);

            label = new ASPxLabel();
            innerHintPanel.Controls.Add(label);
        }
        public ASPxLabel Label {
            get { return label; }
        }
    }
}
