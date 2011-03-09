using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Quartz;
using Quartz.Util;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public static class JobDataMapExtensions {
        public static void MapValue(this JobDataMap jobDataMap, object currentObject, IMemberInfo info) {
            jobDataMap.Put(info.Name, info.GetValue(currentObject));
        }


        public static List<Key> GetJobListenerNames(this JobDataMap jobDataMap, JobListenerEvent jobListenerEvent) {
            return jobDataMap.GetListenerNames(XpandScheduler.TriggerJobListenersOn, jobListenerEvent);
        }

        static List<Key> GetListenerNames(this JobDataMap jobDataMap, string triggerJobListenersOn, Enum listenerEvent) {
            var jobData = jobDataMap[triggerJobListenersOn + listenerEvent] as string;
            if (jobData != null) {
                var listenerNames = jobData.Split(';').ToList();
                return listenerNames.Select(name => name.Split('|')).Select(strings => new Key(strings[0], strings[1])).ToList();
            }
            return new List<Key>();
        }

        public static void CreateJobListenersKey(this JobDataMap jobDataMap, JobListenerEvent listenerEvent, params Key[] args) {
            jobDataMap.CreateListenersKeys(XpandScheduler.TriggerJobListenersOn, listenerEvent, args);
        }

        static void CreateListenersKeys(this JobDataMap jobDataMap, string s, Enum listenerEvent, IEnumerable<Key> args) {
            jobDataMap[s + listenerEvent] = (args.Aggregate<Key, string>(null, (current, key) => current + (key.Name + "|" + key.Group + ";")) + "").Trim(';');
        }

        public static List<Key> GetTriggerListenerNames(this JobDataMap jobDataMap, TriggerListenerEvent triggerListenerEvent) {
            return GetListenerNames(jobDataMap, XpandScheduler.TriggerTriggerJobListenersOn, triggerListenerEvent);
        }

        public static void CreateTriggerListenersKey(this JobDataMap jobDataMap, TriggerListenerEvent listenerEvent, params Key[] keys) {
            jobDataMap.CreateListenersKeys(XpandScheduler.TriggerTriggerJobListenersOn, listenerEvent, keys);
        }

    }
}