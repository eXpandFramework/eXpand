using System;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Win.SystemModule {
//    internal class MyClass:ViewController {
//        protected override void OnViewChanging(View view)
//        {
//            base.OnViewChanging(view);
//            view.CaptionChanged+=ViewOnCaptionChanged;
//        }
//
//        void ViewOnCaptionChanged(object sender, EventArgs eventArgs) {
//            Debug.Print("");
//        }
//    }
//    public partial class CustomizeWindowController : WindowController
//    {
//        WindowTemplateController controller;
//        public CustomizeWindowController()
//        {
//            TargetWindowType = WindowType.Main;
//            Activated += CustomizeWindowController_Activated;
//        }
//        void CustomizeWindowController_Activated(object sender, EventArgs e)
//        {
//            controller = Frame.GetController<WindowTemplateController>();
//            controller.CustomizeWindowCaption += controller_CustomizeWindowCaption;
//        }
//        void controller_CustomizeWindowCaption(object sender, CustomizeWindowCaptionEventArgs e)
//        {
//            e.WindowCaption.Text = "My Custom Caption";
//            
//        }
//    }

//    public class WindowCaptionController:WindowController {
//        public WindowCaptionController() {
//            TargetWindowType=WindowType.Main;
//        }
//        protected override void OnActivated()
//        {
//            base.OnActivated();
//            Frame.GetController<WindowTemplateController>().CustomizeWindowCaption+=OnCustomizeWindowCaption;
//        }
//
//        void OnCustomizeWindowCaption(object sender, CustomizeWindowCaptionEventArgs customizeWindowCaptionEventArgs) {
//            var fileVersionInfo = FileVersionInfo.GetVersionInfo(System.Windows.Forms.Application.ExecutablePath);
//            customizeWindowCaptionEventArgs.WindowCaption.Text = string.Format("{0} - Ver {1}.{2}.{3}",
//                                                                               Application.Title,
//                                                                               fileVersionInfo.ProductMajorPart,
//                                                                               fileVersionInfo.ProductMinorPart,
//                                                                               fileVersionInfo.FilePrivatePart); 
//        }
//
//    }
}