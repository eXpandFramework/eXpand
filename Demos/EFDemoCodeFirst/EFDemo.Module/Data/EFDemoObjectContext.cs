using System;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.ComponentModel;
using System.Collections.Generic;

namespace EFDemo.Module.Data {
	public partial class EFDemoObjectContext {
		private void EFDemoObjectContext_ObjectMaterialized(Object sender, ObjectMaterializedEventArgs e) {
			if(e.Entity is Event) {
				((Event)e.Entity).Resources.Load();
			}
			else if(e.Entity is Resource) {
				((Resource)e.Entity).Events.Load();
			}
			else if(e.Entity is Analysis) {
				((Analysis)e.Entity).UpdateDimensionProperties();
			}
		}
		private void ObjectStateManager_ObjectStateManagerChanged(Object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Add) {
				if(e.Element is Event) {
					((Event)e.Element).objectContext = this;
				}
			}
		}
		partial void OnContextCreated() {
			ObjectMaterialized += new ObjectMaterializedEventHandler(EFDemoObjectContext_ObjectMaterialized);
			ObjectStateManager.ObjectStateManagerChanged += new CollectionChangeEventHandler(ObjectStateManager_ObjectStateManagerChanged);
		}
		protected override void Dispose(Boolean disposing) {
			base.Dispose(disposing);
			ObjectMaterialized -= new ObjectMaterializedEventHandler(EFDemoObjectContext_ObjectMaterialized);
			if(ObjectStateManager != null) {
				ObjectStateManager.ObjectStateManagerChanged -= new CollectionChangeEventHandler(ObjectStateManager_ObjectStateManagerChanged);
			}
		}
		public override int SaveChanges(SaveOptions options) {
			foreach(ObjectStateEntry objectStateEntry in ObjectStateManager.GetObjectStateEntries(EntityState.Added)) {
				if(objectStateEntry.Entity is Event) {
					((Event)objectStateEntry.Entity).BeforeSave();
				}
			}
			return base.SaveChanges(options);
		}
	}
}
