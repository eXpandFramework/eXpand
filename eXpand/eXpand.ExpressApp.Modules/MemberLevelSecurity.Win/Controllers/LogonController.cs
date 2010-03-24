using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.NodeWrappers;

namespace XAFPoint.ExpressApp.HideMemberModule.Controllers
{
    public partial class LogonController : DevExpress.ExpressApp.LogonController 
    {
        public LogonController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override SimpleAction CreateAcceptAction()
        {
            var action = base.CreateAcceptAction();
            action.Executed+=Action_OnExecuted;
            action.ExecuteCompleted+=Action_OnExecuteCompleted;
            action.Executing+=Action_OnExecuting;
            
            return action;
        }

        protected override void Accept(SimpleActionExecuteEventArgs args)
        {
            base.Accept(args);
        }

        private void Action_OnExecuting(object sender, CancelEventArgs e)
        {
            
        }

        private void Action_OnExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            
        }

        private void Action_OnExecuted(object sender, ActionBaseEventArgs e)
        {
                        
        }

        public override void UpdateModel(Dictionary dictionary)
        {
            base.UpdateModel(dictionary);
        }

        public override ControllerInfoNodeWrapper CreateInfo(ApplicationNodeWrapper applicationNode, Controller baseController)
        {
            return base.CreateInfo(applicationNode, baseController);
        }
    }
}
