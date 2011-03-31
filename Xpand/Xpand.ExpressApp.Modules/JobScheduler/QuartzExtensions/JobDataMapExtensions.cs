using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.ExpressApp.DC;
using Quartz;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.JobScheduler.QuartzExtensions {
    public static class JobDataMapExtensions {
        public static object Put<T>(this JobDataMap jobDataMap, object key,object val) where T : IDataMap {
            return jobDataMap.Put(GetKey<T>(key), val);
        }
        public static object Put<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property, object val) where T : IDataMap {
            return jobDataMap.Put(GetKey(property), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, bool val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property, bool val) where T : IDataMap {
            jobDataMap.Put(GetKey(property), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, char val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property, char val) where T : IDataMap {
            jobDataMap.Put(GetKey(property), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, double val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property, double val) where T : IDataMap {
            jobDataMap.Put(GetKey(property), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, float val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property, float val) where T : IDataMap {
            jobDataMap.Put(GetKey(property), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, int val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property, int val) where T : IDataMap {
            jobDataMap.Put(GetKey(property), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, long val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property, long val) where T : IDataMap {
            jobDataMap.Put(GetKey(property), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, object key, string val) where T : IDataMap {
            jobDataMap.Put(GetKey<T>(key), val);
        }
        public static void Put<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property, string val) where T : IDataMap {
            jobDataMap.Put(GetKey(property), val);
        }

        public static E GetEnum<T, E>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return (E)jobDataMap.Get<T>(key);
        }
        public static E GetEnum<T, E>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return (E)jobDataMap.Get<T>(GetKey(property));
        }

        public static string GetString<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetString(GetKey<T>(key));
        }
        public static string GetString<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetString(GetKey(property));
        }

        static string GetKey<T>(Expression<Func<T, object>> property) {
            T key = default(T);
            string name = key.GetMemberInfo(property).Name;
            return GetKey<T>(name);
        }

        public static long GetLong<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetLong(GetKey<T>(key));
        }
        public static long GetLong<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetLong(GetKey(property));
        }

        public static int GetInt<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetInt(GetKey<T>(key));
        }
        public static int GetInt<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetInt(GetKey(property));
        }

        public static float GetFloat<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetFloat(GetKey<T>(key));
        }
        public static float GetFloat<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetFloat(GetKey(property));
        }

        public static double GetDouble<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetDouble(GetKey<T>(key));
        }
        public static double GetDouble<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetDouble(GetKey(property));
        }

        public static DateTime GetDateTime<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetDateTime(GetKey<T>(key));
            
        }
        public static DateTime GetDateTime<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetDateTime(GetKey(property));
            
        }

        public static char GetCharFromString<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetCharFromString(GetKey<T>(key));
        }
        public static char GetCharFromString<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetCharFromString(GetKey(property));
        }

        public static char GetChar<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetChar(GetKey<T>(key));
        }
        public static char GetChar<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetChar(GetKey(property));
        }

        public static bool GetBooleanValue<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetBooleanValue(GetKey<T>(key));
            
        }
        public static bool GetBooleanValue<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetBooleanValue(GetKey(property));
            
        }
        public static bool GetBooleanValueFromString<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetBooleanValueFromString(GetKey<T>(key));
        }
        public static bool GetBooleanValueFromString<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetBooleanValueFromString(GetKey(property));
        }

        public static bool GetBooleanFromString<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetBooleanFromString(GetKey<T>(key));
            
        }
        public static bool GetBooleanFromString<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetBooleanFromString(GetKey(property));
            
        }

        static string GetKey<T>(object key) {
            return string.Format("{0}:{1}", typeof(T).Name, key);
        }

        public static bool GetBoolean<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.GetBoolean(GetKey<T>(key));
        }
        public static bool GetBoolean<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.GetBoolean(GetKey(property));
        }

        public static object Get<T>(this JobDataMap jobDataMap, object key) where T : IDataMap {
            return jobDataMap.Get(GetKey<T>(key));
            
        }
        public static object Get<T>(this JobDataMap jobDataMap, Expression<Func<T, object>> property) where T : IDataMap {
            return jobDataMap.Get(GetKey(property));
            
        }

        public static void MapValue(this JobDataMap jobDataMap, object currentObject, IMemberInfo info) {
            jobDataMap.Put(string.Format("{0}:{1}", currentObject.GetType().Name, info.Name), GetValue(currentObject, info));
        }

        static object GetValue(object currentObject, IMemberInfo info) {
            return info.MemberTypeInfo.IsPersistent ? info.MemberTypeInfo.KeyMember.GetValue(info.GetValue(currentObject)) : info.GetValue(currentObject);
        }


        public static List<JobKey> GetJobListenerNames(this JobDataMap jobDataMap, JobListenerEvent jobListenerEvent) {
            return jobDataMap.GetListenerNames(SchedulerExtensions.TriggerJobListenersOn, jobListenerEvent);
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
            jobDataMap.CreateListenersKeys(SchedulerExtensions.TriggerJobListenersOn, listenerEvent, args);
        }

        static void CreateListenersKeys(this JobDataMap jobDataMap, string s, Enum listenerEvent, IEnumerable<JobKey> args) {
            jobDataMap[s + listenerEvent] = (args.Aggregate<JobKey, string>(null, (current, key) => current + (key.Name + "|" + key.Group + ";")) + "").Trim(';');
        }

        public static List<JobKey> GetTriggerListenerNames(this JobDataMap jobDataMap, TriggerListenerEvent triggerListenerEvent) {
            return GetListenerNames(jobDataMap, SchedulerExtensions.TriggerTriggerJobListenersOn, triggerListenerEvent);
        }

        public static void CreateTriggerListenersKey(this JobDataMap jobDataMap, TriggerListenerEvent listenerEvent, params JobKey[] keys) {
            jobDataMap.CreateListenersKeys(SchedulerExtensions.TriggerTriggerJobListenersOn, listenerEvent, keys);
        }

    }
}