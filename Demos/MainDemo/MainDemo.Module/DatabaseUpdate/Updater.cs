using System;
using System.Data.SqlClient;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using MainDemo.Module.BusinessObjects;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System.Drawing;
using DevExpress.ExpressApp.Utils;
using System.Data;
using System.IO;
using System.Collections.Generic;

namespace MainDemo.Module.DatabaseUpdate {
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            UpdateAnalysisCriteriaColumn();

            // PermissionPolicyRole defaultRole = CreateDefaultRole();

            UpdateStatus("CreateContacts", "", "Creating contacts, departments and positions in the database...");
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
            Department seoDepartment = ObjectSpace.FindObject<Department>(CriteriaOperator.Parse("Title == 'SEO'"));
            if(seoDepartment == null) {
                seoDepartment = ObjectSpace.CreateObject<Department>();
                seoDepartment.Title = "SEO";
                seoDepartment.Office = "703";
                seoDepartment.Positions.Add(developerPosition);
                seoDepartment.Positions.Add(managerPosition);
            }
            ObjectSpace.CommitChanges();

            try {
                DataTable employeesTable = GetEmployeesDataTable();
                foreach(DataRow employee in employeesTable.Rows) {
                    string email = Convert.ToString(employee["EmailAddress"]);
                    Contact contact = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("Email=?", email));
                    if(contact == null) {
                        contact = ObjectSpace.CreateObject<Contact>();
                        contact.Email = email;
                        contact.FirstName = Convert.ToString(employee["FirstName"]);
                        contact.LastName = Convert.ToString(employee["LastName"]);
                        contact.Birthday = Convert.ToDateTime(employee["BirthDate"]);
                        contact.Photo = Convert.FromBase64String(Convert.ToString(employee["ImageData"]));
                        string titleOfCourtesyText = Convert.ToString(employee["Title"]).ToLower();
                        if(!string.IsNullOrEmpty(titleOfCourtesyText)) {
                            titleOfCourtesyText = titleOfCourtesyText.Replace(".", "");
                            TitleOfCourtesy titleOfCourtesy;
                            if(Enum.TryParse<TitleOfCourtesy>(titleOfCourtesyText, true, out titleOfCourtesy)) {
                                contact.TitleOfCourtesy = titleOfCourtesy;
                            }
                        }
                        PhoneNumber phoneNumber = ObjectSpace.CreateObject<PhoneNumber>();
                        phoneNumber.Party = contact;
                        phoneNumber.Number = Convert.ToString(employee["Phone"]);
                        phoneNumber.PhoneType = "Work";

                        Address address = ObjectSpace.CreateObject<Address>();
                        contact.Address1 = address;
                        address.ZipPostal = Convert.ToString(employee["PostalCode"]);
                        address.Street = Convert.ToString(employee["AddressLine1"]);
                        address.City = Convert.ToString(employee["City"]);
                        address.StateProvince = Convert.ToString(employee["StateProvinceName"]);
                        string countryName = Convert.ToString(employee["CountryRegionName"]);
                        Country country = ObjectSpace.FindObject<Country>(CriteriaOperator.Parse("Name=?", countryName), true);
                        if(country == null) {
                            country = ObjectSpace.CreateObject<Country>();
                            country.Name = countryName;
                        }
                        address.Country = country;

                        string departmentTitle = Convert.ToString(employee["GroupName"]);
                        Department department = ObjectSpace.FindObject<Department>(CriteriaOperator.Parse("Title=?", departmentTitle), true);
                        if(department == null) {
                            department = ObjectSpace.CreateObject<Department>();
                            department.Title = departmentTitle;
                            Random rnd = new Random();
                            department.Office = string.Format("{0}0{0}", rnd.Next(1, 7), rnd.Next(9));
                        }
                        contact.Department = department;

                        string positionTitle = Convert.ToString(employee["JobTitle"]);
                        Position position = ObjectSpace.FindObject<Position>(CriteriaOperator.Parse("Title=?", positionTitle), true);
                        if(position == null) {
                            position = ObjectSpace.CreateObject<Position>();
                            position.Title = positionTitle;
                            position.Departments.Add(department);
                        }
                        contact.Position = position;
                    }
                }
                ObjectSpace.CommitChanges();
            }
            catch(Exception e) {
                Tracing.Tracer.LogText("Cannot initialize contacts, departments and positions from the XML file.");
                Tracing.Tracer.LogError(e);
            }


            Contact contactMary = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Mary' && LastName == 'Tellitson'"));
            if(contactMary != null && LocationIsEmpty(contactMary)) {
                if(contactMary.Location == null)
                    contactMary.Location = ObjectSpace.CreateObject<Location>();

                contactMary.Location.Contact = contactMary;
                contactMary.Location.Latitude = 40.620610;
                contactMary.Location.Longitude = -73.935242;
            }

            Contact contactJohn = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'John' && LastName == 'Nilsen'"));
            if(contactJohn != null && LocationIsEmpty(contactJohn)) {
                if(contactJohn.Location == null)
                    contactJohn.Location = ObjectSpace.CreateObject<Location>();

                contactJohn.Location.Contact = contactJohn;
                contactJohn.Location.Latitude = 40.711510;
                contactJohn.Location.Longitude = -73.845252;
            }

            Contact contactJanete = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Janete' && LastName == 'Limeira'"));
            if(contactJanete != null && LocationIsEmpty(contactJanete)) {
                if(contactJanete.Location == null)
                    contactJanete.Location = ObjectSpace.CreateObject<Location>();

                contactJanete.Location.Contact = contactJanete;
                contactJanete.Location.Latitude = 40.710410;
                contactJanete.Location.Longitude = -73.963262;
            }

            Contact contactKarl = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Karl' && LastName == 'Jablonski'"));
            if(contactKarl != null && LocationIsEmpty(contactKarl)) {
                if(contactKarl.Manager == null) {
                    contactKarl.Manager = contactJanete;
                }
                if(contactKarl.Location == null)
                    contactKarl.Location = ObjectSpace.CreateObject<Location>();

                contactKarl.Location.Contact = contactKarl;
                contactKarl.Location.Latitude = 40.792613;
                contactKarl.Location.Longitude = -73.925142;
            }


            ObjectSpace.CommitChanges();


            UpdateStatus("CreatePayments", "", "Creating payments, resumes and scheduler events in the database...");
            IList<Contact> topTenContacts = ObjectSpace.GetObjects<Contact>();
            ObjectSpace.SetCollectionSorting(topTenContacts, new SortProperty[] { new SortProperty("LastName", DevExpress.Xpo.DB.SortingDirection.Ascending) });
            ObjectSpace.SetTopReturnedObjectsCount(topTenContacts, 10);
            string[] notes = {
                "works with customers until their problems are resolved and often goes an extra step to help upset customers be completely surprised by how far we will go to satisfy customers",
                "is very good at making team members feel included. The inclusion has improved the team's productivity dramatically",
                "is very good at sharing knowledge and information during a problem to increase the chance it will be resolved quickly",
                "actively elicits feedback from customers and works to resolve their problems",
                "creates an inclusive work environment where everyone feels they are a part of the team",
                "consistently keeps up on new trends in the industry and applies these new practices to every day work",
                "is clearly not a short term thinker - the ability to set short and long term business goals is a great asset to the company",
                "seems to want to achieve all of the goals in the last few weeks before annual performance review time, but does not consistently work towards the goals throughout the year",
                "does not yet delegate effectively and has a tendency to be overloaded with tasks which should be handed off to subordinates",
                "to be discussed with the top management..."
            };
            for(int i = 0; i < topTenContacts.Count; i++) {
                Contact contact = topTenContacts[i];
                Payment payment = ObjectSpace.FindObject<Payment>(CriteriaOperator.Parse("Contact=?", contact));
                if(payment == null) {
                    payment = ObjectSpace.CreateObject<Payment>();
                    payment.Contact = contact;
                    payment.Hours = new Random().Next(10, 40);
                    payment.Rate = new Random().Next(30, 50) + new Random().Next(5, 20);
                }
                Resume resume = ObjectSpace.FindObject<Resume>(CriteriaOperator.Parse("Contact=?", contact));
                if(resume == null) {
                    resume = ObjectSpace.CreateObject<Resume>();
                    FileData file = ObjectSpace.CreateObject<FileData>();
                    try {
                        file.LoadFromStream(string.Format("{0}_Photo.png", contact.FullName), new MemoryStream(contact.Photo));
                    }
                    catch(Exception e) {
                        Tracing.Tracer.LogText("Cannot initialize FileData for the contact {0}.", contact.FullName);
                        Tracing.Tracer.LogError(e);
                    }
                    resume.File = file;
                    resume.Contact = contact;
                }
                Contact reviewerContact = i < 5 ? contactMary : contactJanete;
                Note note = ObjectSpace.FindObject<Note>(CriteriaOperator.Parse("Contains(Text, ?)", contact.FullName));
                if(note == null) {
                    note = ObjectSpace.CreateObject<Note>();
                    note.Author = reviewerContact.FullName;
                    note.Text = string.Format("{0} \r\n{1}", contact.FullName, notes[i]);
                    note.DateTime = DateTime.Now.AddDays(i * (-1));
                }
#if !EASYTEST
                Event appointment = ObjectSpace.FindObject<Event>(CriteriaOperator.Parse("Contains(Subject, ?)", contact.FullName));
                if(appointment == null) {
                    appointment = ObjectSpace.CreateObject<Event>();
                    appointment.Subject = string.Format("{0} - performance review", contact.FullName);
                    appointment.Description = note.Text;
                    appointment.StartOn = note.DateTime.AddDays(5).AddHours(12);
                    appointment.EndOn = appointment.StartOn.AddHours(2);
                    appointment.Location = "101";
                    appointment.AllDay = false;
                    appointment.Status = 0;
                    appointment.Label = i % 2 == 0 ? 2 : 5;
                    Resource reviewerContactResource = ObjectSpace.FindObject<Resource>(CriteriaOperator.Parse("Contains(Caption, ?)", reviewerContact.FullName));
                    if(reviewerContactResource == null) {
                        reviewerContactResource = ObjectSpace.CreateObject<Resource>();
                        reviewerContactResource.Caption = reviewerContact.FullName;
                        reviewerContactResource.Color = reviewerContact == contactMary ? Color.AliceBlue : Color.LightCoral;
                    }
                    appointment.Resources.Add(reviewerContactResource);
                }
#endif
            }

            ObjectSpace.CommitChanges();

            UpdateStatus("CreateTasks", "", "Creating demo tasks in the database...");


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
            UpdateStatus("CreateAnalysis", "", "Creating analysis reports in the database...");
            CreateDataToBeAnalysed();
            UpdateStatus("CreateSecurityData", "", "Creating users and roles in the database...");
            #region Create a User for the Simple Security Strategy
            //// If a simple user named 'Sam' doesn't exist in the database, create this simple user
            //SecuritySimpleUser adminUser = ObjectSpace.FindObject<SecuritySimpleUser>(new BinaryOperator("UserName", "Sam"));
            //if(adminUser == null) {
            //    adminUser = ObjectSpace.CreateObject<SecuritySimpleUser>();
            //    adminUser.UserName = "Sam";
            //}
            //// Make the user an administrator
            //adminUser.IsAdministrator = true;
            //// Set a password if the standard authentication type is used
            //adminUser.SetPassword("");
            #endregion

            #region Create Users for the Complex Security Strategy
            // If a user named 'Sam' doesn't exist in the database, create this user
            PermissionPolicyUser user1 = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "Sam"));
            if(user1 == null) {
                user1 = ObjectSpace.CreateObject<PermissionPolicyUser>();
                user1.UserName = "Sam";
                // Set a password if the standard authentication type is used
                user1.SetPassword("");
            }
            // If a user named 'John' doesn't exist in the database, create this user
            PermissionPolicyUser user2 = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "John"));
            if(user2 == null) {
                user2 = ObjectSpace.CreateObject<PermissionPolicyUser>();
                user2.UserName = "John";
                // Set a password if the standard authentication type is used
                user2.SetPassword("");
            }
            // If a role with the Administrators name doesn't exist in the database, create this role
            PermissionPolicyRole adminRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Administrators"));
            if(adminRole == null) {
                adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                adminRole.Name = "Administrators";
            }
            adminRole.IsAdministrative = true;

            // If a role with the Users name doesn't exist in the database, create this role
            PermissionPolicyRole userRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Users"));
            if(userRole == null) {
                userRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
                userRole.Name = "Users";
                userRole.PermissionPolicy = SecurityPermissionPolicy.AllowAllByDefault;
                userRole.AddNavigationPermission("Application/NavigationItems/Items/Default/Items/PermissionPolicyRole_ListView", SecurityPermissionState.Deny);
                userRole.AddNavigationPermission("Application/NavigationItems/Items/Default/Items/PermissionPolicyUser_ListView", SecurityPermissionState.Deny);
                userRole.AddTypePermission<PermissionPolicyRole>(SecurityOperations.FullAccess, SecurityPermissionState.Deny);
                userRole.AddTypePermission<PermissionPolicyUser>(SecurityOperations.FullAccess, SecurityPermissionState.Deny);
                userRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.ReadOnlyAccess, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                userRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", null, SecurityPermissionState.Allow);
                userRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", null, SecurityPermissionState.Allow);
                userRole.AddTypePermission<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Allow);
                userRole.AddTypePermission<PermissionPolicyTypePermissionObject>("Write;Delete;Create", SecurityPermissionState.Deny);
                userRole.AddTypePermission<PermissionPolicyMemberPermissionsObject>("Write;Delete;Create", SecurityPermissionState.Deny);
                userRole.AddTypePermission<PermissionPolicyObjectPermissionsObject>("Write;Delete;Create", SecurityPermissionState.Deny);
            }

            // Add the Administrators role to the user1
            user1.Roles.Add(adminRole);
            // Add the Users role to the user2
            user2.Roles.Add(userRole);
            #endregion

            ObjectSpace.CommitChanges();
        }
        private bool LocationIsEmpty(Contact contact) {
            return contact.Location == null || contact.Location.Contact == null || (contact.Location.Latitude == 0 && contact.Location.Longitude == 0);
        }
        private DataTable GetEmployeesDataTable() {
            string shortName = "EmployeesWithPhoto.xml";
            string embeddedResourceName = Array.Find<string>(this.GetType().Assembly.GetManifestResourceNames(), (s) => { return s.Contains(shortName); });
            Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedResourceName);
            if(stream == null) {
                throw new Exception(string.Format("Cannot read employees data from the {0} file!", shortName));
            }
            DataSet ds = new DataSet();
            ds.ReadXml(stream);
            return ds.Tables["Employee"];
        }
        private void CreateDataToBeAnalysed() {
            Analysis taskAnalysis1 = ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Completed tasks'"));
            if(taskAnalysis1 == null) {
                taskAnalysis1 = ObjectSpace.CreateObject<Analysis>();
                taskAnalysis1.Name = "Completed tasks";
                taskAnalysis1.ObjectTypeName = typeof(DemoTask).FullName;
                taskAnalysis1.Criteria = "[Status] = ##Enum#DevExpress.Persistent.Base.General.TaskStatus,Completed#";
            }
            Analysis taskAnalysis2 = ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Estimated and actual work comparison'"));
            if(taskAnalysis2 == null) {
                taskAnalysis2 = ObjectSpace.CreateObject<Analysis>();
                taskAnalysis2.Name = "Estimated and actual work comparison";
                taskAnalysis2.ObjectTypeName = typeof(DemoTask).FullName;
            }
        }
        private void UpdateAnalysisCriteriaColumn(){
            var sqlConnection = ((XPObjectSpace)ObjectSpace).Session.Connection as SqlConnection;
            if(sqlConnection!=null&&!sqlConnection.Database.EndsWith("WC")) {
                int length = (int)ExecuteScalarCommand(@"select CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.Columns WHERE TABLE_NAME = 'Analysis' AND COLUMN_NAME = 'Criteria'", true);
                if(length != -1) {
                    ExecuteNonQueryCommand("alter table Analysis alter column Criteria nvarchar(max)", true);
                }
            }
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
