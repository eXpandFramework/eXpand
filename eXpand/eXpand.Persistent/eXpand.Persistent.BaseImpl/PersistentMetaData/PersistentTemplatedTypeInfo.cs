using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalEditorState;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [EditorStateRule("Hide templateInfo name", "CodeTemplateInfo.TemplateInfo.Name", EditorState.Hidden, null,
        ViewType.DetailView)]
    public class PersistentTemplatedTypeInfo : PersistentTypeInfo, IPersistentTemplatedTypeInfo {
        CodeTemplateInfo _codeTemplateInfo;

        public PersistentTemplatedTypeInfo(Session session) : base(session) {
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