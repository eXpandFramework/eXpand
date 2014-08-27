using System;
using System.Drawing;
using System.IO;
using System.Linq;
using AForge.Video.FFMPEG;

namespace VideoBuilder {
    class Program {
        static void Main(string[] args){
            var files = Directory.GetFiles(args[0],"*.bmp");
            var firstFile = files.FirstOrDefault();
            if (firstFile != null){
                var videoFileWriter = new VideoFileWriter();
                using (var bitmap = new Bitmap(firstFile)){
                    var videoFileName = Path.Combine(Path.GetDirectoryName(firstFile)+"", Path.GetFileNameWithoutExtension(firstFile) + ".avi");
                    if (File.Exists(videoFileName))
                        File.Delete(videoFileName);
                    videoFileWriter.Open(videoFileName, bitmap.Width, bitmap.Height,3);
                }
                foreach (string file in files.OrderBy(File.GetCreationTime)){
                    using (var bitmap = new Bitmap(file)){
                        videoFileWriter.WriteVideoFrame(bitmap);
                    }
                }
                videoFileWriter.Close();
                videoFileWriter.Dispose();
                foreach (var file in files){
                    File.Delete(file);
                }
                Console.Write("Video for "+Path.GetFileNameWithoutExtension(firstFile)+" created");
            }
        }
    }
}
