using System;
using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.Web.BusinessObjects{
    [DefaultClassOptions]
    [DefaultProperty("Title")]
    public class HorizontalDimension : BaseObject, IComparable{
        // Fields...
        private string _title;

        public HorizontalDimension(Session session)
            : base(session){
        }

        public string Title{
            get { return _title; }
            set { SetPropertyValue("Title", ref _title, value); }
        }

        public int CompareTo(object obj){
            var toCompare = obj as HorizontalDimension;
            if (toCompare != null)
                return String.Compare(Title, toCompare.Title, StringComparison.Ordinal);
            return 0;
        }
    }
}