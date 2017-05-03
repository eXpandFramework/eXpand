using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.Utils.Frames;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.SystemModule{
    public class WindowHintController : WindowController{
        private NotePanelEx _warningHintPanel;


        public NotePanelEx BottomHintPanel{ get; private set; }

        protected override void OnActivated(){
            base.OnActivated();
            Active[""] = false;
            Window.TemplateViewChanged += Window_TemplateViewChanged;
            Frame.GetController<ModificationsController>(controller => {
                controller.Activated += (sender, e) => {
                    var view = ((ModificationsController) sender).View;
                    view.CurrentObjectChanged += (sender1, e1) => HidePanels();
                };
            });
        }

        private void HidePanels(){
            if (BottomHintPanel != null)
                BottomHintPanel.Visible = false;
            if (_warningHintPanel != null)
                _warningHintPanel.Visible =
                    false;
        }

        private void Window_TemplateViewChanged(object sender, EventArgs e){
            if (Frame.View != null)
                PlaceHintPanels();
        }

        public event EventHandler<HintPanelReadyEventArgs> BottomHintPanelReady;
        public event EventHandler<HintPanelReadyEventArgs> WarningHintPanelReady;

        private void PlaceHintPanels(){
            BottomHintPanel = GetHintPanel(DockStyle.Bottom);
            var controls = ((Control) ((IViewSiteTemplate) Frame.Template).ViewSiteControl).Controls;
            controls.Add(BottomHintPanel);
            DoHintPanelReady(BottomHintPanel, BottomHintPanelReady);
            _warningHintPanel = GetHintPanel(DockStyle.Top);
            controls.Add(_warningHintPanel);
            DoHintPanelReady(_warningHintPanel, WarningHintPanelReady);
        }

        private NotePanelEx GetHintPanel(DockStyle dockStyle){
            return new NotePanelEx {
                Size = new Size(200, 200),
                BackColor = Color.LightGoldenrodYellow,
                Dock = dockStyle,
                MaxRows = 25,
                TabIndex = 0,
                TabStop = false,
                MinimumSize = new Size(350, 33),
                Visible = false,
                ArrowImage = null
            };
        }

        private void DoHintPanelReady(NotePanelEx hintPanel, EventHandler<HintPanelReadyEventArgs> eventHandler){
            eventHandler?.Invoke(this, new HintPanelReadyEventArgs(hintPanel));
        }
    }

    public class HintPanelReadyEventArgs : EventArgs{
        public HintPanelReadyEventArgs(NotePanelEx hintPanel){
            HintPanel = hintPanel;
        }

        public NotePanelEx HintPanel{ get; }
    }
}