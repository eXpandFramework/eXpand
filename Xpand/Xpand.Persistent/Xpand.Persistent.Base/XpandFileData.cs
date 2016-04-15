using System;
using System.ComponentModel;
using System.IO;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace Xpand.Persistent.Base {
    [Persistent]
    [DefaultProperty("FileName")]
    public class XpandFileData : XpandBaseCustomObject, IFileData, IEmptyCheckable {
#if MediumTrust
		private int _size;
		private string fileName = "";
		public int Size {
			get { return _size; }
			set { _size = value; }
		}
#else
        [Persistent]
        private int _size;
        private string _fileName = "";
        public int Size {
            get { return _size; }
        }
#endif
        public XpandFileData(Session session) : base(session) { }
        public void LoadFromStream(string fileName, Stream stream) {
            Guard.ArgumentNotNull(stream, "stream");
            Guard.ArgumentNotNullOrEmpty(fileName, "fileName");
            FileName = fileName;
            var bytes = new byte[stream.Length];
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
            get { return _fileName; }
            set {
                SetPropertyValue("FileName", ref _fileName, value);
            }
        }
        [Persistent, Delayed,
         ValueConverter(typeof(CompressionConverter)),
         MemberDesignTimeVisibility(false)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] Content {
            get { return GetDelayedPropertyValue<byte[]>("Content"); }
            set {
                int oldSize = _size;
                _size = value != null ? value.Length : 0;
                SetDelayedPropertyValue("Content", value);
                OnChanged("Size", oldSize, _size);
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