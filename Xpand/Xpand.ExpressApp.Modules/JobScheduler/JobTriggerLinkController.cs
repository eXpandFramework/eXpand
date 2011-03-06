using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using Quartz;
using Quartz.Util;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobTriggerLinkController : SupportSchedulerController {
        public JobTriggerLinkController() {
            TargetObjectType = typeof(IJobDetail);
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
            var jobDetails = ObjectSpace.GetObjects(View.ObjectTypeInfo.Type, ForTheSameGroup(jobSchedulerGroupTriggerLink)).OfType<IJobDetail>();
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
            List<IJobDetail> jobDetails = GetRelatedJobDetails(ForTheSameJobTypeOrGroup(link.TriggerListenerTrigger.JobType,link.TriggerListenerTrigger.Group));
            var calculateTriggerListenerNames = CalculateTriggerListenerNames(link.TriggerListenerTrigger.Event);
            var triggerListenersKeys = CreateTriggerListenersKeys(link.TriggerListenerTrigger.Event);
            jobDetails.ForEach(detail => GetListenerDataMap(detail, AddListener, detail.Name, detail.Job.JobType, calculateTriggerListenerNames, triggerListenersKeys));
        }

        Action<JobDataMap, List<Key>, JobDataMapKeyCalculator> CreateTriggerListenersKeys(TriggerListenerEvent listenerEvent) {
            return (map, list, calculator) => calculator.CreateTriggerListenersKey(map, listenerEvent, list.ToArray());
        }

        Func<JobDataMap, JobDataMapKeyCalculator, List<Key>> CalculateTriggerListenerNames(TriggerListenerEvent triggerListenerEvent) {
            return (map, calculator) => calculator.GetTriggerListenerNames(map, triggerListenerEvent);
        }

        void AddJobListeners(IJobDetailJobListenerTriggerLink link) {
            List<IJobDetail> jobDetails = GetRelatedJobDetails(ForTheSameJobTypeOrGroup(link.JobListenerTrigger.JobType, link.JobListenerTrigger.Group));
            var calculateJobListenerNames = CalculateJobListenerNames(link.JobListenerTrigger.Event);
            var listenersKeyAction = CreateJobListenersKeys(link.JobListenerTrigger.Event);
            jobDetails.ForEach(detail => GetListenerDataMap(detail, AddListener, link.JobDetail.Name, link.JobDetail.Job.JobType, calculateJobListenerNames, listenersKeyAction));
        }

        Action<JobDataMap, List<Key>, JobDataMapKeyCalculator> CreateJobListenersKeys(JobListenerEvent jobListenerEvent) {
            return (map, list, calculator) => calculator.CreateJobListenersKey(map, jobListenerEvent, list.ToArray());
        }

        Func<JobDataMap, JobDataMapKeyCalculator, List<Key>> CalculateJobListenerNames(JobListenerEvent listenerEvent) {
            return (map, calculator) => calculator.GetJobListenerNames(map, listenerEvent);
        }

        Func<CriteriaOperator> ForTheSameJobTypeOrGroup(Type type, IJobSchedulerGroup group) {
            return () => CriteriaOperator.Parse("Job.JobType=? OR (Group Is Not Null AND Group=?)", type,group);
        }

        List<IJobDetail> GetRelatedJobDetails(Func<CriteriaOperator> action) {
            var type = TypesInfo.FindBussinessObjectType<IJobDetail>();
            return ObjectSpace.GetObjects(type, action.Invoke()).OfType<IJobDetail>().ToList();
        }

        void Delete() {
            supportJobDetails.OfType<ListenerInfo>().ToList().ForEach(RemoveListeners);
            supportJobDetails.OfType<JobDetailTriggerInfo>().ToList().ForEach(UnscheduleJob);
            supportJobDetails.Clear();
        }

        void UnscheduleJob(JobDetailTriggerInfo jobTriggerInfo) {
            var jobGroup = jobTriggerInfo is JobGroupInfo ? ((JobGroupInfo)jobTriggerInfo).Group : null;
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
            Scheduler.ScheduleJob(link.JobTrigger, link.JobDetail, Mapper, null);
        }

        void ScheduleGroup(IJobSchedulerGroupTriggerLink link) {
            var relatedJobDetails = GetRelatedJobDetails(() => ForTheSameGroup(link));
            relatedJobDetails.ForEach(detail => Scheduler.ScheduleJob(link.Trigger, detail, Mapper, link.JobSchedulerGroup.Name));
        }

        CriteriaOperator ForTheSameGroup(IJobSchedulerGroupTriggerLink link) {
            return CriteriaOperator.Parse("Group.Name=?", link.JobSchedulerGroup.Name);
        }

        void RemoveListeners(ListenerInfo listenerInfo) {
            List<IJobDetail> jobDetails = GetRelatedJobDetails(ForTheSameJobTypeOrGroup(listenerInfo.JobType, listenerInfo.Group));
            Func<JobDataMap, JobDataMapKeyCalculator, List<Key>> calculateJobListenerNames;
            Action<JobDataMap, List<Key>, JobDataMapKeyCalculator> listenersKeyAction;
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

        void RemoveListener(List<Key> list, Key key) {
            list.Remove(key);
        }

        void AddListener(List<Key> list, Key key) {
            list.Add(key);
        }

        void GetListenerDataMap(IJobDetail detail, Action<List<Key>, Key> action, string jobName, Type jobType, Func<JobDataMap, JobDataMapKeyCalculator, List<Key>> getListeners, Action<JobDataMap, List<Key>, JobDataMapKeyCalculator> createListenersKeyAction) {
            var jobDetail = Scheduler.GetJobDetail(detail);
            var jobDataMapKeyCalculator = new JobDataMapKeyCalculator();
            var listenerNames = getListeners.Invoke(jobDetail.JobDataMap, jobDataMapKeyCalculator);
            action.Invoke(listenerNames, new Key(jobName, jobType.FullName));
            createListenersKeyAction.Invoke(jobDetail.JobDataMap, listenerNames, jobDataMapKeyCalculator);
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

        public ListenerInfo(string jobName, Type jobType, Enum listenerEvent, Type originType,IJobSchedulerGroup group)
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