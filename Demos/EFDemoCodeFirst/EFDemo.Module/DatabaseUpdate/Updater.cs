using System;

using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Reports;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.General;
#if (CodeFirst)
using DevExpress.Persistent.BaseImpl.EF;
#endif
using EFDemo.Module.Data;

namespace EFDemo.Module.DatabaseUpdate {
	public class Updater : ModuleUpdater {
		private void CreateReport(String reportName, Type dataType) {
			ReportData reportdata = ObjectSpace.FindObject<ReportData>(new BinaryOperator("ReportName", reportName));
			if(reportdata == null) {
				reportdata = ObjectSpace.CreateObject<ReportData>();
				XafReport rep = new XafReport();
				rep.ObjectSpace = ObjectSpace;
				rep.LoadLayout(GetType().Assembly.GetManifestResourceStream("EFDemo.Module.EmbeddedReports." + reportName + ".repx"));
				rep.ReportName = reportName;
				rep.DataType = dataType;
				if(dataType == typeof(DemoTask)) {
					rep.Filtering.Filter = "[AssignedTo] = ?";
				}
				reportdata.SaveReport(rep);
				reportdata.IsInplaceReport = true;
			}
		}
		private Role CreateDefaultRole() {
			Role defaultRole = ObjectSpace.FindObject<Role>(new BinaryOperator("Name", "Default"));
			if(defaultRole == null) {
				defaultRole = ObjectSpace.CreateObject<Role>();
				defaultRole.Name = "Default";
				/*defaultRole.AddObjectAccessPermission<User>("[Oid] = CurrentUserId()", SecurityOperations.ReadOnlyAccess);
				defaultRole.AddMemberAccessPermission<User>("ChangePasswordOnFirstLogon, StoredPassword", SecurityOperations.Write);
				defaultRole.SetTypePermissions<Role>(SecurityOperations.Read, true, true);
				defaultRole.SetTypePermissions<AuditDataItemPersistent>(SecurityOperations.ModificationAccess, true, true);*/
			}
			return defaultRole;
		}
		private void CreateDataToBeAnalysed() {
			Analysis taskAnalysis1 = ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Completed tasks'"));
			if(taskAnalysis1 == null) {
				taskAnalysis1 = ObjectSpace.CreateObject<Analysis>();
				taskAnalysis1.Name = "Completed tasks";
				taskAnalysis1.ObjectTypeName = typeof(DemoTask).FullName;
				/*taskAnalysis1.Criteria = "[Status] = 'Completed'";*/
				taskAnalysis1.Criteria = "[Status_Int] = " + ((Int32)TaskStatus.Completed).ToString();
			}
			Analysis taskAnalysis2 = ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Estimated and actual work comparison'"));
			if(taskAnalysis2 == null) {
				taskAnalysis2 = ObjectSpace.CreateObject<Analysis>();
				taskAnalysis2.Name = "Estimated and actual work comparison";
				taskAnalysis2.ObjectTypeName = typeof(DemoTask).FullName;
			}
		}
		public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
		public override void UpdateDatabaseAfterUpdateSchema() {
			base.UpdateDatabaseAfterUpdateSchema();

			Role defaultRole = CreateDefaultRole();

			// Create reports
			CreateReport("ContactsGroupedByPosition", typeof(Contact));
			CreateReport("TasksStateReport", typeof(DemoTask));

			Position developerPosition = ObjectSpace.FindObject<Position>(CriteriaOperator.Parse("Title == 'Developer'"));
			if(developerPosition == null) {
				developerPosition = ObjectSpace.CreateObject<Position>();
				developerPosition.Title = "Developer";
			}
			Position managerPosition = ObjectSpace.FindObject<Position>(CriteriaOperator.Parse("Title == 'Manager'"));
			if(managerPosition == null) {
				managerPosition = ObjectSpace.CreateObject<Position>();
				managerPosition.Title = "Manager";
			}

			Department devDepartment = ObjectSpace.FindObject<Department>(CriteriaOperator.Parse("Title == 'Development Department'"));
			if(devDepartment == null) {
				devDepartment = ObjectSpace.CreateObject<Department>();
				devDepartment.Title = "Development Department";
				devDepartment.Office = "205";
				devDepartment.Positions.Add(developerPosition);
				devDepartment.Positions.Add(managerPosition);
			}

			Contact contactMary = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Mary' && LastName == 'Tellitson'"));
			if(contactMary == null) {
				contactMary = ObjectSpace.CreateObject<Contact>();
				contactMary.FirstName = "Mary";
				contactMary.LastName = "Tellitson";
				contactMary.Email = "mary_tellitson@md.com";
				contactMary.Birthday = new DateTime(1980, 11, 27);
				contactMary.Department = devDepartment;
				contactMary.Position = managerPosition;
			}
			Contact contactJohn = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'John' && LastName == 'Nilsen'"));
			if(contactJohn == null) {
				contactJohn = ObjectSpace.CreateObject<Contact>();
				contactJohn.FirstName = "John";
				contactJohn.LastName = "Nilsen";
				contactJohn.Email = "john_nilsen@md.com";
				contactJohn.Birthday = new DateTime(1981, 10, 3);
				contactJohn.Department = devDepartment;
				contactJohn.Position = developerPosition;
			}

			if(ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Review reports'")) == null) {
				DemoTask task = ObjectSpace.CreateObject<DemoTask>();
				task.Subject = "Review reports";
				task.AssignedTo = contactJohn;
				task.StartDate = DateTime.Parse("May 03, 2008");
				task.DueDate = DateTime.Parse("September 06, 2008");
				task.Status = DevExpress.Persistent.Base.General.TaskStatus.InProgress;
				task.Priority = Priority.High;
				task.EstimatedWork = 60;
				task.Description = "Analyse the reports and assign new tasks to employees.";
			}

			if(ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Fix breakfast'")) == null) {
				DemoTask task = ObjectSpace.CreateObject<DemoTask>();
				task.Subject = "Fix breakfast";
				task.AssignedTo = contactMary;
				task.StartDate = DateTime.Parse("May 03, 2008");
				task.DueDate = DateTime.Parse("May 04, 2008");
				task.Status = DevExpress.Persistent.Base.General.TaskStatus.Completed;
				task.Priority = Priority.Low;
				task.EstimatedWork = 1;
				task.ActualWork = 3;
				task.Description = "The Development Department - by 9 a.m.\r\nThe R&QA Department - by 10 a.m.";
			}
			if(ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Task1'")) == null) {
				DemoTask task = ObjectSpace.CreateObject<DemoTask>();
				task.Subject = "Task1";
				task.AssignedTo = contactJohn;
				task.StartDate = DateTime.Parse("June 03, 2008");
				task.DueDate = DateTime.Parse("June 06, 2008");
				task.Status = DevExpress.Persistent.Base.General.TaskStatus.Completed;
				task.Priority = Priority.High;
				task.EstimatedWork = 10;
				task.ActualWork = 15;
				task.Description = "A task designed specially to demonstrate the PivotChart module. Switch to the Reports navigation group to view the generated analysis.";
			}
			if(ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Task2'")) == null) {
				DemoTask task = ObjectSpace.CreateObject<DemoTask>();
				task.Subject = "Task2";
				task.AssignedTo = contactJohn;
				task.StartDate = DateTime.Parse("July 03, 2008");
				task.DueDate = DateTime.Parse("July 06, 2008");
				task.Status = DevExpress.Persistent.Base.General.TaskStatus.Completed;
				task.Priority = Priority.Low;
				task.EstimatedWork = 8;
				task.ActualWork = 16;
				task.Description = "A task designed specially to demonstrate the PivotChart module. Switch to the Reports navigation group to view the generated analysis.";
			}

			CreateDataToBeAnalysed();

			// Create Users for the Complex Security Strategy
			// If a user named 'Sam' doesn't exist in the database, create this user
			User user1 = ObjectSpace.FindObject<User>(new BinaryOperator("UserName", "Sam"));
			if(user1 == null) {
				user1 = ObjectSpace.CreateObject<User>();
				user1.UserName = "Sam";
				// Set a password if the standard authentication type is used
				user1.SetPassword("");
			}
			// If a user named 'John' doesn't exist in the database, create this user
			User user2 = ObjectSpace.FindObject<User>(new BinaryOperator("UserName", "John"));
			if(user2 == null) {
				user2 = ObjectSpace.CreateObject<User>();
				user2.UserName = "John";
				// Set a password if the standard authentication type is used
				user2.SetPassword("");
			}
			// If a role with the Administrators name doesn't exist in the database, create this role
			Role adminRole = ObjectSpace.FindObject<Role>(new BinaryOperator("Name", "Administrators"));
			if(adminRole == null) {
				adminRole = ObjectSpace.CreateObject<Role>();
				adminRole.Name = "Administrators";
				// Add the Administrators role to the user1
				user1.Roles.Add(adminRole);
			}
			adminRole.IsAdministrative = true;

			// If a role with the Users name doesn't exist in the database, create this role
			Role userRole = ObjectSpace.FindObject<Role>(new BinaryOperator("Name", "Users"));
			if(userRole == null) {
				userRole = ObjectSpace.CreateObject<Role>();
				userRole.Name = "Users";
				// Add the Users role to the user2
				user2.Roles.Add(userRole);
				user2.Roles.Add(defaultRole);
			}

			ObjectSpace.CommitChanges();
		}
	}

