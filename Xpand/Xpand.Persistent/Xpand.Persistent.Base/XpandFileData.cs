using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Fasterflect;
using PropertyChanged;
using Xpand.Extensions.XAF.Attributes;


namespace Xpand.Persistent.Base {
    [DomainComponent]
    [DefaultProperty("FileName")]
    [AddINotifyPropertyChangedInterface]
    public class XpandFileData :  IFileData, IEmptyCheckable,ISupportFullName {
        private byte[] _content;
        public int Size { get; set; }

        
        public void LoadFromStream(string fileName, Stream stream) {
            Guard.ArgumentNotNull(stream, "stream");
            Guard.ArgumentNotNullOrEmpty(fileName, "fileName");
            FileName = fileName;
            var bytes = new byte[stream.Length];
            stream.ReadExactly(bytes);
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
        public string FileName{ get; set; }

        [Persistent, Delayed,
         ValueConverter(typeof(CompressionConverter)),
         MemberDesignTimeVisibility(false)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] Content {
            get => _content;
            set {
                Size = value?.Length ?? 0;
                _content = value;
                var methodInfo = GetType().Methods().First(info => info.Name.EndsWith("OnPropertyChanged"));
                methodInfo.Call(this,new PropertyChangedEventArgs("Size"));
            }
        }
        #region IEmptyCheckable Members
        [NonPersistent, MemberDesignTimeVisibility(false)]
        public bool IsEmpty => string.IsNullOrEmpty(FileName);

        #endregion

        [InvisibleInAllViews]
        [Size(SizeAttribute.Unlimited)]
        public string FullName{ get; set; }

        
    }
}