using System;
using System.ComponentModel;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.Web.BusinessObjects{
    [DefaultProperty("Title")]
    public class VerticalDimension : BaseObject, IComparable{
        // Fields...
        private string _title;

        public VerticalDimension(Session session)
            : base(session){
        }

        public string Title{
            get { return _title; }
            set { SetPropertyValue("Title", ref _title, value); }
        }

        public int CompareTo(object obj){
            var toCompare = obj as VerticalDimension;
            if (toCompare != null)
                return String.Compare(Title, toCompare.Title, StringComparison.Ordinal);
            return 0;
        }
    }
}