using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using TestEntities;
using System.Configuration;
using TestDataLayer;
using Xpand.ExpressApp.NH.DataLayer;
using TestDataLayer.Maps;
using Xpand.ExpressApp.NH;
using Xpand.ExpressApp.NH.Core;
using Xpand.ExpressApp.NH.BaseImpl;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Module
{
    // For more typical usage scenarios, be sure to check out http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppModuleBasetopic.
    public sealed partial class NHibernateIntegrationModule : ModuleBase
    {
        public NHibernateIntegrationModule()
        {
            InitializeComponent();
            AdditionalExportedTypes.Add(typeof(Person));
            AdditionalExportedTypes.Add(typeof(PhoneNumber));
            AdditionalExportedTypes.Add(typeof(User));
            AdditionalExportedTypes.Add(typeof(Role));
            AdditionalExportedTypes.Add(typeof(TypePermission));
            AdditionalExportedTypes.Add(typeof(ObjectPermission));
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
        public override void Setup(XafApplication application)
        {
            
            base.Setup(application);
            application.CreateCustomObjectSpaceProvider += application_CreateCustomObjectSpaceProvider;
            application.CreateCustomPropertyCollectionSource += application_CreateCustomPropertyCollectionSource;
            application.LoggingOn += application_LoggingOn;
            // Manage various aspects of the application UI and behavior at the module level.
        }

        void application_LoggingOn(object sender, LogonEventArgs e)
        {
            using (var objectSpace = Application.CreateObjectSpace())
            {
                var users = objectSpace.GetObjects(typeof(User));
                if (users.Count == 0)
                {
                    var adminUser = objectSpace.CreateObject<User>();
                    adminUser.UserName = "Administrator";
                    adminUser.IsActive = true;
                    adminUser.Roles.Add(new Role { Name = "Administrator", CanEditModel = true, IsAdministrative = true });
                    objectSpace.CommitChanges();
                }
            }
        }

        void application_CreateCustomPropertyCollectionSource(object sender, CreateCustomPropertyCollectionSourceEventArgs e)
        {
            e.PropertyCollectionSource =  new NHPropertyCollectionSource(e.ObjectSpace, e.MasterObjectType, e.MasterObject, e.MemberInfo, e.Mode);    
        }


        void application_CreateCustomObjectSpaceProvider(object sender, CreateCustomObjectSpaceProviderEventArgs e)
        {
            //Use following two lines for the local connection:
            //var persistenceManager = new PersistenceManager(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            //persistenceManager.AddMappingAssembly(typeof(PersonMap).Assembly);

            var persistenceManager = (IPersistenceManager)new RemotePersistenceManagerProxy(
                "http://localhost:8733/Design_Time_Addresses/NHibernateService/Service1").GetTransparentProxy();
            e.ObjectSpaceProvider = new NH.NHObjectSpaceProvider(XafTypesInfo.Instance, persistenceManager,
                Application.Security as ISelectDataSecurityProvider);
        }
    }
}
