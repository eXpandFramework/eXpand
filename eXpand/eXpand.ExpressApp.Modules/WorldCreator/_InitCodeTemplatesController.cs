using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.SystemModule;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator
{
    public partial class _InitCodeTemplatesController : ViewController
    {
        public _InitCodeTemplatesController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<TrackObjectConstructionsViewController>().ObjectCreated+=OnObjectCreated;
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Frame.GetController<TrackObjectConstructionsViewController>().ObjectCreated -= OnObjectCreated;
        }
        void OnObjectCreated(object sender, ObjectCreatedEventArgs objectCreatedEventArgs) {
            var codeTemplate = ((ICodeTemplate) objectCreatedEventArgs.Object);
            codeTemplate.TemplateCode=codeTemplate.GetDefaultTemplate();
            codeTemplate
        }
    }
}
