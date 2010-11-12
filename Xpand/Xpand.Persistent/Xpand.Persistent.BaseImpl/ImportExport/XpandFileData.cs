using System;
using System.ComponentModel;
using System.IO;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.ImportExport {
    [Persistent]
    [DefaultProperty("FileName")]
    public class XpandFileData : XpandCustomObject, IFileData, IEmptyCheckable {
#if MediumTrust
		private int size;
		private string fileName = "";
		public int Size {
			get { return size; }
			set { size = value; }
		}
#else
        [Persistent]
        private int size;
        private string fileName = "";
        public int Size {
            get { return size; }
        }
#endif
        public XpandFileData(Session session) : base(session) { }
        public void LoadFromStream(string fileName, Stream stream) {
            Guard.ArgumentNotNull(stream, "stream");
            Guard.ArgumentNotNullOrEmpty(fileName, "fileName");
            FileName = fileName;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            Content = bytes;
        }
        public void SaveToStream(Stream stream) {
            if (string.IsNullOrEmpty(FileName)) {
                throw new InvalidOperationException();
            }
            stream.Write(Content, 0, Size);
            stream.Flush();
        }
        public void Clear() {
            Content = null;
            FileName = String.Empty;
        }
        public override string ToString() {
            return FileName;
        }
        [Size(260)]
        public string FileName {
            get { return fileName; }
            set {
                SetPropertyValue<string>("FileName", ref fileName, value);
            }
        }
        [DevExpress.Xpo.Persistent, Delayed,
         ValueConverter(typeof(CompressionConverter)),
         MemberDesignTimeVisibility(false)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] Content {
            get { return GetDelayedPropertyValue<byte[]>("Content"); }
            set {
                int oldSize = size;
                if (value != null) {
                    size = value.Length;
                } else {
                    size = 0;
                }
                SetDelayedPropertyValue<byte[]>("Content", value);
                OnChanged("Size", oldSize, size);
            }
        }
        #region IEmptyCheckable Members
        [NonPersistent, MemberDesignTimeVisibility(false)]
        public bool IsEmpty {
            get { return string.IsNullOrEmpty(FileName); }
        }
        #endregion
    }
}