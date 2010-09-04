using System.ComponentModel;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Core;


namespace Xpand.ExpressApp.Web
{
    public abstract partial class XpandWebApplication : DevExpress.ExpressApp.Web.WebApplication,ISupportModelsManager
    {
        protected XpandWebApplication()
        {
            InitializeComponent();
            DetailViewCreating += OnDetailViewCreating;
            ListViewCreating += OnListViewCreating;
        }

        protected override void OnCustomProcessShortcut(CustomProcessShortcutEventArgs args)
        {
            base.OnCustomProcessShortcut(args);
            new ViewShortCutProccesor(this).Proccess(args);
            
        }

        void OnListViewCreating(object sender, ListViewCreatingEventArgs args)
        {
            args.View = ViewFactory.CreateListView(this, args.ViewID, args.CollectionSource, args.IsRoot);
        }

        void OnDetailViewCreating(object sender, DetailViewCreatingEventArgs args)
        {
            args.View = ViewFactory.CreateDetailView(this, args.ViewID, args.Obj, args.ObjectSpace, args.IsRoot);
        }
        public ApplicationModelsManager ModelsManager
        {
            get { return modelsManager; }
        }

        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            this.CreateCustomObjectSpaceprovider(args);
            base.OnCreateCustomObjectSpaceProvider(args);
        }

        protected XpandWebApplication(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }


    }
}