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

namespace Xpand.EasyTest.Commands {
    public class XpandCompareScreenshotCommand : CompareScreenshotCommand {
        public const string Name = "XpandCompareScreenshot";
        readonly Size _defaultWindowSize = new Size(1024, 768);

        protected override void InternalExecute(ICommandAdapter adapter) {
            EasyTestTracer.Tracer.LogText("MainParameter=" + Parameters.MainParameter.Value);
            var activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
            var windowHandleInfo = GetWindowHandle(activeWindowControl);
            if (!windowHandleInfo.Value)
                ExecuteAdditionalCommands(adapter);

            var testImage = GetTestImage(windowHandleInfo.Key);
            var filename = GetFilename(adapter);

            try {
                if (File.Exists(filename)) {
                    CompareAndSave(filename, testImage, adapter);
                }
                else {
                    SaveActualImage(testImage, filename);
                    throw new CommandException(String.Format("'{0}' master copy was not found", filename), StartPosition);
                }
            }
            finally {
                if (!windowHandleInfo.Value && this.ParameterValue("ToggleNavigation", true)) {
                    ToggleNavigation(adapter);
                }
            }
        }

        private void CompareAndSave(string filename, Image testImage, ICommandAdapter adapter) {
            var threshold = this.ParameterValue<byte>("ImageDiffThreshold", 3);
            var localImage = Image.FromFile(filename);
            var masks = GetMasks(adapter).ToArray();
            var validDiffPercentace = this.ParameterValue("ValidDiffPercentage", 10);
            if (!masks.Any()) {
                var maskRectangle = GetMaskRectangle(adapter);
                if (maskRectangle!=Rectangle.Empty){
                    var isValidPercentage = IsValidPercentage(testImage, maskRectangle, threshold, validDiffPercentace,
                        localImage);
                    if (!isValidPercentage)
                        SaveImages(filename, testImage, threshold, localImage, maskRectangle);
                }
            }
            var height = this.ParameterValue("MaskHeight", 0);
            var width = this.ParameterValue("MaskWidth", 0);
            foreach (var mask in masks) {
                var maskImage = mask;
                if (width > 0 && height > 0) {
                    maskImage = (Bitmap)maskImage.ResizeRectangle(width, height);
                }
                var isValidPercentage = IsValidPercentage(testImage, maskImage, threshold, validDiffPercentace, localImage);
                if (!isValidPercentage)
                    SaveImages(filename, testImage, threshold, localImage, maskImage);
            }
        }


        private bool IsValidPercentage(Image testImage, Rectangle maskRectangle, byte threshold, int validDiffPercentace, Image localImage) {
            var differences = localImage.Differences(testImage, maskRectangle, threshold);
            return IsValidPercentageCore(validDiffPercentace, differences);
        }

        private bool IsValidPercentage(Image testImage, Bitmap maskImage, byte threshold, int validDiffPercentace, Image localImage) {
            var differences = localImage.Differences(testImage, maskImage, threshold);
            return IsValidPercentageCore(validDiffPercentace, differences);
        }

        private static bool IsValidPercentageCore(int validDiffPercentace, IEnumerable<KeyValuePair<byte[,], float>> differences) {
            return differences.FirstOrDefault(pair => pair.Value < validDiffPercentace).Key != null;
        }

        private void SaveImages(string filename, Image testImage, byte threshold, Image localImage, Rectangle maskRectangle) {
            var differences = localImage.Differences(testImage, maskRectangle, threshold);
            SaveImagesCore(differences, filename, testImage);
        }

        private void SaveImages(string filename, Image testImage, byte threshold, Image localImage, Bitmap maskImage) {
            var differences = localImage.Differences(testImage, maskImage, threshold);
            SaveImagesCore(differences, filename, testImage);
        }

        private void SaveImagesCore(IEnumerable<KeyValuePair<byte[,], float>> differences, string filename, Image testImage) {
            var valuePairs = differences.OrderBy(pair => pair.Value);
            var keyValuePair = valuePairs.First();
            var diffImage = keyValuePair.Key.DiffImage(true, true);
            SaveDiffImage(diffImage, filename);
            SaveActualImage(testImage, filename);
            throw new CommandException(String.Format("A screenshot of the active window differs {1}% from the '{0}' master copy", filename, Math.Round(keyValuePair.Value)), StartPosition);
        }

