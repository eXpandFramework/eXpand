using DevExpress.ExpressApp;
using DevExpress.Utils.Frames;
using eXpand.ExpressApp.HintModule.Controllers;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.HintModule.Win.Controllers
{
    public partial class WinHintController : ViewController, IAdditionalInfoControlProvider
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View is DetailView|| !((ListView) View).IsNested(Frame))
            {
                var winShowAdditionalInfoController = Frame.GetController<WinShowAdditionalInfoController>();
                winShowAdditionalInfoController.Register(this);
            }
            
        }
        protected override void OnDeactivating()
        {
            Frame.GetController<WinShowAdditionalInfoController>().Unregister(this);
            base.OnDeactivating();
        }
        public object CreateControl()
        {
            var hintPanel = new NotePanel8_1
                                {
                                    BackColor = System.Drawing.Color.LightGoldenrodYellow,
                                    Dock = System.Windows.Forms.DockStyle.Bottom,
                                    MaxRows = 25,
                                    TabIndex = 0,
                                    TabStop = false,
                                    MinimumSize = new System.Drawing.Size(350, 33),
                                    Visible = false
                                };
            //hintPanel.Size = new System.Drawing.Size(350, 40);
            new WinHintDecorator(View, hintPanel);
            return hintPanel;
        }
    }
}