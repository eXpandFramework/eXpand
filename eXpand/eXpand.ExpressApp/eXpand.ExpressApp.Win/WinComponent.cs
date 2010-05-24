using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.Win.Interfaces;

namespace eXpand.ExpressApp.Win
{
    public partial class WinComponent : WinApplication, ILogOut
    {

        protected override void OnCustomProcessShortcut(CustomProcessShortcutEventArgs args)
        {
            base.OnCustomProcessShortcut(args);
            new ViewShortCutProccesor(this).Proccess(args);
            
        }

        public void Logout()
        {
            Tracing.Tracer.LogSeparator("Application is being restarted");
            if (!ignoreUserModelDiffs)
                SaveModelChanges();

            ShowViewStrategy.CloseAllWindows();
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

        protected override CollectionSourceBase CreateCollectionSourceCore(ObjectSpace objectSpace, Type objectType, bool isServerMode, CollectionSourceMode mode) {
            return isServerMode
                       ? (CollectionSourceBase) new LinqServerCollectionSource(objectSpace, objectType, true)
                       : new LinqCollectionSource(objectSpace, objectType, false);
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