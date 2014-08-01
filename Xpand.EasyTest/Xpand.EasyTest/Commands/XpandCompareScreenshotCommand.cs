using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandCompareScreenshotCommand : FilesCommand{
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
                    throw new CommandException(String.Format("Cannot find window {0}", windowNameParameter.Value), StartPosition);
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

        protected void SaveExpectedImage(Image img, string fileName){
            var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            var etalonImageStream = (MemoryStream) ImageHelper.GetStreamForImage(img, "image/png", 100L);
            etalonImageStream.WriteTo(fileStream);
            fileStream.Close();
        }

        protected void SaveDiffImage(Image img, string fileName){
            var view = new ApplicationView{
                fileNamePostfix = Path.GetFileNameWithoutExtension(fileName),
                fileExtension = "png",
                applicationViewStream = ImageHelper.GetStreamForImage(img, "image/png", 100L)
            };
            Logger.Instance.SaveApplicationViews(new[]{view});
        }

        protected void SaveActualImage(Image actualImage, string etalonFileName){
            string actualCopyFileNameWithoutExtension = Path.Combine(Path.GetDirectoryName(etalonFileName)+"",
                Path.GetFileNameWithoutExtension(etalonFileName)+"");
            string actualCopyFileName = string.Format("{0}.Actual{1}", actualCopyFileNameWithoutExtension,
                Path.GetExtension(etalonFileName));
            actualImage.Save(actualCopyFileName, ImageFormat.Png);
        }

        public static IntPtr GetRootWindow(int hWnd){
            int currentWindowHandle = hWnd;
            int result = currentWindowHandle;
            do{
                currentWindowHandle = GetParent(currentWindowHandle);
                if (currentWindowHandle != 0){
                    result = currentWindowHandle;
                }
            } while (currentWindowHandle != 0);
            return new IntPtr(result);
        }

        [DllImport("USER32.DLL", EntryPoint = "GetParent")]
        public static extern int GetParent(int hWnd);
    }
}