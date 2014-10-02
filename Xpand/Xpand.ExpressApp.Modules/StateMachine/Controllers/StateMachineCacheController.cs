using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class StateMachineCacheController : DevExpress.ExpressApp.StateMachine.StateMachineCacheController {
        Type _stateMachineStorageType;
#if DebugTest
		internal StateMachineCacheController(Type stateMachineStorageType)
			: base() {
			this.stateMachineStorageType = stateMachineStorageType;
		}
		public List<IStateMachine> Cache {
			get {
				return cache;
			}
		}
#endif
        readonly List<IStateMachine> _cache = new List<IStateMachine>();
        internal bool IsCompleteCache = false;

        public new ReadOnlyCollection<IStateMachine> CachedStateMachines {
            get { return _cache.AsReadOnly(); }
        }

        public new void ClearCache() {
            _cache.Clear();
            IsCompleteCache = false;
        }

        void EnsureCache() {
            if (!IsCompleteCache) {
                if (_stateMachineStorageType == null) {
                    _stateMachineStorageType =
                        ((StateMachineModule) Application.Modules.FindModule(typeof (StateMachineModule)))
                            .StateMachineStorageType;
                }
                foreach (object stateMachineObject in GetObjectSpace(_stateMachineStorageType).GetObjects(_stateMachineStorageType, null)) {
                    var stateMachine = (IStateMachine) stateMachineObject;
                    _cache.Add(stateMachine);
                }
                IsCompleteCache = true;
            }
        }

        public override IList<IStateMachine> GetStateMachinesByType(Type targetObjectType) {
            EnsureCache();
            return _cache.Where(stateMachine => stateMachine.Active && stateMachine.TargetObjectType.IsAssignableFrom(targetObjectType)).ToList();
        }

        protected override void OnActivated() {
            base.OnActivated();
            GetObjectSpace(((StateMachineModule)Application.Modules.FindModule(typeof(StateMachineModule)))
                            .StateMachineStorageType).Reloaded += ObjectSpace_Reloaded;
        }

        IObjectSpace GetObjectSpace(Type type) {
            return Application.CreateObjectSpace(type);
        }

        protected override void OnDeactivated() {
            GetObjectSpace(_stateMachineStorageType).Reloaded -= ObjectSpace_Reloaded;
            base.OnDeactivated();
            ClearCache();
        }

        void ObjectSpace_Reloaded(object sender, EventArgs e) {
            ClearCache();
        }
    }
}