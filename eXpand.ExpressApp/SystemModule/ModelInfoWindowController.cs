using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class ModelInfoWindowController : BaseWindowController
    {
        private static DictionaryNode model;

        public static DictionaryNode Model
        {
            get { return model; }
        }

        public static DictionaryNode GetOptionNode()
        {
            return Model.GetChildNodeByPath("Options");
        }

        public ModelInfoWindowController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            model = Application.Info;
        }
    }
}