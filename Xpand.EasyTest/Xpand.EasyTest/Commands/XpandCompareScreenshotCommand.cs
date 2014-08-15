using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Xpand.Utils.Win32;

namespace Xpand.EasyTest.Commands{
    public class XpandCompareScreenshotCommand : CompareScreenshotCommand {
        public const string Name = "XpandCompareScreenshot";
        protected override void InternalExecute(ICommandAdapter adapter){
            ITestControl activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
            ExecuteAdditionalCommands(adapter);
            var windowHandle = GetWindowHandle(activeWindowControl);
            var testImage = GetTestImage(windowHandle);
            var filename = GetFilename(adapter);
            if (File.Exists(filename)){
                CompareAndSave(filename, testImage,adapter);
            }
            else{
                SaveActualImage(testImage, filename);
                throw new CommandException(String.Format("'{0}' master copy was not found", filename),StartPosition);
            }
        }

        private void CompareAndSave(string filename, Image testImage, ICommandAdapter adapter){
            Image etalonImage = Image.FromFile(filename);
            var maskFileNames = GetMaskFileNames(adapter);
            foreach (var maskFileName in maskFileNames){
                Image maskImage = (!string.IsNullOrEmpty(maskFileName)) ? Image.FromFile(maskFileName) : null;
                Image diffImage = ImageHelper.CompareImage(etalonImage, testImage, maskImage);
                if (diffImage != null) {
                    SaveAllImages(diffImage, filename, testImage);
                }    
            }
        }

        private void SaveAllImages(Image diffImage, string filename, Image testImage){
            SaveDiffImage(diffImage, filename);
            SaveActualImage(testImage, filename);
            throw new CommandException(
                String.Format("A screenshot of the active window differs from the '{0}' master copy",
                    filename), StartPosition);
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
            if (this.ParameterValue("KillFocus", true)){
                var hideCaretCommand = new KillFocusCommand();
                hideCaretCommand.Execute(adapter);
            }
            if (this.ParameterValue("HideCursor", true)){
                var hideCaretCommand = new HideCursorCommand();
                hideCaretCommand.Execute(adapter);
            }
            var parameter = Parameters["ActiveWindowSize"];
            string activeWindowSize = "1024x768";
            if (parameter!=null){
                activeWindowSize = parameter.Value;
            }
            var activeWindowSizeCommand = new SetActiveWindowSizeCommand();
            activeWindowSizeCommand.Parameters.MainParameter = new MainParameter(activeWindowSize);
            activeWindowSizeCommand.Execute(adapter);
        }

        private IEnumerable<string> GetMaskFileNames(ICommandAdapter adapter){
            var parameter = Parameters["Mask"];
            if (parameter != null){
                foreach (var p in GetMasks(adapter, parameter)) yield return p;
            }
            parameter = Parameters["XMask"];
            if (parameter != null){
                parameter.Value = string.Join(";", parameter.Value.Split(';').Select(s => "/regX/" + s));
                foreach (var p in GetMasks(adapter, parameter)) yield return p;
            }
        }

        private IEnumerable<string> GetMasks(ICommandAdapter adapter, Parameter parameter){
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