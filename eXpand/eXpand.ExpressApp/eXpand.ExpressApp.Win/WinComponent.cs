using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.Win.Interfaces;

namespace eXpand.ExpressApp.Win
{

    public partial class WinComponent : WinApplication, ILogOut, ISupportModelsManager, ISupportCustomListEditorCreation,IWinApplication {
        

        public event EventHandler<CreatingListEditorEventArgs> CustomCreateListEditor;

        public void OnCustomCreateListEditor(CreatingListEditorEventArgs e) {
            EventHandler<CreatingListEditorEventArgs> handler = CustomCreateListEditor;
            if (handler != null) handler(this, e);
        }

        protected override void OnCustomProcessShortcut(CustomProcessShortcutEventArgs args)
        {
            base.OnCustomProcessShortcut(args);
            new ViewShortCutProccesor(this).Proccess(args);
            
        }

        public ApplicationModelsManager ModelsManager {
            get { return modelsManager; }
        }

        protected override ListEditor CreateListEditorCore(IModelListView modelListView, CollectionSourceBase collectionSource) {
            var creatingListEditorEventArgs = new CreatingListEditorEventArgs(modelListView,collectionSource);
            OnCustomCreateListEditor(creatingListEditorEventArgs);
            if (creatingListEditorEventArgs.Handled)
                return creatingListEditorEventArgs.ListEditor;
            return base.CreateListEditorCore(modelListView, collectionSource);
        }

        public void Logout()
        {
            Tracing.Tracer.LogSeparator("Application is being restarted");
            

            ShowViewStrategy.CloseAllWindows();
            if (!ignoreUserModelDiffs)
                SaveModelChanges();
            Security.Logoff();
            Tracing.Tracer.LogSeparator("Application is now restarting");
            Setup();
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


        

        public WinComponent()
        {
            InitializeComponent();
        }


        public WinComponent(IContainer container)
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