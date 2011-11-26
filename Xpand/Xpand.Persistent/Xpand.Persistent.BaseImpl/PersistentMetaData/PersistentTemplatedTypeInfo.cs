using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;
using DevExpress.Xpo;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.Persistent.BaseImpl.PersistentMetaData {
    [Appearance("Hide templateInfo name", AppearanceItemType.ViewItem, null, TargetItems = "CodeTemplateInfo.TemplateInfo.Name", Visibility = ViewItemVisibility.Hide)]
    public class PersistentTemplatedTypeInfo : PersistentTypeInfo, IPersistentTemplatedTypeInfo {
        CodeTemplateInfo _codeTemplateInfo;

        public PersistentTemplatedTypeInfo(Session session)
            : base(session) {
        }


        [Aggregated]
        [Association("CodeTemplateInfo-PersistentTemplatedTypeInfos")]
        public CodeTemplateInfo CodeTemplateInfo {
            get { return _codeTemplateInfo; }
            set { SetPropertyValue("CodeTemplateInfo", ref _codeTemplateInfo, value); }
        }
        #region IPersistentTemplatedTypeInfo Members
        ICodeTemplateInfo IPersistentTemplatedTypeInfo.CodeTemplateInfo {
            get { return CodeTemplateInfo; }
            set { CodeTemplateInfo = value as CodeTemplateInfo; }
        }
        #endregion
    }
}