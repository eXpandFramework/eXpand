using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Win.Interfaces;

namespace Xpand.ExpressApp.Win
{
    public partial class XpandWinApplication : WinApplication, ILogOut, ISupportModelsManager, ISupportCustomListEditorCreation, IWinApplication
    {
        bool _isSharedModel;

        protected override bool IsSharedModel
        {
            get { return _isSharedModel; }
        }
        public event EventHandler<CreatingListEditorEventArgs> CustomCreateListEditor;

        public void OnCustomCreateListEditor(CreatingListEditorEventArgs e)
        {
            EventHandler<CreatingListEditorEventArgs> handler = CustomCreateListEditor;
            if (handler != null) handler(this, e);
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

        public override IModelTemplate GetTemplateCustomizationModel(IFrameTemplate template)
        {
            var list = new List<ModelApplicationBase>();
            while (((ModelApplicationBase)Model).LastLayer.Id != "UserDiff" && ((ModelApplicationBase)Model).LastLayer.Id != AfterSetupLayerId)
            {
                var modelApplicationBase = ((ModelApplicationBase)Model).LastLayer;
                list.Add(modelApplicationBase);
                ((ModelApplicationBase)Model).RemoveLayer(modelApplicationBase);
            }
            var modelTemplate = base.GetTemplateCustomizationModel(template);
            foreach (var modelApplicationBase in list)
            {
                ((ModelApplicationBase)Model).AddLayer(modelApplicationBase);
            }
            return modelTemplate;
        }

        protected override ListEditor CreateListEditorCore(IModelListView modelListView, CollectionSourceBase collectionSource)
        {
            var creatingListEditorEventArgs = new CreatingListEditorEventArgs(modelListView, collectionSource);
            OnCustomCreateListEditor(creatingListEditorEventArgs);
            return creatingListEditorEventArgs.Handled ? creatingListEditorEventArgs.ListEditor : base.CreateListEditorCore(modelListView, collectionSource);
        }

        public void Logout()
        {
            Tracing.Tracer.LogSeparator("Application is being restarted");


            ShowViewStrategy.CloseAllWindows();
            if (!ignoreUserModelDiffs)
                SaveModelChanges();
            Security.Logoff();
            Tracing.Tracer.LogSeparator("Application is now restarting");
            _isSharedModel = true;
            Setup();
            _isSharedModel = false;
            if (SecuritySystem.Instance.NeedLogonParameters)
            {
                Tracing.Tracer.LogText("Logon With Parameters");
                PopupWindowShowAction showLogonAction = CreateLogonAction();
                showLogonAction.Cancel += showLogonAction_Cancel;
                var helper = new PopupWindowShowActionHelper(showLogonAction);

                using (WinWindow popupWindow = helper.CreatePopupWindow(false))
                    ShowLogonWindow(popupWindow);
            }
            else
                Logon(null);

            ProcessStartupActions();
            ShowStartupWindow();
            SplashScreen.Stop();
            Tracing.Tracer.LogSeparator("Application running");
        }


        void showLogonAction_Cancel(object sender, EventArgs e)
        {
            Exit();
        }




        public XpandWinApplication()
        {
            if (XpandModuleBase.Application==null)
                Application.ThreadException+=(sender, args) => HandleException(args.Exception,this);
            else {
                Application.ThreadException += (sender, args) => HandleException(args.Exception, (XpandWinApplication) XpandModuleBase.Application);
            }
            InitializeComponent();        
            DetailViewCreating += OnDetailViewCreating;
            ListViewCreating += OnListViewCreating;
        }

        public static void HandleException(Exception exception, XpandWinApplication xpandWinApplication){
            xpandWinApplication.HandleException(exception);
        }

        public XpandWinApplication(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            this.CreateCustomObjectSpaceprovider(args);
            base.OnCreateCustomObjectSpaceProvider(args);
        }

    }


    public class ModelEditFormShowningEventArgs : HandledEventArgs
    {
        public ModelEditFormShowningEventArgs(ModelEditorForm modelEditorForm)
        {
            ModelEditorForm = modelEditorForm;
        }

        public ModelEditorForm ModelEditorForm { get; set; }
    }
}