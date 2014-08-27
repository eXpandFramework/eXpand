using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Xpand.Utils.Automation;
using Xpand.Utils.Helpers;
using Xpand.Utils.Win32;
using Image = System.Drawing.Image;

namespace Xpand.EasyTest.Commands{
    public class XpandCompareScreenshotCommand : CompareScreenshotCommand {
        public const string Name = "XpandCompareScreenshot";
        protected override void InternalExecute(ICommandAdapter adapter){
            var activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
            var windowHandle = GetWindowHandle(activeWindowControl);
            ExecuteAdditionalCommands(adapter);
            var testImage = GetTestImage(windowHandle);
            var filename = GetFilename(adapter);

            try{
                if (File.Exists(filename)){
                    CompareAndSave(filename, testImage,adapter);
                }
                else{
                    SaveActualImage(testImage, filename);
                    throw new CommandException(String.Format("'{0}' master copy was not found", filename),StartPosition);
                }
            }
            finally{
                if (this.ParameterValue("ToggleNavigation", true)){
                    ToggleNavigation(adapter);
                }
            }
        }

        private void CompareAndSave(string filename, Image testImage, ICommandAdapter adapter){
            var localImage = Image.FromFile(filename);
            var masks = GetMasks(adapter);
            if (!masks.Any()){
                var maskImage = CreateMaskImage(adapter);
                CompareAndSaveCore(filename, testImage, localImage,maskImage);
            }
            var height = this.ParameterValue("MaskHeight", 0);
            var width = this.ParameterValue("MaskWidth", 0);
            foreach (var mask in masks){
                var maskImage = mask;
                if (width>0&&height>0){
                    maskImage = (Bitmap) maskImage.ResizeRectangle(width, height);
                }
                CompareAndSaveCore(filename, testImage, localImage, maskImage);
            }
        }

        private Bitmap CreateMaskImage(ICommandAdapter adapter){
            Bitmap maskImage = null;
            var parameterName = adapter.IsWinAdapter() ? "WinMaskRectangle" : "WebMaskRectangle";
            var parameterValue = this.ParameterValue<string>(parameterName);
            if (!string.IsNullOrEmpty(parameterValue)){
                var maskSize = this.ParameterValue("MaskSize", new Size(1024, 768));
                maskImage = new Bitmap(maskSize.Width, maskSize.Height);
                using (var graphics = Graphics.FromImage(maskImage)) {
                    using (var solidBlackBrush = new SolidBrush(Color.Black)) {
                        graphics.FillRectangle(solidBlackBrush, new Rectangle(0, 0, maskImage.Width, maskImage.Height));
                    }
                    using (var solidBlackBrush = new SolidBrush(Color.White)) {
                        var pointValues = parameterValue.Split(';')[0].Split('x');
                        var location = new Point(Convert.ToInt32(pointValues[0]), Convert.ToInt32(pointValues[1]));
                        var sizeValues = parameterValue.Split(';')[1].Split('x');
                        var size = new Size(Convert.ToInt32(sizeValues[0]), Convert.ToInt32(sizeValues[1]));
                        var rectangle = new Rectangle(location, size);
                        graphics.FillRectangle(solidBlackBrush, rectangle);
                    }
                }
            }
            return maskImage;
        }

        private void CompareAndSaveCore(string filename, Image testImage, Image localImage, Image maskImage=null) {
            var differences = localImage.Differences(testImage, maskImage);
            var threshold = this.ParameterValue<byte>("ImageDiffThreshold", 3);
            var percentage = differences.Percentage(threshold);
            if (percentage > this.ParameterValue("ValidDiffPercentage", 10)){
                var diffImage = differences.DiffImage(true, true);
                SaveAllImages(diffImage, filename, testImage);
            }
        }

        private void SaveAllImages(Image diffImage, string filename, Image testImage){
            SaveDiffImage(diffImage, filename);
            SaveActualImage(testImage, filename);
            throw new CommandException(String.Format("A screenshot of the active window differs from the '{0}' master copy",filename), StartPosition);
        }

