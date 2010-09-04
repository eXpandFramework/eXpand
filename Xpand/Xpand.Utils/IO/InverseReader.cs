using System;
using System.IO;
using System.Text;

namespace Xpand.Utils.IO
{
    [Obsolete("use ReverseLineReader", true)]
    public class InverseReader
    {
        private readonly FileStream fileStream;

        public InverseReader(string path)
            : this(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {

        }

        public InverseReader(FileStream fs)
        {
            fileStream = fs;
            fs.Seek(0, SeekOrigin.End);
        }

        public bool SOF
        {
            get { return fileStream.Position == 0; }
        }

        public string Readline()
        {
            var text = new byte[1];
            fileStream.Seek(0, SeekOrigin.Current);

            long position = fileStream.Position;
            bool trailingNewLine = fileStream.Length > 1;
            if (trailingNewLine)
            {
                var bytes = new byte[2];
                fileStream.Seek(-2, SeekOrigin.Current);
                fileStream.Read(bytes, 0, 2);
                if (Encoding.ASCII.GetString(bytes).Equals("\r\n"))
                {
                    //move it back
                    fileStream.Seek(-2, SeekOrigin.Current);
                    position = fileStream.Position;
                }
            }
            while (fileStream.Position > 0)
            {
                text.Initialize();
                //read one char
                fileStream.Read(text, 0, 1);
                string asciiText = Encoding.ASCII.GetString(text);
                //moveback to the charachter before
                fileStream.Seek(-2, SeekOrigin.Current);
                if (asciiText.Equals("\n"))
                {
                    fileStream.Read(text, 0, 1);
                    asciiText = Encoding.ASCII.GetString(text);
                    if (asciiText.Equals("\r"))
                    {
                        fileStream.Seek(1, SeekOrigin.Current);
                        break;
                    }
                }
            }

            int count = int.Parse((position - fileStream.Position).ToString());
            var line = new byte[count];
            fileStream.Read(line, 0, count);
            fileStream.Seek(-count, SeekOrigin.Current);
            return Encoding.ASCII.GetString(line);
        }


        public void Close()
        {
            fileStream.Close();
        }
    }
}