using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalEditorState;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [EditorStateRule("Hide templateInfo name", "CodeTemplateInfo.TemplateInfo.Name", EditorState.Hidden, "CodeTemplateInfo.PersistentAssemblyInfo is NULL", ViewType.DetailView)]
    public class PersistentTemplatedTypeInfo:PersistentTypeInfo, IPersistentTemplatedTypeInfo {

        public PersistentTemplatedTypeInfo(Session session) : base(session) {
        }

        private CodeTemplateInfo _codeTemplateInfo;


        [Aggregated]
        public CodeTemplateInfo CodeTemplateInfo
        {
            get
            {
                return _codeTemplateInfo;
            }
            set
            {
                SetPropertyValue("CodeTemplateInfo", ref _codeTemplateInfo, value);
            }
        }
        ICodeTemplateInfo IPersistentTemplatedTypeInfo.CodeTemplateInfo
        {
            get { return CodeTemplateInfo; }
            set { CodeTemplateInfo = value as CodeTemplateInfo; }
        }

    }
}