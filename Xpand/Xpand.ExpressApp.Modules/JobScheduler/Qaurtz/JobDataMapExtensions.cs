using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using Quartz;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public static class JobDataMapExtensions {
        public static object Put<T>(this JobDataMap jobDataMap, object key,object val) where T : IDataMap {
            return jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, bool val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, char val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, double val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, float val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, int val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, long val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, string val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }

        public static string GetString<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetString(GetKey<T>(key));
        }

        public static long GetLong<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetLong(GetKey<T>(key));
        }

        public static int GetInt<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetInt(GetKey<T>(key));
        }

        public static float GetFloat<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetFloat(GetKey<T>(key));
        }

        public static double GetDouble<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetDouble(GetKey<T>(key));
        }

        public static DateTime GetDateTime<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetDateTime(GetKey<T>(key));
            
        }

        public static char GetCharFromString<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetCharFromString(GetKey<T>(key));
        }

        public static char GetChar<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetChar(GetKey<T>(key));
        }

        public static bool GetBooleanValue<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetBooleanValue(GetKey<T>(key));
            
        }
        public static bool GetBooleanValueFromString<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetBooleanValueFromString(GetKey<T>(key));
        }

        public static bool GetBooleanFromString<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetBooleanFromString(GetKey<T>(key));
            
        }

        static string GetKey<T>(object key) {
            return string.Format("{0}:{1}", typeof(T).Name, key);
        }

        public static bool GetBoolean<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetBoolean(GetKey<T>(key));
        }

        public static object Get<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.Get(GetKey<T>(key));
            
        }

        public static void MapValue(this JobDataMap jobDataMap, object currentObject, IMemberInfo info) {
            jobDataMap.Put(string.Format("{0}:{1}", currentObject.GetType().Name, info.Name), info.GetValue(currentObject));
        }


        public static List<JobKey> GetJobListenerNames(this JobDataMap jobDataMap, JobListenerEvent jobListenerEvent) {
            return jobDataMap.GetListenerNames(XpandScheduler.TriggerJobListenersOn, jobListenerEvent);
        }

        static List<JobKey> GetListenerNames(this JobDataMap jobDataMap, string triggerJobListenersOn, Enum listenerEvent) {
            var jobData = jobDataMap[triggerJobListenersOn + listenerEvent] as string;
            if (jobData != null) {
                var listenerNames = jobData.Split(';').ToList();
                return listenerNames.Select(name => name.Split('|')).Select(strings => new JobKey(strings[0], strings.Length > 1 ? strings[1] : "")).ToList();
            }
            return new List<JobKey>();
        }


        public static void CreateJobListenersKey(this JobDataMap jobDataMap, JobListenerEvent listenerEvent, params JobKey[] args) {
            jobDataMap.CreateListenersKeys(XpandScheduler.TriggerJobListenersOn, listenerEvent, args);
        }

        static void CreateListenersKeys(this JobDataMap jobDataMap, string s, Enum listenerEvent, IEnumerable<JobKey> args) {
            jobDataMap[s + listenerEvent] = (args.Aggregate<JobKey, string>(null, (current, key) => current + (key.Name + "|" + key.Group + ";")) + "").Trim(';');
        }

        public static List<JobKey> GetTriggerListenerNames(this JobDataMap jobDataMap, TriggerListenerEvent triggerListenerEvent) {
            return GetListenerNames(jobDataMap, XpandScheduler.TriggerTriggerJobListenersOn, triggerListenerEvent);
        }

        public static void CreateTriggerListenersKey(this JobDataMap jobDataMap, TriggerListenerEvent listenerEvent, params JobKey[] keys) {
            jobDataMap.CreateListenersKeys(XpandScheduler.TriggerTriggerJobListenersOn, listenerEvent, keys);
        }

    }
}