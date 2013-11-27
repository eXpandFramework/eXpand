using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.FileAttachment.BusinessObjects;

namespace FileAttachmentTester.Module.BusinessObjects {
[DefaultClassOptions]
  public partial class BusinessObject1 : DevExpress.Persistent.BaseImpl.BaseObject
  {

    public BusinessObject1(DevExpress.Xpo.Session session)
      : base(session)
    {
    }
    [DevExpress.Xpo.Aggregated, ExpandObjectMembers(ExpandObjectMembers.Never), NoForeignKey, ImmediatePostData]
    public FileSystemStoreObject File
    {
        get { return GetPropertyValue<FileSystemStoreObject>("File"); }
        set { SetPropertyValue("File", value); }
    }
  }
}
