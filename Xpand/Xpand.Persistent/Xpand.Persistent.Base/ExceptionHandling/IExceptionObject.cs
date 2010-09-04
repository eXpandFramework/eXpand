using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Persistent.Base.General;

namespace Xpand.Persistent.Base.ExceptionHandling {
    public interface IExceptionObject : ITreeNode {
        int ID { get; set; }
        DateTime Date { get; set; }
        TimeSpan Time { get; set; }
        Guid UserId { get; set; }
        string TracingLastEntries { get; set; }
        string WindowsID { get; set; }
        string ComputerName { get; set; }
        string Application { get; set; }
        string ThreadID { get; set; }
        string Message { get; set; }
        string FullException { get; set; }
        Image Screenshot { get; set; }
        IExceptionObject ParentExceptionObject { get; set; }
        IList<IExceptionObject> InnerExceptionObjects { get; }
    }
}