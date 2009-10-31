using System;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using eXpand.Persistent.BaseImpl.PersistentMetaData;

namespace Solution3.Module {
    public class Updater : ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) { }
         
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            Debug.Print("");
            // If a user named 'Sam' doesn't exist in the database, create this user

            var user1 = Session.FindObject<User>(new BinaryOperator("UserName", "Sam"));



            if (user1 == null)
            {
                new PersistentClassInfo(Session){Name = "Customer"}.Save();
                new PersistentClassInfo(Session){Name = "Order"}.Save();
                new PersistentClassInfo(Session){Name = "OrderLine"}.Save();
                user1 = new User(Session) { UserName = "Sam", FirstName = "Sam" };
                // Set a password if the standard authentication type is used
                user1.SetPassword("");
                user1.Save();
                
            }
            // If a user named 'John' doesn't exist in the database, create this user
            var user2 = Session.FindObject<User>(new BinaryOperator("UserName", "John"));
            if (user2 == null)
            {
                user2 = new User(Session) { UserName = "John", FirstName = "John" };
                // Set a password if the standard authentication type is used
                user2.SetPassword("");
                InitializeDB(user1);
                // If a role with the Administrators name doesn't exist in the database, create this role

                Role adminRole = Session.FindObject<Role>(new BinaryOperator("Name", "Administrators")) ??
                                 new Role(Session) { Name = "Administrators" };

                // If a role with the Users name doesn't exist in the database, create this role
                var userRole = Session.FindObject<Role>(new BinaryOperator("Name", "Users")) ?? new Role(Session) { Name = "Users" };

                // Delete all permissions assigned to the Administrators and Users roles
                while (adminRole.PersistentPermissions.Count > 0)
                {
                    Session.Delete(adminRole.PersistentPermissions[0]);
                }
                // Allow full access to all objects to the Administrators role
                adminRole.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
                // Deny editing access to the AuditDataItemPersistent type objects to the Administrators role
                adminRole.AddPermission(new ObjectAccessPermission(typeof(AuditDataItemPersistent),
                                                                   ObjectAccess.ChangeAccess, ObjectAccessModifier.Deny));
                // Allow editing the application model to the Administrators role
                adminRole.AddPermission(new EditModelPermission(ModelAccessModifier.Allow));
                // Save the Administrators role to the database
                adminRole.Save();
                // Allow full access to all objects to the Users role
                addPermissionsToSimpleRoles(userRole);

                // Add the Administrators role to the user1
                user1.Roles.Add(adminRole);
                // Add the Users role to the user2
                user2.Roles.Add(userRole);


                // Save the users to the database
                user1.Save();
                user2.Save();

            }
        }


        public void InitializeDB(User user1)
        {
            var customerNames = new[]
                                {
                                    "John SMITH", "Tom SAILOR", "Robert DUVAL", "Ken WONG ", "Jerry MERRY",
                                    "Maria JONES", "James T KIRK", "Jean-Luc PICARD", "Benjamin CISCO",
                                    "Jordy LAFORGE"
                                };

            var citynames = new[]
                            {
                                "New York", "Paris", "London", "Hong Kong"
                            };

            var articles = new[]
                           {
                               "X-Ray glasses", "Laser gun", "Photon torpedoe", "Candle pack", "Electric Toothbush",
                               "Batteries", "Mobile phone", "Set of saucepans", "WiFI access Point", "Talking robot"
                           };
            var rand = new Random();

            // Create random prices for our articles
            var artprice = new Dictionary<string, float>();
            foreach (string ar in articles) artprice.Add(ar, rand.Next(10, 200));

//            // Create custoemrs living in random cities
//            Customer cust;
//            Order order;
//            OrderLine line;
//            int counter = 0;
//            foreach (string cn in customerNames)
//            {
//                // Create and save the customer
//                cust = new Customer(Session) { Name = cn, City = citynames[rand.Next(citynames.Length)] };
//                //                cust.SetMemberValue("User",user1.Oid);
//                // Create random number of orders for the customer
//                for (int i = 0; i < rand.Next(5, 20); i++)
//                {
//                    order = new Order(Session)
//                            {
//                                Customer = cust,
//                                Date = DateTime.Now.AddDays((-rand.Next(365 * 3))),
//                                Reference = String.Format("ORD{0}", counter + 1000),
//                                Total = 0F
//                            };
//                    //                    order.SetMemberValue("User", user1.Oid);
//                    // Set random date within the last 3 years
//                    counter++;
//                    for (int j = 0; j < rand.Next(2, 8); j++)
//                    {
//                        line = new OrderLine(Session) { Order = order, Article = articles[rand.Next(articles.Length)] };
//                        line.UnitPrice = artprice[line.Article];
//                        line.Quantity = rand.Next(1, 4);
//                        order.Total += line.TotalPrice;
//                        //                        line.SetMemberValue("User", user1.Oid);
//                        line.Save();
//                    }
//                    order.Save();
//                }
//                cust.Save();
//            }


        }
        private void addPermissionsToSimpleRoles(Role courierRole)
        {
            courierRole.AddPermission(new EditModelPermission(ModelAccessModifier.Allow));
            courierRole.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
            // Deny editing access to the User type objects to the Users role
            //            courierRole.AddPermission(new ObjectAccessPermission(typeof(User), ObjectAccess.Navigate, ObjectAccessModifier.Deny));
            // Deny full access to the Role type objects to the Users role
            courierRole.AddPermission(new ObjectAccessPermission(typeof(Role), ObjectAccess.AllAccess,
                                                                 ObjectAccessModifier.Deny));
            // Deny editing the application model to the Users role

            // Save the Users role to the database
            courierRole.Save();
        }
    }
}