	public abstract class TaskAnalysis1LayoutUpdaterBase {
		protected IObjectSpace objectSpace;
		protected abstract IAnalysisControl CreateAnalysisControl();
		protected abstract IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control);
		public TaskAnalysis1LayoutUpdaterBase(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
		}
		public void Update(Analysis analysis) {
			if(analysis != null && !PivotGridSettingsHelper.HasPivotGridSettings(analysis)) {
				IAnalysisControl control = CreateAnalysisControl();
				control.DataSource = new AnalysisDataSource(analysis, objectSpace.GetObjects(typeof(DemoTask)));
				control.Fields["Priority"].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
				control.Fields["Subject"].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
				control.Fields["AssignedTo.DisplayName"].Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
				PivotGridSettingsHelper.SavePivotGridSettings(CreatePivotGridSettingsStore(control), analysis);
			}
		}
	}

	public abstract class TaskAnalysis2LayoutUpdaterBase {
		protected IObjectSpace objectSpace;
		protected abstract IAnalysisControl CreateAnalysisControl();
		protected abstract IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control);
		public TaskAnalysis2LayoutUpdaterBase(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
		}
		public void Update(Analysis analysis) {
			if(analysis != null && !PivotGridSettingsHelper.HasPivotGridSettings(analysis)) {
				IAnalysisControl control = CreateAnalysisControl();
				control.DataSource = new AnalysisDataSource(analysis, objectSpace.GetObjects(typeof(DemoTask)));
				control.Fields["Status"].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
				control.Fields["Priority"].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
				control.Fields["EstimatedWork"].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
				control.Fields["ActualWork"].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
				control.Fields["AssignedTo.DisplayName"].Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
				PivotGridSettingsHelper.SavePivotGridSettings(CreatePivotGridSettingsStore(control), analysis);
			}
		}
	}
}
