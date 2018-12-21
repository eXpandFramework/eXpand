using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using Fasterflect;

namespace Xpand.ExpressApp.Security.Permissions {
    public interface IPermissionInfo {
        IEnumerable<IOperationPermission> GetPermissions(ISecurityRole securityRole);
    }
    public static class PermissionRequestProcessorStorage {
        static PermissionRequestProcessorStorage() {
            Instance = new ConcurrentDictionary<Type, IPermissionRequestProcessor>();
        } 


        public static T GetProcessor<T>(this IPermissionDictionary customPermissions) where T : ICustomPermissionRequestProccesor {
            if (!Instance.ContainsKey(typeof(T)))
                Instance.TryAdd(typeof(T), (IPermissionRequestProcessor)typeof(T).CreateInstance());
            var customPermissionRequestProccesor = (ICustomPermissionRequestProccesor)Instance[typeof(T)];
            customPermissionRequestProccesor.Permissions = customPermissions;
            return (T)customPermissionRequestProccesor;
        }

        public static ConcurrentDictionary<Type, IPermissionRequestProcessor> Instance { get; }
    }

    public class PermissionProviderStorage : HashSet<IPermissionInfo> {
        static PermissionProviderStorage() {
            Instance = new PermissionProviderStorage();
        }

        public static PermissionProviderStorage Instance { get; }
    }
}