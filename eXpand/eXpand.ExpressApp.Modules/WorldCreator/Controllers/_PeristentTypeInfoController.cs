using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator.Controllers
{
    public partial class _PeristentTypeInfoController : ViewController
    {
        public _PeristentTypeInfoController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (IPersistentTypeInfo);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (ObjectSpace.Session.IsNewObject(View.CurrentObject))
                ((IPersistentTemplatedTypeInfo) View.CurrentObject).CodeTemplateInfo =
                    (ICodeTemplateInfo) ObjectSpace.CreateObject(TypesInfo.Instance.CodeTemplateInfoType);

        }
    }
}
