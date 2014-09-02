using System;
using System.Collections;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.General;
#if (CodeFirst)
using DevExpress.Persistent.BaseImpl.EF;
#endif
using EFDemo.Module.Data;

namespace EFDemo.Module.Controllers {
	public partial class TaskActionsController : ViewController {
        private ChoiceActionItem setPriorityItem;
        private ChoiceActionItem setStatusItem;
        private void FillItemWithEnumValues(ChoiceActionItem parentItem, Type enumType) {
            foreach(object current in Enum.GetValues(enumType)) {
                EnumDescriptor ed = new EnumDescriptor(enumType);
                ChoiceActionItem item = new ChoiceActionItem(ed.GetCaption(current), current);
                item.ImageName = ImageLoader.Instance.GetEnumValueImageName(current);
                parentItem.Items.Add(item);
            }
        }
        private void TaskActionsController_Activated(object sender, EventArgs e) {
            SetTaskAction.Active.SetItemValue("SecurityAllowance", SecuritySystem.IsGranted(new ObjectAccessPermission(typeof(DemoTask), ObjectAccess.Write, ObjectAccessModifier.Allow)));
        }
        private void SetTaskAction_Execute(object sender, SingleChoiceActionExecuteEventArgs args) {
            IObjectSpace objectSpace = View is ListView ? Application.CreateObjectSpace() : View.ObjectSpace;
            ArrayList objectsToProcess = new ArrayList(args.SelectedObjects);
            if(args.SelectedChoiceActionItem.ParentItem == setPriorityItem) {
                foreach(Object obj in objectsToProcess) {
                    DemoTask objInNewObjectSpace = (DemoTask)objectSpace.GetObject(obj);
                    objInNewObjectSpace.Priority = (Priority)args.SelectedChoiceActionItem.Data;
                }
            }
            else if(args.SelectedChoiceActionItem.ParentItem == setStatusItem) {
                foreach(Object obj in objectsToProcess) {
                    DemoTask objInNewObjectSpace = (DemoTask)objectSpace.GetObject(obj);
                    objInNewObjectSpace.Status = (TaskStatus)args.SelectedChoiceActionItem.Data;
                }
            }
            if(View is DetailView && ((DetailView)View).ViewEditMode == ViewEditMode.View) {
                objectSpace.CommitChanges();
            }
            if(View is ListView) {
                objectSpace.CommitChanges();
                View.ObjectSpace.Refresh();
            }
        }
        public TaskActionsController() {
            TypeOfView = typeof(ObjectView);
            InitializeComponent();
            RegisterActions(components);

            setPriorityItem = new ChoiceActionItem(CaptionHelper.GetMemberCaption(typeof(DemoTask), "Priority"), null);
            SetTaskAction.Items.Add(setPriorityItem);
            FillItemWithEnumValues(setPriorityItem, typeof(Priority));
            setStatusItem = new ChoiceActionItem(CaptionHelper.GetMemberCaption(typeof(DemoTask), "Status"), null);
            SetTaskAction.Items.Add(setStatusItem);
            FillItemWithEnumValues(setStatusItem, typeof(TaskStatus));
        }
    }
}
