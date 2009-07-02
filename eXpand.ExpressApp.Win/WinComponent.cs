﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.Win.Interfaces;
using eXpand.ExpressApp.Win.Templates;

namespace eXpand.ExpressApp.Win
{
    public partial class WinComponent : WinApplication,ILogOut
    {
 
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

            if (SplashScreen != null)
                SplashScreen.Start();
            
            try
            {
                ProcessStartupActions();
                ShowStartupWindow();
            }
            finally
            {
                if (SplashScreen != null)
                    SplashScreen.Stop();
            }
            Tracing.Tracer.LogSeparator("Application running");
        }


        void showLogonAction_Cancel(object sender, EventArgs e) {
            Exit();
        }
        
        #region OnModelEditFormShowning
        /// <summary>
        /// Triggers the ModelEditFormShowning event.
        /// </summary>
        public virtual void OnModelEditFormShowning(ModelEditFormShowningEventArgs ea)
        {
            if (ModelEditFormShowning != null)
                ModelEditFormShowning(null/*this*/, ea);
        }
        #endregion
        public event EventHandler<ModelEditFormShowningEventArgs> ModelEditFormShowning;
//        private const string XpoModelDictionaryDifferenceModuleTypeName = "XAFPoint.ExpressApp.DictionaryDifferenceStore.XpoModelDictionaryDifferenceStore, XAFPoint.ExpressApp.DictionaryDifferenceStore, Version=*, Culture=neutral, PublicKeyToken=c52ffed5d5ff0958";
//        private const string XpoUserModelDictionaryDifferenceModuleTypeName = "XAFPoint.ExpressApp.DictionaryDifferenceStore.XpoUserModelDictionaryDifferenceStore, XAFPoint.ExpressApp.DictionaryDifferenceStore, Version=*, Culture=neutral, PublicKeyToken=c52ffed5d5ff0958";
//        private IObjectSpaceProvider objectSpaceProvider;
        
        public WinComponent()
        {
            InitializeComponent();
            DatabaseVersionMismatch += (sender, args) => this.DatabaseVersionMismatchEvent(sender, args);
            
        }

        protected override void OnCustomCheckCompatibility(CustomCheckCompatibilityEventArgs args)
        {
            if (Info.GetChildNode("Options").GetAttributeBoolValue("DisableCompatibilityCheck", false))
                args.Handled = true;
            base.OnCustomCheckCompatibility(args);
        }

        protected override void OnLoggedOn(LogonEventArgs args)
        {
            base.OnLoggedOn(args);
            DictionaryHelper.AddFields(Info, ObjectSpaceProvider.XPDictionary);
        }

        protected override void OnSetupComplete()
        {
            base.OnSetupComplete();

            DictionaryHelper.AddFields(Info, ObjectSpaceProvider.XPDictionary);
            
        }

        protected override void ProcessStartupActions(){
            base.ProcessStartupActions();
            Session session = ObjectSpaceProvider.CreateUpdatingSession();
            //this.InitializeXPObjectTypes(new UnitOfWork(session.DataLayer));
        }

        protected override Form CreateModelEditorForm()
        {
            var modelEditorForm = (ModelEditorForm) base.CreateModelEditorForm();
            OnModelEditFormShowning(new ModelEditFormShowningEventArgs(modelEditorForm));
            return modelEditorForm;
            
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
            
//            objectSpaceProvider = args.ObjectSpaceProvider;
        }

        
        protected override IFrameTemplate OnCreateCustomTemplate(string name)
        {
            if (name == TemplateContext.ApplicationWindow)
                return new MainForm(this);
            return base.OnCreateCustomTemplate(name);
        }

//        protected override DictionaryDifferenceStore CreateModelDifferenceStoreCore()
//        {
//            Type type = Type.GetType(
//                XpoModelDictionaryDifferenceModuleTypeName);
//            if (type != null)
//                return
//                    (DictionaryDifferenceStore)
//                    Activator.CreateInstance(type, new object[] {objectSpaceProvider.CreateUpdatingSession(), this});
//
//            return base.CreateModelDifferenceStoreCore();
//        }

//        protected override DictionaryDifferenceStore CreateUserModelDifferenceStoreCore()
//        {
//
//            Type type = Type.GetType(
//                XpoUserModelDictionaryDifferenceModuleTypeName);
//            if (type != null)
//                return
//                    (DictionaryDifferenceStore)
//                    Activator.CreateInstance(type, new object[] { objectSpaceProvider.CreateUpdatingSession(), this});
//            
//            return base.CreateUserModelDifferenceStoreCore();
//        }

        protected override CollectionSourceBase CreateCollectionSourceCore(ObjectSpace objectSpace, Type objectType, string listViewID)
        {
            CollectionSourceBase result = null;
            if(Model != null) {
                if(!String.IsNullOrEmpty(listViewID)) {
                    DictionaryNode listViewNode = FindViewInfo(listViewID);
                    if(listViewNode != null) {
                        var listViewInfo = new ListViewInfoNodeWrapper(listViewNode);
                        if(listViewInfo.UseServerMode && (listViewInfo.EditMode != EditMode.Editable)) {
                            result = new LinqServerCollectionSource(objectSpace, objectType);
                        }
                    }
                }
            }
            if(result == null) {
                result = new LinqCollectionSource(objectSpace, objectType);
            }
            return result;

        }

//        //TODO:check webappliation
//        protected override void HandleExceptionCore(Exception e)
//        {
//            base.HandleExceptionCore(e);
//            if (!(e is ValidationException)&&!(e.InnerException!=null&&e.InnerException is ValidationException) )
//            {
//                if (!System.Diagnostics.Debugger.IsAttached)
//                {
//                    string asString = Tracing.Tracer.GetLastEntriesAsString();
//                    asString = Regex.Replace(asString, @"\n", "<br>");
//                    Logger.Write(asString);
//                }
//            }
//        }
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