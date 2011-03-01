using System.Collections.Generic;
using System.Linq;
using Quartz;
using Quartz.Util;
using Xpand.ExpressApp.JobScheduler.Qaurtz;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler {
    public class JobDataMapKeyCalculator {


        public List<Key> GetListenerNames(JobDataMap jobDataMap, JobListenerEvent jobListenerEvent) {
            var jobData = jobDataMap[XpandScheduler.TriggerJobListenersOn + jobListenerEvent] as string;
            if (jobData != null) {
                var listenerNames = jobData.Split(';').ToList();
                return listenerNames.Select(name => name.Split('|')).Select(strings => new Key(strings[0], strings[1])).ToList();
            }
            return new List<Key>();
        }

        public void CreateListenersKey(JobDataMap jobDataMap, JobListenerEvent listenerEvent, params Key[] args) {
            jobDataMap[XpandScheduler.TriggerJobListenersOn + listenerEvent] = (args.Aggregate<Key, string>(null, (current, key) => current + (key.Name + "|" + key.Group + ";")) + "").Trim(';');
        }
    }
}