        private static Image GetTestImage(IntPtr windowHandle){
            Image testImage;
            try{
                testImage = ImageHelper.GetImage(windowHandle);
            }
            catch (Exception e){
                EasyTestTracer.Tracer.LogText("Exception:" + e.Message);
                testImage = new Bitmap(100, 100);
            }
            return testImage;
        }

        private string GetFilename(ICommandAdapter adapter){
            var filename = ScriptsPath + "\\Images\\" + Parameters.MainParameter.Value;
            return adapter.GetPlatformSuffixedPath(filename);
        }

        private IntPtr GetWindowHandle(ITestControl activeWindowControl){
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
                windowHandle = Win32Declares.WindowHandles.FindWindowByCaption(IntPtr.Zero, windowNameParameter.Value);
                if (windowHandle == IntPtr.Zero)
                    throw new CommandException(String.Format("Cannot find window {0}", windowNameParameter.Value), StartPosition);
            }
            return windowHandle;
        }

        private void ExecuteAdditionalCommands(ICommandAdapter adapter){
            if (this.ParameterValue("ToggleNavigation", true)) {
                ToggleNavigation(adapter);
            }

            var parameter = Parameters["ActiveWindowSize"];
            string activeWindowSize = "1024x768";
            if (parameter != null) {
                activeWindowSize = parameter.Value;
            }
            var activeWindowSizeCommand = new ResizeWindowCommand();
            activeWindowSizeCommand.Parameters.MainParameter = new MainParameter(activeWindowSize);
            activeWindowSizeCommand.Execute(adapter);

            if (this.ParameterValue(HideCursorCommand.Name, true)){
                var hideCaretCommand = new HideCursorCommand();
                hideCaretCommand.Execute(adapter);
            }

            if (this.ParameterValue(KillFocusCommand.Name, true)){
                var helperAutomation = new HelperAutomation();
                Win32Declares.Message.SendMessage(helperAutomation.GetFocusControlHandle(), Win32Declares.Message.EM_SETSEL, -1, 0);
                var hideCaretCommand = new KillFocusCommand();
                hideCaretCommand.Execute(adapter);
            }

            Wait(adapter, 1000);
        }

        private static void Wait(ICommandAdapter adapter, int interval){
            var sleepCommand = new SleepCommand();
            sleepCommand.Parameters.MainParameter = new MainParameter(interval.ToString(CultureInfo.InvariantCulture));
            sleepCommand.Execute(adapter);
        }

        private void ToggleNavigation(ICommandAdapter adapter){
            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("Toggle Navigation");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);
        }

        private IEnumerable<Bitmap> GetMasks(ICommandAdapter adapter){
            var parameter = Parameters["Mask"];
            if (parameter != null){
                foreach (var p in GetMaskFileNames(adapter, parameter)) yield return (Bitmap) Image.FromFile(p);
            }
            parameter = Parameters["XMask"];
            if (parameter != null){
                parameter.Value = string.Join(";", parameter.Value.Split(';').Select(s => "/regX/" + s));
                foreach (var p in GetMaskFileNames(adapter, parameter)) yield return (Bitmap) Image.FromFile(p);
            }
        }

        private IEnumerable<string> GetMaskFileNames(ICommandAdapter adapter, Parameter parameter){
            foreach (string maskPath in parameter.Value.Split(';')){
                string maskFileName;
                if (maskPath.StartsWith("/regX/")){
                    string path = Extensions.GetXpandPath(ScriptsPath);
                    maskFileName = Path.Combine(path,
                        @"Xpand.EasyTest\Resources\Masks\" + maskPath.TrimStart("/regX/".ToCharArray()));
                }
                else
                    maskFileName = ScriptsPath + "\\Images\\" + maskPath;
                yield return adapter.GetPlatformSuffixedPath(maskFileName);
            }
        }
    }
}