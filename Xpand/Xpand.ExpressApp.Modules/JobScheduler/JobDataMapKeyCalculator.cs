using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using Quartz.Util;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobDataMapKeyCalculator {


        public List<Key> GetJobListenerNames(JobDataMap jobDataMap, JobListenerEvent jobListenerEvent) {
            return GetListenerNames(jobDataMap, XpandScheduler.TriggerJobListenersOn, jobListenerEvent);
        }

        List<Key> GetListenerNames(JobDataMap jobDataMap, string triggerJobListenersOn, Enum listenerEvent) {
            var jobData = jobDataMap[triggerJobListenersOn + listenerEvent] as string;
            if (jobData != null) {
                var listenerNames = jobData.Split(';').ToList();
                return listenerNames.Select(name => name.Split('|')).Select(strings => new Key(strings[0], strings[1])).ToList();
            }
            return new List<Key>();
        }

        public void CreateJobListenersKey(JobDataMap jobDataMap, JobListenerEvent listenerEvent, params Key[] args) {
            CreateListenersKeys(XpandScheduler.TriggerJobListenersOn, jobDataMap,listenerEvent,args);
        }

        void CreateListenersKeys(string s, JobDataMap jobDataMap, Enum listenerEvent, IEnumerable<Key> args) {
            jobDataMap[s + listenerEvent] = (args.Aggregate<Key, string>(null, (current, key) => current + (key.Name + "|" + key.Group + ";")) + "").Trim(';');
        }

        public List<Key> GetTriggerListenerNames(JobDataMap jobDataMap, TriggerListenerEvent triggerListenerEvent) {
            return GetListenerNames(jobDataMap, XpandScheduler.TriggerTriggerJobListenersOn, triggerListenerEvent);
        }

        public void CreateTriggerListenersKey(JobDataMap jobDataMap, TriggerListenerEvent listenerEvent, params Key[] keys) {
            CreateListenersKeys(XpandScheduler.TriggerTriggerJobListenersOn, jobDataMap, listenerEvent, keys);
        }
    }
}