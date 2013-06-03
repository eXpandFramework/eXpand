using System;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Data.Common;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System.ComponentModel;

namespace EFDemo.Module.Data {
	public class EFDemoDbContext : DbContext {
		private ObjectContext objectContext;
		private void ObjectContext_ObjectMaterialized(Object sender, ObjectMaterializedEventArgs e) {
			if(e.Entity is Event) {
				Int32 count = ((Event)e.Entity).Resources.Count;
			}
			else if(e.Entity is Resource) {
				Int32 count = ((Resource)e.Entity).Events.Count;
			}
		}
		private void ObjectStateManager_ObjectStateManagerChanged(Object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Add) {
				if(e.Element is Event) {
					((Event)e.Element).objectContext = objectContext;
				}
			}
		}
		private void ObjectContext_SavingChanges(object sender, EventArgs e) {
			foreach(ObjectStateEntry objectStateEntry in ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added)) {
				if(objectStateEntry.Entity is Event) {
					((Event)objectStateEntry.Entity).BeforeSave();
				}
			}
		}
		private void Init() {
			objectContext = ((IObjectContextAdapter)this).ObjectContext;
			objectContext.SavingChanges += new EventHandler(ObjectContext_SavingChanges);
			objectContext.ObjectMaterialized += new ObjectMaterializedEventHandler(ObjectContext_ObjectMaterialized);
			objectContext.ObjectStateManager.ObjectStateManagerChanged += new CollectionChangeEventHandler(ObjectStateManager_ObjectStateManagerChanged);
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<FileData>().ToTable("FileData");
			modelBuilder.Entity<Person>().ToTable("Parties_Person");
			modelBuilder.Entity<Contact>().ToTable("Parties_Contact");
			modelBuilder.Entity<Organization>().ToTable("Parties_Organization");
			modelBuilder.Entity<PortfolioFileData>().ToTable("FileAttachments_PortfolioFileData");
			modelBuilder.Entity<DemoTask>().ToTable("Tasks_DemoTask");
			modelBuilder.Entity<Analysis_EF>().ToTable("Analysis");
			modelBuilder.Entity<ReportData_EF>().ToTable("ReportData_EF");
//			modelBuilder.Entity<ModuleInfo_EF>().ToTable("ModulesInfo");

			modelBuilder.Entity<DemoTask>().HasMany(t => t.Contacts).WithMany(c => c.Tasks).Map(mc => {
				mc.ToTable("DemoTasks_Contacts");
				mc.MapLeftKey("Tasks_ID");
				mc.MapRightKey("Contacts_ID");
			});

			modelBuilder.Entity<Department>().HasMany(d => d.Positions).WithMany(p => p.Departments).Map(mc => {
				mc.ToTable("Departments_Positions");
				mc.MapLeftKey("Departments_ID");
				mc.MapRightKey("Positions_ID");
			});

			modelBuilder.Entity<Event>().HasMany(e => e.Resources).WithMany(r => r.Events).Map(mc => {
				mc.ToTable("Events_Resources");
				mc.MapLeftKey("Events_ID");
				mc.MapRightKey("Resources_Key");
			});

			modelBuilder.Entity<User>().HasMany(u => u.Roles).WithMany(r => r.Users).Map(mc => {
				mc.ToTable("Users_Roles");
				mc.MapLeftKey("Users_ID");
				mc.MapRightKey("Roles_ID");
			});

			modelBuilder.Entity<Role>().HasMany(r => r.ChildRoles).WithMany(r => r.ParentRoles).Map(mc => {
				mc.ToTable("Roles_Roles");
				mc.MapLeftKey("ParentRoles_ID");
				mc.MapRightKey("ChildRoles_ID");
			});
		}
		protected override void Dispose(Boolean disposing) {
			base.Dispose(disposing);
			if(objectContext != null) {
				objectContext.SavingChanges -= new EventHandler(ObjectContext_SavingChanges);
				objectContext.ObjectMaterialized -= new ObjectMaterializedEventHandler(ObjectContext_ObjectMaterialized);
				if(objectContext.ObjectStateManager != null) {
					objectContext.ObjectStateManager.ObjectStateManagerChanged -= new CollectionChangeEventHandler(ObjectStateManager_ObjectStateManagerChanged);
				}
				objectContext = null;
			}
		}

		public EFDemoDbContext(String connectionString)
			: base(connectionString) {
			Init();
		}
		public EFDemoDbContext(DbConnection connection)
			: base(connection, false) {
			Init();
		}

		public DbSet<Address> Addresses { get; set; }
//		public DbSet<Analysis> Analysis { get; set; }
		public DbSet<Country> Countries { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Event> Events { get; set; }
		public DbSet<FileAttachment> FileAttachments { get; set; }
		public DbSet<FileData> FileData { get; set; }
		public DbSet<HCategory> HCategories { get; set; }
//		public DbSet<ModuleInfo_EF> ModulesInfo { get; set; }
		public DbSet<Note> Notes { get; set; }
		public DbSet<Party> Parties { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<PhoneNumber> PhoneNumbers { get; set; }
		public DbSet<Position_EF> Positions { get; set; }
		public DbSet<ReportData_EF> ReportData { get; set; }
		public DbSet<Resource> Resources { get; set; }
		public DbSet<Resume> Resumes { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<State> States { get; set; }
		public DbSet<Task> Tasks { get; set; }
		public DbSet<TypePermissionObject> TypePermissionObjects { get; set; }
		public DbSet<User> Users { get; set; }
	}
}
