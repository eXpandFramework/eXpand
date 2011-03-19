using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Quartz;
using Xpand.ExpressApp.JobScheduler.QuartzExtensions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobTriggerLinkController : SupportSchedulerController {
        public JobTriggerLinkController() {
            TargetObjectType = typeof(IXpandJobDetail);
        }

        public event EventHandler<CustomNonDeletedObjectsToSaveArgs> CustomNonDeletedObjectsToSave;

        protected void OnCustomNonDeletedObjectsToSave(CustomNonDeletedObjectsToSaveArgs e) {
            EventHandler<CustomNonDeletedObjectsToSaveArgs> handler = CustomNonDeletedObjectsToSave;
            if (handler != null) handler(this, e);
        }


        readonly List<JobDetailTriggerInfo> supportJobDetails = new List<JobDetailTriggerInfo>();

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleting -= ObjectSpaceOnObjectDeleting;
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpaceOnCommitting;
            ObjectSpace.ObjectDeleting += ObjectSpaceOnObjectDeleting;
        }

        void ObjectSpaceOnObjectDeleting(object sender, ObjectsManipulatingEventArgs objectsManipulatingEventArgs) {
            supportJobDetails.AddRange(objectsManipulatingEventArgs.Objects.OfType<ISupportSchedulerLink>().SelectMany(CreateJobDetailInfos()));
        }

        Func<ISupportSchedulerLink, IEnumerable<JobDetailTriggerInfo>> CreateJobDetailInfos() {
            return link => {
                if (link is ISupportJobDetail)
                    return CreateJobDetailInfosCore(((ISupportJobDetail)link));
                return CreateJobDetailInfosCore((IJobSchedulerGroupTriggerLink)link);
            };
        }

        IEnumerable<JobDetailTriggerInfo> CreateJobDetailInfosCore(IJobSchedulerGroupTriggerLink jobSchedulerGroupTriggerLink) {
            var jobDetails = ObjectSpace.GetObjects(View.ObjectTypeInfo.Type, ForTheSameGroup(jobSchedulerGroupTriggerLink)).OfType<IXpandJobDetail>();
            return jobDetails.Select(detail => new JobGroupInfo(detail.Name, detail.Job.JobType, detail.Group.Name, jobSchedulerGroupTriggerLink.Trigger.Name)).OfType<JobDetailTriggerInfo>();
        }

        IEnumerable<JobDetailTriggerInfo> CreateJobDetailInfosCore(ISupportJobDetail supportJobDetail) {
            var jobType = supportJobDetail.JobDetail.Job.JobType;
            if (supportJobDetail is IJobDetailJobListenerTriggerLink) {
                var jobListenerTrigger = ((IJobDetailJobListenerTriggerLink)supportJobDetail).JobListenerTrigger;
                jobType = jobListenerTrigger.JobType;
                return new List<JobDetailTriggerInfo> { new ListenerInfo(supportJobDetail.JobDetail.Name, jobType, jobListenerTrigger.Event, supportJobDetail.JobDetail.Job.JobType, jobListenerTrigger.Group) };
            }
            return new List<JobDetailTriggerInfo> { new JobDetailTriggerInfo(supportJobDetail.JobDetail.Name, jobType, ((IJobDetailTriggerLink)supportJobDetail).JobTrigger.Name) };
        }

        void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs) {
            Save();
            Delete();
        }
        void AddTriggerListeners(IJobTriggerTriggerListenerTriggerLink link) {
            List<IXpandJobDetail> jobDetails = GetRelatedJobDetails(ForTheSameJobTypeOrGroup(link.TriggerListenerTrigger.JobType, link.TriggerListenerTrigger.Group));
            var calculateTriggerListenerNames = CalculateTriggerListenerNames(link.TriggerListenerTrigger.Event);
            var triggerListenersKeys = CreateTriggerListenersKeys(link.TriggerListenerTrigger.Event);
            jobDetails.ForEach(detail => GetListenerDataMap(detail, AddListener, detail.Name, detail.Job.JobType, calculateTriggerListenerNames, triggerListenersKeys));
        }

        Action<JobDataMap, List<JobKey>> CreateTriggerListenersKeys(TriggerListenerEvent listenerEvent) {
            return (map, list) => map.CreateTriggerListenersKey(listenerEvent, list.ToArray());
        }

        Func<JobDataMap, List<JobKey>> CalculateTriggerListenerNames(TriggerListenerEvent triggerListenerEvent) {
            return map => map.GetTriggerListenerNames(triggerListenerEvent);
        }

        void AddJobListeners(IJobDetailJobListenerTriggerLink link) {
            List<IXpandJobDetail> jobDetails = GetRelatedJobDetails(ForTheSameJobTypeOrGroup(link.JobListenerTrigger.JobType, link.JobListenerTrigger.Group));
            var calculateJobListenerNames = CalculateJobListenerNames(link.JobListenerTrigger.Event);
            var listenersKeyAction = CreateJobListenersKeys(link.JobListenerTrigger.Event);
            jobDetails.ForEach(detail => GetListenerDataMap(detail, AddListener, link.JobDetail.Name, link.JobDetail.Job.JobType, calculateJobListenerNames, listenersKeyAction));
        }

        Action<JobDataMap, List<JobKey>> CreateJobListenersKeys(JobListenerEvent jobListenerEvent) {
            return (map, list) => map.CreateJobListenersKey(jobListenerEvent, list.ToArray());
        }

        Func<JobDataMap, List<JobKey>> CalculateJobListenerNames(JobListenerEvent listenerEvent) {
            return map => map.GetJobListenerNames(listenerEvent);
        }

        Func<CriteriaOperator> ForTheSameJobTypeOrGroup(Type type, IJobSchedulerGroup group) {
            return () => CriteriaOperator.Parse("Job.JobType=? OR (Group Is Not Null AND Group=?)", type, group);
        }

        List<IXpandJobDetail> GetRelatedJobDetails(Func<CriteriaOperator> action) {
            var type = TypesInfo.FindBussinessObjectType<IXpandJobDetail>();
            return ObjectSpace.GetObjects(type, action.Invoke()).OfType<IXpandJobDetail>().Where(detail => !ObjectSpace.IsDeletedObject(detail)).ToList();
        }

        void Delete() {
            supportJobDetails.OfType<ListenerInfo>().ToList().ForEach(RemoveListeners);
            supportJobDetails.OfType<JobDetailTriggerInfo>().ToList().ForEach(UnscheduleJob);
            supportJobDetails.Clear();
        }

        void UnscheduleJob(JobDetailTriggerInfo jobTriggerInfo) {
            var jobGroup = jobTriggerInfo is JobGroupInfo ? ((JobGroupInfo)jobTriggerInfo).Group : null;
            if (!string.IsNullOrEmpty(jobTriggerInfo.TriggerName))
                Scheduler.UnscheduleJob(jobTriggerInfo.TriggerName, jobTriggerInfo.JobType, jobTriggerInfo.JobName, jobGroup);
        }

        void Save() {
            var customNonDeletedObjectsToSaveArgs = new CustomNonDeletedObjectsToSaveArgs(ObjectSpace.GetNonDeletedObjectsToSave<object>());
            OnCustomNonDeletedObjectsToSave(customNonDeletedObjectsToSaveArgs);
            customNonDeletedObjectsToSaveArgs.Objects.OfType<IJobTriggerTriggerListenerTriggerLink>().ToList().ForEach(AddTriggerListeners);
            customNonDeletedObjectsToSaveArgs.Objects.OfType<IJobDetailJobListenerTriggerLink>().ToList().ForEach(AddJobListeners);
            customNonDeletedObjectsToSaveArgs.Objects.OfType<IJobDetailTriggerLink>().ToList().ForEach(ScheduleJob);
            customNonDeletedObjectsToSaveArgs.Objects.OfType<IJobSchedulerGroupTriggerLink>().ToList().ForEach(ScheduleGroup);
        }



        void ScheduleJob(IJobDetailTriggerLink link) {
            Scheduler.ScheduleJob(link.JobTrigger, link.JobDetail, null);
        }

        void ScheduleGroup(IJobSchedulerGroupTriggerLink link) {
            var relatedJobDetails = GetRelatedJobDetails(() => ForTheSameGroup(link));
            relatedJobDetails.ForEach(detail => Scheduler.ScheduleJob(link.Trigger, detail, link.JobSchedulerGroup.Name));
        }

        CriteriaOperator ForTheSameGroup(IJobSchedulerGroupTriggerLink link) {
            return CriteriaOperator.Parse("Group.Name=?", link.JobSchedulerGroup.Name);
        }

        void RemoveListeners(ListenerInfo listenerInfo) {
            List<IXpandJobDetail> jobDetails = GetRelatedJobDetails(ForTheSameJobTypeOrGroup(listenerInfo.JobType, listenerInfo.Group));
            Func<JobDataMap, List<JobKey>> calculateJobListenerNames;
            Action<JobDataMap, List<JobKey>> listenersKeyAction;
            if (listenerInfo.ListenerEvent is JobListenerEvent) {
                var jobListenerEvent = (JobListenerEvent)listenerInfo.ListenerEvent;
                listenersKeyAction = CreateJobListenersKeys(jobListenerEvent);
                calculateJobListenerNames = CalculateJobListenerNames(jobListenerEvent);
            } else {
                var triggerListenerEvent = (TriggerListenerEvent)listenerInfo.ListenerEvent;
                listenersKeyAction = CreateTriggerListenersKeys(triggerListenerEvent);
                calculateJobListenerNames = CalculateTriggerListenerNames(triggerListenerEvent);
            }
            jobDetails.ForEach(detail => GetListenerDataMap(detail, RemoveListener, listenerInfo.JobName, listenerInfo.OriginType, calculateJobListenerNames, listenersKeyAction));
        }

        void RemoveListener(List<JobKey> list, JobKey key) {
            list.Remove(key);
        }

        void AddListener(List<JobKey> list, JobKey key) {
            list.Add(key);
        }

        void GetListenerDataMap(IXpandJobDetail detail, Action<List<JobKey>, JobKey> action, string jobName, Type jobType, Func<JobDataMap, List<JobKey>> getListeners, Action<JobDataMap, List<JobKey>> createListenersKeyAction) {
            var jobDetail = Scheduler.GetJobDetail(detail);
            var listenerNames = getListeners.Invoke(jobDetail.JobDataMap);
            action.Invoke(listenerNames, new JobKey(jobName, jobType.FullName));
            createListenersKeyAction.Invoke(jobDetail.JobDataMap, listenerNames);
            Scheduler.StoreJob(jobDetail);

        }
    }

    public class CustomNonDeletedObjectsToSaveArgs : HandledEventArgs {
        public CustomNonDeletedObjectsToSaveArgs(IEnumerable<object> objects) {
            Objects = objects;
        }

        public IEnumerable<object> Objects { get; set; }
    }


    internal class ListenerInfo : JobDetailTriggerInfo {
        readonly Enum _listenerEvent;
        readonly Type _originType;
        readonly IJobSchedulerGroup _group;

        public ListenerInfo(string jobName, Type jobType, Enum listenerEvent, Type originType, IJobSchedulerGroup group)
            : base(jobName, jobType, null) {
            _listenerEvent = listenerEvent;
            _originType = originType;
            _group = group;
        }

        public Enum ListenerEvent {
            get { return _listenerEvent; }
        }

        public IJobSchedulerGroup Group {
            get { return _group; }
        }

        public Type OriginType {
            get { return _originType; }
        }
    }

    internal class JobGroupInfo : JobDetailTriggerInfo, IJobTriggerInfo {
        readonly string _group;

        public JobGroupInfo(string jobName, Type jobType, string group, string triggerName)
            : base(jobName, jobType, triggerName) {
            _group = group;
        }

        public string Group {
            get { return _group; }
        }

    }

    internal interface IJobTriggerInfo {
        string TriggerName { get; }
    }

    internal class JobDetailTriggerInfo {
        readonly Type _jobType;
        readonly string _triggerName;
        //        readonly LinkType _linkType;
        readonly string _jobName;

        public JobDetailTriggerInfo(string jobName, Type jobType, string triggerName) {
            _jobName = jobName;
            _jobType = jobType;
            _triggerName = triggerName;

            //            _jobListenerTriggerLink = supportSchedulerLink is IJobDetailJobListenerTriggerLink;
            //            if (!_jobListenerTriggerLink) {
            //                _triggerName = ((IJobDetailTriggerLink)supportSchedulerLink).JobTrigger.Name;
            //            }
            //            JobName = supportSchedulerLink.JobDetail.Name;
            //
            //            if (_jobListenerTriggerLink) {
            //                var jobListenerTrigger = ((IJobDetailJobListenerTriggerLink)supportSchedulerLink).JobListenerTrigger;
            //                JobType = jobListenerTrigger.JobType;
            //                _listenerEvent = jobListenerTrigger.Event;
            //            } else JobType = supportSchedulerLink.JobDetail.Job.JobType;
            //            OriginType = supportSchedulerLink.JobDetail.Job.JobType;
            //            _linkType = linkType;
        }



        //        public string TriggerName {
        //            get { return _triggerName; }
        //        }

        //        public LinkType LinkType {
        //            get { return _linkType; }
        //        }

        public string TriggerName {
            get { return _triggerName; }
        }

        public string JobName {
            get { return _jobName; }
        }

        public Type JobType {
            get { return _jobType; }

        }

        //        public Type OriginType { get; set; }
    }
}