using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.Persistent.Base.ExceptionHandling;
using eXpand.Persistent.Base.General.ValueConverters;

namespace eXpand.Persistent.BaseImpl.ExceptionHandling
{
    
    [HideFromNewMenu]
    [DeferredDeletion(false)]
    public class ExceptionObject : XPLiteObject, IExceptionObject
    {

        private int _iD;
        #region Members
        string _application;
        string _computerName;
        DateTime _date;
        string _fullException;
        string _message;
        string _threadId;
        TimeSpan _time;
        User _user;
        string _windowsId;
        #endregion
        #region Constructor
        public ExceptionObject(Session session)
            : base(session)
        {
        }

        [Browsable(false)]
        [Key(true)]
        public int ID
        {
            get { return _iD; }
            set { SetPropertyValue("ID", ref _iD, value); }
        }
        #endregion
        #region Properties
        public DateTime Date {
            get { return _date; }
            set { SetPropertyValue("Date", ref _date, value); }
        }
        public TimeSpan Time
        {
            get { return _time; }
            set { SetPropertyValue("Time", ref _time, value); }
        }
        public User User
        {
            get { return _user; }
            set { SetPropertyValue("User", ref _user, value); }
        }

        IUser IExceptionObject.User {
            get { return User; }
            set { User=value as User; }
        }

        private string _tracingLastEntries;
        [Size(SizeAttribute.Unlimited)]
        public string TracingLastEntries
        {
            get { return _tracingLastEntries; }
            set { SetPropertyValue("TracingLastEntries", ref _tracingLastEntries, value); }
        }
        public string WindowsID
        {
            get { return _windowsId; }
            set { SetPropertyValue("WindowsID", ref _windowsId, value); }
        }

        public string ComputerName
        {
            get { return _computerName; }
            set { SetPropertyValue("ComputerName", ref _computerName, value); }
        }

        public string Application
        {
            get { return _application; }
            set { SetPropertyValue("Application", ref _application, value); }
        }

        public string ThreadID
        {
            get { return _threadId; }
            set { SetPropertyValue("ThreadID", ref _threadId, value); }
        }


        [Size(SizeAttribute.Unlimited)]
        public string Message
        {
            get { return _message; }
            set { SetPropertyValue("Message", ref _message, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string FullException
        {
            get { return _fullException; }
            set { SetPropertyValue("FullException", ref _fullException, value); }
        }

        [Size(SizeAttribute.Unlimited), Delayed, ValueConverter(typeof(ImageCompressionValueConverter))]
        public Image Screenshot
        {
            get { return GetDelayedPropertyValue<Image>("Screenshot"); }
            set { SetDelayedPropertyValue("Screenshot", value); }
        }

        IExceptionObject IExceptionObject.ParentExceptionObject {
            get { return ParentExceptionObject; }
            set { ParentExceptionObject = value as ExceptionObject; }
        }

        IList<IExceptionObject> IExceptionObject.InnerExceptionObjects {
            get { return new ListConverter<IExceptionObject,ExceptionObject>(InnerExceptionObjects); }
        }
        #endregion
        string ITreeNode.Name
        {
            get { return Message; }
        }
        private ExceptionObject _parentExceptionObject;

        [Association("ExceptionObject-ExceptionObjects")]
        public ExceptionObject ParentExceptionObject
        {
            get { return _parentExceptionObject; }
            set { SetPropertyValue("ParentExceptionObject", ref _parentExceptionObject, value); }
        }
        ITreeNode ITreeNode.Parent
        {
            get { return _parentExceptionObject; }
        }
        [Association("ExceptionObject-ExceptionObjects")]
        [Aggregated]
        public XPCollection<ExceptionObject> InnerExceptionObjects
        {
            get
            {
                return GetCollection<ExceptionObject>("InnerExceptionObjects");
            }
        }
        IBindingList ITreeNode.Children
        {
            get { return  InnerExceptionObjects; }
        }
    }
}
