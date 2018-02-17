using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using Fasterflect;

namespace Xpand.ExpressApp.Security.Permissions {
    public interface IPermissionInfo {
        IEnumerable<IOperationPermission> GetPermissions(ISecurityRole securityRole);
    }
    public static class PermissionRequestProcessorStorage {
        private static readonly string ValueManagerKey = typeof(PermissionRequestProcessorStorage).Name;
        private static volatile IValueManager<Dictionary<Type,IPermissionRequestProcessor>> _instanceValueManager;
        private static readonly object SyncRoot = new object();

        public static Dictionary<Type, IPermissionRequestProcessor> Instance {
            get {
                if (_instanceValueManager == null) {
                    lock (SyncRoot) {
                        if (_instanceValueManager == null) {
                            _instanceValueManager = ValueManager.GetValueManager<Dictionary<Type, IPermissionRequestProcessor>>(ValueManagerKey);
                        }
                    }
                }
	            if (_instanceValueManager.CanManageValue){

		            if (_instanceValueManager.Value == null) {
			            lock (SyncRoot) {
				            if (_instanceValueManager.Value == null) {
					            _instanceValueManager.Value = new Dictionary<Type, IPermissionRequestProcessor>();
				            }
			            }
		            }
		            return _instanceValueManager.Value;
	            }
	            return new Dictionary<Type, IPermissionRequestProcessor>();
            }
        }

        public static T GetProcessor<T>(this IPermissionDictionary customPermissions) where T : ICustomPermissionRequestProccesor {
            if (!Instance.ContainsKey(typeof(T)))
                Instance.Add(typeof(T), (IPermissionRequestProcessor)typeof(T).CreateInstance());
            var customPermissionRequestProccesor = (ICustomPermissionRequestProccesor)Instance[typeof(T)];
            customPermissionRequestProccesor.Permissions = customPermissions;
            return (T)customPermissionRequestProccesor;
        }
    }

    public class PermissionProviderStorage : HashSet<IPermissionInfo> {
        static PermissionProviderStorage() {
            Instance = new PermissionProviderStorage();
        }

        public static PermissionProviderStorage Instance { get; }
    }
}