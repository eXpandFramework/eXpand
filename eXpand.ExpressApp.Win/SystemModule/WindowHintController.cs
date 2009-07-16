using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.Utils.Frames;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class WindowHintController : BaseWindowController
    {
        private NotePanelEx bottomHintPanel;
        private NotePanelEx warningHintPanel;

        public WindowHintController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public NotePanelEx BottomHintPanel
        {
            get { return bottomHintPanel; }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Active[""] = false;
            Window.TemplateViewChanged += Window_TemplateViewChanged;
            Frame.GetController<DetailViewController>().Activated += (sender, e) =>
                                                                         {
                                                                             var view = ((DetailViewController)sender).View;
                                                                             view.CurrentObjectChanged +=(sender1, e1) => hidePanels();
                                                                         };
        }

        private void hidePanels()
        {
            if (bottomHintPanel!= null)
                bottomHintPanel.Visible = false;
            if (warningHintPanel!= null)
                warningHintPanel.Visible =
                    false;
        }

        private void Window_TemplateViewChanged(object sender, EventArgs e)
        {
            if (Frame.View!= null)
                PlaceHintPanels();
        }

        public event EventHandler<HintPanelReadyEventArgs> BottomHintPanelReady;
        public event EventHandler<HintPanelReadyEventArgs> WarningHintPanelReady;

        private void PlaceHintPanels()
        {
            bottomHintPanel = getHintPanel(DockStyle.Bottom);
            var controls = ((Control)((IViewSiteTemplate)Frame.Template).ViewSiteControl).Controls;
            controls.Add(bottomHintPanel);
            DoHintPanelReady(bottomHintPanel,BottomHintPanelReady);
            warningHintPanel = getHintPanel(DockStyle.Top);
            controls.Add(warningHintPanel);
            DoHintPanelReady(warningHintPanel, WarningHintPanelReady);
        }

        private NotePanelEx getHintPanel(DockStyle dockStyle)
        {
            return new NotePanelEx
                       {
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

        private void DoHintPanelReady(NotePanelEx hintPanel,EventHandler<HintPanelReadyEventArgs> eventHandler)
        {
            if (eventHandler != null)
                eventHandler(this, new HintPanelReadyEventArgs(hintPanel));
        }
    }

    public class HintPanelReadyEventArgs : EventArgs
    {
        private readonly NotePanelEx hintPanel;

        public HintPanelReadyEventArgs(NotePanelEx hintPanel)
        {
            this.hintPanel = hintPanel;
        }

        public NotePanelEx HintPanel
        {
            get { return hintPanel; }
        }
    }
}