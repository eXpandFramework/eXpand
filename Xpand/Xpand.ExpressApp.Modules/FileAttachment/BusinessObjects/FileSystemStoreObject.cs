using System;
using System.IO;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
using Xpand.Xpo;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.FileAttachment.BusinessObjects {
    public interface IFileSystemObject {
    }

    /// <summary>
    /// This class enables you to store uploaded files in a centralized file system location instead of the database. You can configure the file system store location via the static FileSystemDataModule.FileSystemStoreLocation property.
    /// </summary>
    [DefaultProperty("FileName")]
    [DeferredDeletion(false), OptimisticLocking(false)]
    public class FileSystemStoreObject : XpandCustomObject, IFileData, IEmptyCheckable, IFileSystemObject {
        private Stream tempSourceStream;
        private string tempFileName = string.Empty;
        private static readonly object syncRoot = new object();
        public FileSystemStoreObject(Session session) : base(session) { }
        public string RealFileName {
            get {
                if (!string.IsNullOrEmpty(FileName) && Oid != Guid.Empty)
                    return Path.Combine(XpandFileAttachmentsModule.FileSystemStoreLocation, string.Format("{0}-{1}", Oid, FileName));
                return null;
            }
        }
        protected virtual void SaveFileToStore() {
            if (!string.IsNullOrEmpty(RealFileName)) {
                try {
                    using (Stream destination = File.OpenWrite(RealFileName)) {
                        TempSourceStream.CopyStream(destination);
                        Size = (int)destination.Length;
                    }
                } catch (DirectoryNotFoundException exc) {
                    throw new UserFriendlyException(exc);
                }
            }
        }
        private void RemoveOldFileFromStore() {
            if (!string.IsNullOrEmpty(tempFileName) && tempFileName != RealFileName) {
                try {
                    File.Delete(tempFileName);
                    tempFileName = string.Empty;
                } catch (DirectoryNotFoundException exc) {
                    throw new UserFriendlyException(exc);
                }
            }
        }
        protected override void OnSaving() {
            base.OnSaving();
            Guard.ArgumentNotNullOrEmpty(XpandFileAttachmentsModule.FileSystemStoreLocation, "FileSystemStoreLocation");
            lock (syncRoot) {
                if (!Directory.Exists(XpandFileAttachmentsModule.FileSystemStoreLocation))
                    Directory.CreateDirectory(XpandFileAttachmentsModule.FileSystemStoreLocation);
            }
            SaveFileToStore();
            RemoveOldFileFromStore();
        }
        protected override void OnDeleting() {
            Clear();
            base.OnDeleting();
        }
        protected override void Invalidate(bool disposing) {
            if (disposing && TempSourceStream != null) {
                TempSourceStream.Close();
                TempSourceStream = null;
            }
            base.Invalidate(disposing);
        }
        #region IFileData Members
        public void Clear() {
            if (string.IsNullOrEmpty(tempFileName))
                tempFileName = RealFileName;
            FileName = string.Empty;
            Size = 0;
        }
        [Size(260)]
        public string FileName {
            get { return GetPropertyValue<string>("FileName"); }
            set { SetPropertyValue("FileName", value); }
        }
        [Browsable(false)]
        public Stream TempSourceStream {
            get { return tempSourceStream; }
            set {
                tempSourceStream = value;
                if (tempSourceStream is FileStream) {
                    try {
                        tempSourceStream = File.OpenRead(((FileStream)tempSourceStream).Name);
                    } catch (FileNotFoundException exc) {
                        throw new UserFriendlyException(exc);
                    }
                }
            }
        }
        
        void IFileData.LoadFromStream(string fileName, Stream source) {
            FileName = fileName;
            TempSourceStream = source;
            if (string.IsNullOrEmpty(tempFileName))
                tempFileName = RealFileName;
        }
        
        void IFileData.SaveToStream(Stream destination) {
            try {
                if (!string.IsNullOrEmpty(RealFileName)) {
                    if (destination == null)
                        System.Diagnostics.Process.Start(RealFileName);
                    else {
                        using (var fileStream = File.OpenRead(RealFileName)) {
                            fileStream.CopyStream(destination);
                        }
                    }
                } else if (TempSourceStream != null)
                    tempSourceStream.CopyStream(destination);
            } catch (DirectoryNotFoundException exc) {
                throw new UserFriendlyException(exc);
            } catch (FileNotFoundException exc) {
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
            get { return FileDataHelper.IsFileDataEmpty(this) || !File.Exists(RealFileName); }
        }
        #endregion
    }
}