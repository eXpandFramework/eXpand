using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [Persistent]
    [DefaultProperty("FileName")]
    public class StrongKeyFile : XpandBaseCustomObject, IFileData, IEmptyCheckable {
#if MediumTrust
		private int size;
		private string fileName = "";
		public int Size {
			get { return size; }
			set { size = value; }
		}
#else
        [Persistent]
        int _size;
        string _fileName = "";

        public int Size => _size;
#endif

        public StrongKeyFile(Session session)
            : base(session) {
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
            get => _fileName;
            set => SetPropertyValue("FileName", ref _fileName, value);
        }

        
        [Persistent, ValueConverter(typeof(CompressionConverter)),
         MemberDesignTimeVisibility(false)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Delayed(true)]
        [SuppressMessage("Design", "XAF0011:Implement a delayed property correctly.")]
        public byte[] Content {
            get => GetDelayedPropertyValue<byte[]>();
            set {
                int oldSize = _size;
                _size = value?.Length ?? 0;
                if (SetDelayedPropertyValue("Content", value)) {
                    OnChanged("Size", oldSize, _size);    
                }
            }
        }
        
        #region IEmptyCheckable Members
        [ MemberDesignTimeVisibility(false)]
        public bool IsEmpty => string.IsNullOrEmpty(FileName);

        #endregion
    }
}