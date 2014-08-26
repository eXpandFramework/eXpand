using System.Drawing;
using System.IO;
using AForge.Video;
using AForge.Video.FFMPEG;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using DevExpress.EasyTest.Framework.Loggers;
using ScreenCaptureStream = Xpand.Utils.Helpers.ScreenCaptureStream;

namespace Xpand.EasyTest.Commands{
    public class ScreenCaptureCommand:FilesCommand{
        public const string Name = "ScreenCapture";
        private static VideoFileWriter _writer;
        private string _fileName;
        private Size _size;
        private static ScreenCaptureStream _screenCaptureStream;

        public static void Stop() {
            if (_screenCaptureStream != null) 
                _screenCaptureStream.Stop();
            if (_writer != null){
                _writer.Close();
                _writer.Dispose();
            }
        }
        public override void ParseCommand(CommandCreationParam commandCreationParam){
            base.ParseCommand(commandCreationParam);
            _size = this.ParameterValue("Size", new Size(1024, 768));
            _fileName = Path.Combine(ScriptsPath, Logger.Instance.GetLogger<FileLogger>().CurrentTestLog.Name + ".avi");
            if (File.Exists(_fileName))
                File.Delete(_fileName);
        }

        protected override void InternalExecute(ICommandAdapter adapter){
            _screenCaptureStream = new ScreenCaptureStream(new Rectangle(new Point(0, 0), _size));
            _screenCaptureStream.NewFrame+=ScreenCaptureStreamOnNewFrame;
            
            _writer = new VideoFileWriter();
            _writer.Open(_fileName, _size.Width, _size.Height,this.ParameterValue("FrameRate",3),VideoCodec.Default);
            _screenCaptureStream.Start();
        }

        private void ScreenCaptureStreamOnNewFrame(object sender, NewFrameEventArgs e){
            _writer.WriteVideoFrame(e.Frame);
        }
    }
}