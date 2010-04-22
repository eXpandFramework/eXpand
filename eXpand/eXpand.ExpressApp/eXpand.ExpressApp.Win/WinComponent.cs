using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.NodeWrappers;
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
        


        public WinComponent()
        {
            InitializeComponent(); 
        }

        protected override void OnCustomCheckCompatibility(CustomCheckCompatibilityEventArgs args)
        {
            if (Info.GetChildNode("Options").GetAttributeBoolValue("DisableCompatibilityCheck", false))
                args.Handled = true;
            base.OnCustomCheckCompatibility(args);
        }




        protected override Form CreateModelEditorForm()
        {
//            var single = AppDomain.CurrentDomain.GetTypes("ModelDifferenceObject").FirstOrDefault();
//            XPBaseObject baseObject = (XPBaseObject) ObjectSpaceProvider.CreateObjectSpace().FindObject(single,CriteriaOperator.Parse("PersistentApplication.Name=?","Fps"));
//            Dictionary dictionary = (Dictionary)baseObject.GetType().GetMethod("GetCombinedModel1").Invoke(baseObject, null);
//            
//            dictionary=new Dictionary(Model.RootNode,Model.Schema);
//            ModelEditorForm modelEditorForm = new ModelEditorForm(
//                new ModelEditorController(dictionary.Clone(), new ModelDictionaryDifferenceStore(dictionary, LastDiffsStore), Modules),
//                new SettingsStorageOnDictionary(dictionary.RootNode.GetChildNode("ModelEditor")));
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
        }

        protected override CollectionSourceBase CreateCollectionSourceCore(ObjectSpace objectSpace, Type objectType, string listViewID)
        {
            CollectionSourceBase result = null;
            if(Model != null) {
                if(!String.IsNullOrEmpty(listViewID)) {
                    DictionaryNode listViewNode = FindViewInfo(listViewID);
                    if(listViewNode != null) {
                        var listViewInfo = new ListViewInfoNodeWrapper(listViewNode);
                        if(listViewInfo.UseServerMode && (!listViewInfo.AllowEdit)) {
                            result = new LinqServerCollectionSource(objectSpace, objectType);
                        }
                    }
                }
            }
            return result ?? (new LinqCollectionSource(objectSpace, objectType));
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