using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Xpand.EmailTemplateEngine {
    public class StreamEmailTemplateContentReader:IEmailTemplateContentReader {
        readonly Stream _stream;

        public StreamEmailTemplateContentReader(Stream stream) {
            _stream = stream;
        }

        public StreamEmailTemplateContentReader(string template)
            : this(new MemoryStream(Encoding.UTF8.GetBytes(template))) {
        }

        public string Read(string templateName, string suffix) {
            _stream.Position = 0;
            return new StreamReader(_stream).ReadToEnd();
        }
    }
    public class FileSystemEmailTemplateContentReader : IEmailTemplateContentReader {
        public FileSystemEmailTemplateContentReader()
            : this("templates", ".cshtml") {
        }

        public FileSystemEmailTemplateContentReader(string templateDirectory, string fileExtension) {
            Invariant.IsNotBlank(templateDirectory, "templateDirectory");

            if (!Path.IsPathRooted(templateDirectory)) {
                templateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateDirectory);
            }

            if (!Directory.Exists(templateDirectory)) {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, "\"{0}\" does not exist.",templateDirectory));
            }

            TemplateDirectory = templateDirectory;
            FileExtension = fileExtension;
        }

        protected string TemplateDirectory { get; private set; }

        protected string FileExtension { get; private set; }
        #region IEmailTemplateContentReader Members
        public string Read(string templateName, string suffix) {
            Invariant.IsNotBlank(templateName, "templateName");

            string content = string.Empty;
            string path = BuildPath(templateName, suffix);

            if (File.Exists(path)) {
                content = File.ReadAllText(path);
            }

            return content;
        }
        #endregion
        protected virtual string BuildPath(string templateName, string suffix) {
            string fileName = templateName;

            if (!string.IsNullOrWhiteSpace(suffix)) {
                fileName += "." + suffix;
            }

            if (!string.IsNullOrWhiteSpace(FileExtension)) {
                fileName += FileExtension;
            }

            string path = Path.Combine(TemplateDirectory, fileName);

            return path;
        }
    }
}