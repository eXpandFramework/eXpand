using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using Xpand.ExpressApp;
using Xpand.Persistent.Base.ExceptionHandling;
using Xpand.Persistent.Base.General;

//[assembly: Xpand.Xpo.DB.DataStore(typeof(ExceptionObject), "ExceptionHandling")]

namespace Xpand.Persistent.BaseImpl.ExceptionHandling {

    [HideFromNewMenu]
    [NavigationItem(true)]
    public class ExceptionObject : EFXafEntity, IExceptionObject {

        #region Constructor

        public ExceptionObject() {
            _innerExceptionObjects = new List<ExceptionObject>();
        }

        #endregion

        #region Properties

        private DateTime _date;

        public DateTime Date {
            get { return _date; }
            set { SetPropertyValue("Date", ref _date, value); }
        }

        private TimeSpan _time;

        public TimeSpan Time {
            get { return _time; }
            set { SetPropertyValue("Time", ref _time, value); }
        }


        private string _userId;

        public string UserId {
            get { return _userId; }
            set { SetPropertyValue("UserId", ref _userId, value); }
        }

        private string _tracingLastEntries;

        public string TracingLastEntries {
            get { return _tracingLastEntries; }
            set { SetPropertyValue("TracingLastEntries", ref _tracingLastEntries, value); }
        }

        private string _windowsId;

        public string WindowsID {
            get { return _windowsId; }
            set { SetPropertyValue("WindowsID", ref _windowsId, value); }
        }


        private string _computerName;

        public string ComputerName {
            get { return _computerName; }
            set { SetPropertyValue("ComputerName", ref _computerName, value); }
        }


        private string _application;

        public string Application {
            get { return _application; }
            set { SetPropertyValue("Application", ref _application, value); }
        }


        private string _threadId;

        public string ThreadID {
            get { return _threadId; }
            set { SetPropertyValue("ThreadID", ref _threadId, value); }
        }


        private string _message;

        public string Message {
            get { return _message; }
            set { SetPropertyValue("Message", ref _message, value); }
        }

        private string _fullException;

        public string FullException {
            get { return _fullException; }
            set { SetPropertyValue("FullException", ref _fullException, value); }
        }


        private byte[] _screenshot;

        [ImageEditor(ListViewImageEditorMode = ImageEditorMode.PictureEdit,
            DetailViewImageEditorMode = ImageEditorMode.PopupPictureEdit, ListViewImageEditorCustomHeight = 40)]
        public byte[] Screenshot {
            get { return _screenshot; }
            set { SetPropertyValue("Screenshot", ref _screenshot, value); }
        }

        IList<IExceptionObject> IExceptionObject.InnerExceptionObjects {
            get { return new ListConverter<IExceptionObject, ExceptionObject>(InnerExceptionObjects); }
        }

        #endregion

        string ITreeNode.Name {
            get { return Message; }
        }

        private ExceptionObject _parentExceptionObject;

        //[Association("ExceptionObject-ExceptionObjects")]
        public virtual ExceptionObject ParentExceptionObject {
            get { return _parentExceptionObject; }
            set { SetPropertyValue("ParentExceptionObject", ref _parentExceptionObject, value); }
        }

        ITreeNode ITreeNode.Parent {
            get { return _parentExceptionObject; }
        }

        private IList<ExceptionObject> _innerExceptionObjects;

        public virtual IList<ExceptionObject> InnerExceptionObjects {
            get { return _innerExceptionObjects; }
            set { SetPropertyValue("InnerExceptionObjects", ref _innerExceptionObjects, value); }
        }

        IBindingList ITreeNode.Children {
            get { return new BindingList<ExceptionObject>(InnerExceptionObjects); }
        }

        IExceptionObject IExceptionObject.ParentExceptionObject {
            get { return ParentExceptionObject; }
            set { ParentExceptionObject = value as ExceptionObject; }
        }


    }
}
