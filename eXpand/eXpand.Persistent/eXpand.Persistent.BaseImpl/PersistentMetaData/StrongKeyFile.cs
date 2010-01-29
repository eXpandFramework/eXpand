using System;
using System.ComponentModel;
using System.IO;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [Persistent]
    [DefaultProperty("FileName")]
    public class StrongKeyFile : BaseObject, IFileData, IEmptyCheckable {
#if MediumTrust
		private int size;
		private string fileName = "";
		public int Size {
			get { return size; }
			set { size = value; }
		}
#else
        [Persistent] int size;
        string _fileName = "";

        public int Size {
            get { return size; }
        }
#endif

        public StrongKeyFile(Session session)
            : base(session)
        {
        }

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
            set { SetPropertyValue("FileName", ref _fileName, value); }
        }

        [Persistent, Delayed,
         ValueConverter(typeof (CompressionConverter)),
         MemberDesignTimeVisibility(false)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] Content {
            get { return GetDelayedPropertyValue<byte[]>("Content"); }
            set {
                int oldSize = size;
                size = value != null ? value.Length : 0;
                SetDelayedPropertyValue("Content", value);
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