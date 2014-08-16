using System;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General.Model;
using XpandSystemTester.Module.FunctionalTests;

namespace XpandSystemTester.Module.Win.FunctionalTests.FilterEditor{
    [NavigationItem("FilterEditor")]
    [CloneView(CloneViewType.ListView, "FilterEditor_Criteria")]
    [XpandNavigationItem("FilterEditor/Criteria", "FilterEditor_Criteria")]
    public class FilterEditorObject : CommonTestObject{
        private FilterEditorObjectRef _filterEditorObjectRef;

        public FilterEditorObject(Session session) : base(session){
        }

        public FilterEditorObjectRef FilterEditorObjectRef{
            get { return _filterEditorObjectRef; }
            set { SetPropertyValue("FilterEditorObjectRef", ref _filterEditorObjectRef, value); }
        }
    }

    
    public class FilterEditorObjectRef : CommonTestObject{
        private static Guid _guid;

        static FilterEditorObjectRef(){
            _guid = new Guid("00022993-9604-4552-b83a-7de5cec5104a");
        }
        public FilterEditorObjectRef(Session session) : base(session){
        }

        protected override void OnSaving(){
            base.OnSaving();
            SetMemberValue("oid", _guid);
            _guid=Guid.NewGuid();
        }
    }
}