        private Rectangle GetMaskRectangle(ICommandAdapter adapter) {
            var parameterName = adapter.IsWinAdapter() ? "WinMaskRectangle" : "WebMaskRectangle";
            var rectangle = this.ParameterValue<Rectangle>(parameterName);
            return rectangle == Rectangle.Empty ? this.ParameterValue<Rectangle>("MaskRectangle") : rectangle;
        }

        private static Image GetTestImage(IntPtr windowHandle) {
            Image testImage;
            try {
                testImage = ImageHelper.GetImage(windowHandle);
                EasyTestTracer.Tracer.LogText("Captured image for window with handle {0} and title {1}", windowHandle, windowHandle.WindowText());
            }
            catch (Exception e) {
                EasyTestTracer.Tracer.LogText("Exception:" + e.Message);
                testImage = new Bitmap(100, 100);
            }
            return testImage;
        }

        private string GetFilename(ICommandAdapter adapter) {
            var filename = ScriptsPath + "\\Images\\" + Parameters.MainParameter.Value;
            return adapter.GetPlatformSuffixedPath(filename);
        }

        private KeyValuePair<IntPtr, bool> GetWindowHandle(ITestControl activeWindowControl) {
            var isCustom = false;
            var screenMainWindow = this.ParameterValue("ScreenMainWindow", false);
            IntPtr windowHandle = activeWindowControl.GetInterface<ITestWindow>().GetActiveWindowHandle();
            if (screenMainWindow) {
                windowHandle = GetRootWindow(windowHandle.ToInt32());
            }

            Parameter windowNameParameter = Parameters["WindowTitle"];
            if (windowNameParameter != null) {
                isCustom = true;
                windowHandle = Win32Declares.WindowHandles.FindWindowByCaption(IntPtr.Zero, windowNameParameter.Value);
                if (windowHandle == IntPtr.Zero)
                    throw new CommandException(String.Format("Cannot find window {0}", windowNameParameter.Value), StartPosition);
            }

            return new KeyValuePair<IntPtr, bool>(windowHandle, isCustom);
        }

        private void ExecuteAdditionalCommands(ICommandAdapter adapter) {
            if (this.ParameterValue("ToggleNavigation", true)) {
                ToggleNavigation(adapter);
            }


            var activeWindowSize = this.ParameterValue("ActiveWindowSize", _defaultWindowSize);
            var activeWindowSizeCommand = new ResizeWindowCommand();
            activeWindowSizeCommand.Parameters.MainParameter = new MainParameter(String.Format("{0}x{1}", activeWindowSize.Width, activeWindowSize.Height));
            activeWindowSizeCommand.Execute(adapter);

            if (this.ParameterValue(HideCursorCommand.Name, true)) {
                var hideCaretCommand = new HideCursorCommand();
                hideCaretCommand.Execute(adapter);
            }

            if (this.ParameterValue(KillFocusCommand.Name, true)) {
                var helperAutomation = new HelperAutomation();
                Win32Declares.Message.SendMessage(helperAutomation.GetFocusControlHandle(), Win32Declares.Message.EM_SETSEL, -1, 0);
                var hideCaretCommand = new KillFocusCommand();
                hideCaretCommand.Execute(adapter);
            }

            Wait(adapter, 1000);
        }

        private static void Wait(ICommandAdapter adapter, int interval) {
            var sleepCommand = new SleepCommand();
            sleepCommand.Parameters.MainParameter = new MainParameter(interval.ToString(CultureInfo.InvariantCulture));
            sleepCommand.Execute(adapter);
        }

        private void ToggleNavigation(ICommandAdapter adapter) {
            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("Toggle Navigation");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);
        }

        private IEnumerable<Bitmap> GetMasks(ICommandAdapter adapter) {
            var parameter = Parameters["Mask"];
            if (parameter != null) {
                foreach (var p in GetMaskFileNames(adapter, parameter)) yield return (Bitmap)Image.FromFile(p);
            }
            parameter = Parameters["XMask"];
            if (parameter != null) {
                parameter.Value = string.Join(";", parameter.Value.Split(';').Select(s => "/regX/" + s));
                foreach (var p in GetMaskFileNames(adapter, parameter)) yield return (Bitmap)Image.FromFile(p);
            }
        }

        private IEnumerable<string> GetMaskFileNames(ICommandAdapter adapter, Parameter parameter) {
            foreach (string maskPath in parameter.Value.Split(';')) {
                string maskFileName;
                if (maskPath.StartsWith("/regX/")) {
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