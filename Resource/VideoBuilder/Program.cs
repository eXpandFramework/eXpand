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
            foreach (var path in paths.Where(s => !string.IsNullOrEmpty(s))){
                
                var files = Directory.GetFiles(path, "*.bmp");
                var images = GetImages(files).GroupBy(s => Regex.Replace(s, @"([^\d]*)([\d]*)([^\d]*)", "$1$3", RegexOptions.Singleline | RegexOptions.IgnoreCase));
                foreach (var grouping in images){
                    Console.WriteLine("Creating Videos for " + path);
                    var videoFileName = Path.Combine(path + "", grouping.Key + ".avi");
                    if (File.Exists(videoFileName))
                        File.Delete(videoFileName);
                    var videoFileWriter = new VideoFileWriter();
                    int frameRate = 4;
                    using (var bitmap = new Bitmap(Path.Combine(path, grouping.First()+".bmp"))){
                        videoFileWriter.Open(videoFileName, bitmap.Width, bitmap.Height, frameRate,VideoCodec.MPEG4);
                    }
                    WriteFranes(grouping, path, videoFileWriter, frameRate);
                    videoFileWriter.Close();
                    videoFileWriter.Dispose();
                    Console.WriteLine("Video for " + grouping.Key + " created");                    
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        private static void WriteFranes(IEnumerable<string> grouping, string path, VideoFileWriter videoFileWriter, int frameRate){
            int index = 0;
            var images = grouping.Select(s => Path.Combine(path, s + ".bmp")).ToArray();
            foreach (var image in images){
                index++;
                using (var bitmap = new Bitmap(image)){
                    var timeSpan = TimeSpan.FromSeconds((double)index/frameRate);
                    videoFileWriter.WriteVideoFrame(bitmap, timeSpan);
                }
            }
            Console.WriteLine("Found "+grouping.Count()+" images");
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
