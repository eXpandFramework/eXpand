using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.General.Model;
using Xpand.XAF.Modules.CloneModelView;

namespace SystemTester.Module.Win.FunctionalTests.PropertyEditors.StringLookupPropertyEditor {
    [XpandNavigationItem("PropertyEditors/StringLookup/Default", "StringLookupPropertyEditorObject_ListView")]

    [XpandNavigationItem("PropertyEditors/StringLookup/Mask", "StringLookupPropertyEditorObject_Mask_ListView")]

    [XpandNavigationItem("PropertyEditors/StringLookup/PredifenedValues", "StringLookupPropertyEditorObject_PredifenedValues_ListView")]

    [CloneModelView(CloneViewType.DetailView, "StringLookupPropertyEditorObject_Mask_DetailView")]
    [CloneModelView(CloneViewType.ListView, "StringLookupPropertyEditorObject_Mask_ListView", DetailView = "StringLookupPropertyEditorObject_Mask_DetailView")]

    [CloneModelView(CloneViewType.DetailView, "StringLookupPropertyEditorObject_PredifenedValues_DetailView")]
    [CloneModelView(CloneViewType.ListView, "StringLookupPropertyEditorObject_PredifenedValues_ListView", DetailView = "StringLookupPropertyEditorObject_PredifenedValues_DetailView")]
    public class StringLookupPropertyEditorObject:BaseObject {
        public StringLookupPropertyEditorObject(Session session) : base(session){
        }
        
        public string Phone { get; set; }
        [InvisibleInAllViews]
        public string PredifineValue { get; set; }
    }
}
