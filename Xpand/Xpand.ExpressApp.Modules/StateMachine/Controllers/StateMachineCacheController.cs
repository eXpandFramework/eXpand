using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class StateMachineCacheController : DevExpress.ExpressApp.StateMachine.StateMachineCacheController {
        Type stateMachineStorageType;
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
        readonly List<IStateMachine> cache = new List<IStateMachine>();
        internal bool isCompleteCache = false;

        public new ReadOnlyCollection<IStateMachine> CachedStateMachines {
            get { return cache.AsReadOnly(); }
        }

        public new void ClearCache() {
            cache.Clear();
            isCompleteCache = false;
        }

        void EnsureCache() {
            if (!isCompleteCache) {
                if (stateMachineStorageType == null) {
                    stateMachineStorageType =
                        ((StateMachineModule) Application.Modules.FindModule(typeof (StateMachineModule)))
                            .StateMachineStorageType;
                }
                foreach (object stateMachineObject in GetObjectSpace(stateMachineStorageType).GetObjects(stateMachineStorageType, null)) {
                    var stateMachine = (IStateMachine) stateMachineObject;
                    cache.Add(stateMachine);
                }
                isCompleteCache = true;
            }
        }

        public override IList<IStateMachine> GetStateMachinesByType(Type targetObjectType) {
            EnsureCache();
            return cache.Where(stateMachine => stateMachine.Active && stateMachine.TargetObjectType.IsAssignableFrom(targetObjectType)).ToList();
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
            GetObjectSpace(stateMachineStorageType).Reloaded -= ObjectSpace_Reloaded;
            base.OnDeactivated();
            ClearCache();
        }

        void ObjectSpace_Reloaded(object sender, EventArgs e) {
            ClearCache();
        }
    }
}