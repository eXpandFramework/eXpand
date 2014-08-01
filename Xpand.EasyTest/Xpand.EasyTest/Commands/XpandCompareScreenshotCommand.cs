using System;
using System.Drawing;
using System.IO;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Xpand.Utils.Automation;
using Xpand.Utils.Win32;

namespace Xpand.EasyTest.Commands{
    public class XpandCompareScreenshotCommand : CompareScreenshotCommand {
        public const string Name = "XpandCompareScreenshot";
        protected override void InternalExecute(ICommandAdapter adapter){
            ITestControl activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
            string etalonFileName = ScriptsPath + "\\Images\\" + Parameters.MainParameter.Value;
            string maskFileName = Parameters["Mask"] != null
                ? ScriptsPath + "\\Images\\" + Parameters["Mask"].Value
                : null;

            bool screenMainWindow = false;
            if (Parameters["ScreenMainWindow"] != null){
                Boolean.TryParse(Parameters["ScreenMainWindow"].Value, out screenMainWindow);
            }


            IntPtr windowHandle = activeWindowControl.GetInterface<ITestWindow>().GetActiveWindowHandle();
            if (screenMainWindow){
                windowHandle = GetRootWindow(windowHandle.ToInt32());
            }

            Parameter windowNameParameter = Parameters["WindowTitle"];
            if (windowNameParameter != null){
                windowHandle = Win32.FindWindowByCaption(IntPtr.Zero, windowNameParameter.Value);
                if (windowHandle == IntPtr.Zero)
                    throw new CommandException(String.Format("Cannot find window {0}", windowNameParameter.Value),StartPosition);
            }


            Image testImage;
            try{

                testImage = ImageHelper.GetImage(windowHandle);
            }
            catch (Exception e){
                EasyTestTracer.Tracer.LogText("Exception:" + e.Message);
                testImage = new Bitmap(100, 100);
            }
            if (File.Exists(etalonFileName)){
                Image etalonImage = Image.FromFile(etalonFileName);
                Image maskImage = (!string.IsNullOrEmpty(maskFileName)) ? Image.FromFile(maskFileName) : null;
                Image diffImage = ImageHelper.CompareImage(etalonImage, testImage, maskImage);
                if (diffImage != null){
                    SaveDiffImage(diffImage, etalonFileName);
                    SaveActualImage(testImage, etalonFileName);
                    throw new CommandException(
                        String.Format("A screenshot of the active window differs from the '{0}' master copy",
                            etalonFileName), StartPosition);
                }
            }
            else{
                SaveActualImage(testImage, etalonFileName);
                //SaveExpectedImage(testImage, etalonFileName);
                throw new CommandException(String.Format("'{0}' master copy was not found", etalonFileName),
                    StartPosition);
            }
        }

    }
}