using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.FileAttachment.BusinessObjects {
    /// <summary>
    /// This class enables you to add soft links to real files instead of saving their contents to the database. It is intended for use in Windows Forms applications only.
    /// </summary>
    [DefaultProperty("FileName")]
    [DeferredDeletion(false), OptimisticLocking(false)]
    public class FileSystemLinkObject : XpandCustomObject, IFileData, IEmptyCheckable, ISupportFullName, IFileSystemObject {
        public FileSystemLinkObject(Session session) : base(session) { }
        #region IFileData Members
        [Size(260), Custom("AllowEdit", "False")]
        public string FileName {
            get { return GetPropertyValue<string>("FileName"); }
            set { SetPropertyValue("FileName", value); }
        }
        void IFileData.Clear() {
            Size = 0;
            FileName = string.Empty;
        }
        
        void IFileData.LoadFromStream(string fileName, Stream source) {
            Size = (int)source.Length;
            FileName = fileName;
        }
        
        void IFileData.SaveToStream(Stream destination) {
            try {
                if (destination == null)
                    Process.Start(FullName);
                else {
                    using (var fileStream = File.OpenRead(FullName)) {
                        fileStream.CopyTo(destination);
                    }
                }
            } catch (Exception exc) {
                throw new UserFriendlyException(exc);
            }
        }
        [Persistent]
        public int Size {
            get { return GetPropertyValue<int>("Size"); }
            private set { SetPropertyValue("Size", value); }
        }
        #endregion
        #region IEmptyCheckable Members
        public bool IsEmpty {
            get { return !File.Exists(FullName); }
        }
        #endregion
        #region ISupportFullName Members
        [Size(260), Custom("AllowEdit", "False")]
        public string FullName {
            get { return GetPropertyValue<string>("FullName"); }
            set { SetPropertyValue("FullName", value); }
        }
        #endregion
    }
}