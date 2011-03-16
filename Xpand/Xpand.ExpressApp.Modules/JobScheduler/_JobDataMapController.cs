using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Quartz;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.JobScheduler {
    public class _JobDataMapController : SupportSchedulerController {
        readonly SingleChoiceAction _assignDataMapAction;

        public _JobDataMapController() {
            TargetObjectType = typeof(IJobDetail);
            _assignDataMapAction = new SingleChoiceAction(this, "AssignDataMap", PredefinedCategory.OpenObject);
            _assignDataMapAction.Items.Add(new ChoiceActionItem("AssignDataMap", null));
            _assignDataMapAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            _assignDataMapAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            _assignDataMapAction.Execute += AssignDataMapActionOnExecute;
        }

        public SingleChoiceAction AssignDataMapAction {
            get { return _assignDataMapAction; }
        }

        void AssignDataMapActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            var objectSpace = Application.GetObjectSpaceToShowViewFrom(Frame);
            if (singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data!=null) {
                var createObject = objectSpace.CreateObject((Type)singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data);
                AssignDataMap((IJobDetail)View.CurrentObject, createObject);
                var detailView = Application.CreateDetailView(objectSpace, createObject, true);
                singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = detailView;
                singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                var dialogController = new DialogController();
                dialogController.AcceptAction.Execute += AcceptActionOnExecute;
                singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
            }
        }

        void AssignDataMap(IJobDetail jobDetail, object createObject) {
            if (jobDetail.JobDataMap!= null) {
                var typeInfo = TypesInfo.FindTypeInfo(createObject.GetType());
                var jobDataMap = ((JobDataMap) jobDetail.JobDataMap);
                jobDataMap.Keys.OfType<string>().Each(AssignValue(createObject, jobDataMap, typeInfo));
            }
        }

        Action<string> AssignValue(object createObject, JobDataMap jobDataMap, ITypeInfo typeInfo) {
            return s => {
                var memberInfo = typeInfo.FindMember(s);
                if (memberInfo != null) memberInfo.SetValue(createObject, jobDataMap[s]);
            };
        }

        void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var currentObject = simpleActionExecuteEventArgs.CurrentObject;
            var jobDataMap = new JobDataMap();
            GetDataMapMemberInfos(TypesInfo.FindTypeInfo(currentObject.GetType())).Each(MapItsValue(jobDataMap, currentObject));
            var jobDetail = ((IJobDetail)View.CurrentObject);
            jobDetail.JobDataMap = jobDataMap;
        }

        IEnumerable<IMemberInfo> GetDataMapMemberInfos(ITypeInfo typeInfo) {
            return typeInfo.Members.Where(info =>info.IsPersistent&&  info.FindAttribute<NonDataMapMember>() == null);
        }

        Action<IMemberInfo> MapItsValue(JobDataMap jobDataMap, object currentObject) {
            return info => jobDataMap.Put(info.Name, info.GetValue(currentObject));
        }


        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
            View.SelectionChanged += ViewOnSelectionChanged;
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
            View.SelectionChanged-=ViewOnSelectionChanged;
        }

        void ViewOnSelectionChanged(object sender, EventArgs eventArgs) {
            if (View.CurrentObject!=null)
                AddChoiceItems(((IJobDetail)View.CurrentObject).Job.JobType);
        }

        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            if (objectChangedEventArgs.PropertyName == "Job") {
                AddChoiceItems(((IXpandJob) objectChangedEventArgs.NewValue).JobType);
            }
        }

        void AddChoiceItems(object value) {
            _assignDataMapAction.Items.Clear();
            if (value != null)
                Application.ObjectSpaceProvider.TypesInfo.FindTypeInfo((Type) value).FindAttributes<DataMapTypeAttribute>().Each(AddChoiceItem);
        }

        void AddChoiceItem(DataMapTypeAttribute obj) {
            _assignDataMapAction.Items.Add(new ChoiceActionItem(CaptionHelper.GetClassCaption(obj.Type.FullName), obj.Type));
        }
    }
}
