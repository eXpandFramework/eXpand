using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AForge.Video.FFMPEG;

namespace VideoBuilder {
    class Program {
        static void Main(string[] args){
            var paths = args[0].Split(';');
            foreach (var path in paths){
                Console.Write("Creating Videos for " + path);
                var files = Directory.GetFiles(path, "*.bmp");
                var images = GetImages(files).GroupBy(s => Regex.Replace(s, @"([^\d]*)([\d]*)([^\d]*)", "$1$3", RegexOptions.Singleline | RegexOptions.IgnoreCase));
                foreach (var grouping in images){
                    var videoFileName = Path.Combine(path + "", grouping.Key + ".avi");
                    if (File.Exists(videoFileName))
                        File.Delete(videoFileName);
                    var videoFileWriter = new VideoFileWriter();
                    const int frameRate = 3;
                    using (var bitmap = new Bitmap(Path.Combine(path, grouping.First()+".bmp"))){
                        videoFileWriter.Open(videoFileName, bitmap.Width, bitmap.Height, frameRate,VideoCodec.MPEG4);
                    }
                    WriteFranes(grouping, path, videoFileWriter, frameRate);
                    videoFileWriter.Close();
                    videoFileWriter.Dispose();
                    Console.WriteLine("Video for " + grouping.Key + " created");                    
                }
            }
        }

        private static void WriteFranes(IEnumerable<string> grouping, string path, VideoFileWriter videoFileWriter, int frameRate){
            int index = 0;
            foreach (var image in grouping.Select(s => Path.Combine(path, s + ".bmp"))){
                using (var bitmap = new Bitmap(image)){
                    videoFileWriter.WriteVideoFrame(bitmap, TimeSpan.FromSeconds((double)index/frameRate));
                }
                index++;
            }
        }

        private static IEnumerable<string> GetImages(IEnumerable<string> files){
            var list = new List<string>();
            list.AddRange(GetImagesCore(files, s => !s.Contains(".")));
            foreach (var platform in new []{"win","web"}){
                string platform1 = platform;
                list.AddRange(GetImagesCore(files, s => s.EndsWith("." + platform1)));
            }
            return list;
        }

        private static IEnumerable<string> GetImagesCore(IEnumerable<string> files, Func<string, bool> predicate){
            return files.Select(Path.GetFileNameWithoutExtension).Select(s => s.ToLowerInvariant()).Where(predicate).OrderBy(s => Convert.ToInt32(Regex.Match(s, @"[^\d]*([\d]*)[^\d]*").Groups[1].Value));
        }
    }
}
