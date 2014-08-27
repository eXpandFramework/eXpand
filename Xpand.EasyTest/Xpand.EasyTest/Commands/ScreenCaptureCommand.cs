using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AForge.Video;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using DevExpress.EasyTest.Framework.Loggers;
using ScreenCaptureStream = Xpand.Utils.Helpers.ScreenCaptureStream;

namespace Xpand.EasyTest.Commands{
    public class ScreenCaptureCommand:FilesCommand{
        public const string Name = "ScreenCapture";
        private string _fileName;
        private Size _size;
        private static ScreenCaptureStream _screenCaptureStream;
        private long _index;

        public static void Stop() {
            if (_screenCaptureStream != null) 
                _screenCaptureStream.Stop();
        }

        public override void ParseCommand(CommandCreationParam commandCreationParam){
            base.ParseCommand(commandCreationParam);
            _size = this.ParameterValue("Size", new Size(1024, 768));
            var videoDir = ScriptsPath + @"\Images\ScreenCapture";
            if (Directory.Exists(videoDir))
                Directory.Delete(videoDir,true);
            Directory.CreateDirectory(videoDir);
            _fileName = Path.Combine(videoDir, Logger.Instance.GetLogger<FileLogger>().CurrentTestLog.Name);
        }

        protected override void InternalExecute(ICommandAdapter adapter){
            var frameInterval = this.ParameterValue("FrameInterval",100);
            _screenCaptureStream = new ScreenCaptureStream(new Rectangle(new Point(0, 0), _size), frameInterval);
            _screenCaptureStream.NewFrame+=ScreenCaptureStreamOnNewFrame;
            _screenCaptureStream.Start();
        }

        private void ScreenCaptureStreamOnNewFrame(object sender, NewFrameEventArgs e){
            _index++;
            var filename = _fileName+_index+".bmp";
            e.Frame.Save(filename,ImageFormat.Bmp);
        }
    }
}