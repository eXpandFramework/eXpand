using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp;
using FeatureCenter.Module.Win;
using DevExpress.Utils.Frames;

namespace SecurityDemo.Module.Win {
    public class HideApplicationCaptionPanelController : ViewController {
        protected override void OnActivated() {
            base.OnActivated();
            if(Frame.Template is ICaptionPanelHolder) {
                ApplicationCaption8_1 captionPanel = ((ICaptionPanelHolder)Frame.Template).CaptionPanel;
                captionPanel.Visible = false;
            }
        }
        public HideApplicationCaptionPanelController() {
            this.TargetViewNesting = Nesting.Root;
        }
    }
}
